using Nest;
using Procergs.ShoppingList.Service.Dtos;
using Procergs.ShoppingList.Service.Extensions;
using Procergs.ShoppingList.Service.Interfaces;
using Procergs.ShoppingList.Service.Repositories;

namespace Procergs.ShoppingList.Service.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListRepository shoppingListRepository;
        private readonly IElasticClient elasticClient;

        public ShoppingListService(IShoppingListRepository shoppingListRepository, IElasticClient elasticClient) 
        {
            this.shoppingListRepository = shoppingListRepository;
            this.elasticClient = elasticClient;
        }

        public async Task<IEnumerable<ShoppingListDto>> GetAllByUserAsync(string userCpf)
        {

            try
            {
                var shoppingLists = await shoppingListRepository.GetAllByUserAsync(userCpf);

                if (shoppingLists == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuario.");

                return shoppingLists.Select(shoppingList => shoppingList.AsDto()); ;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ShoppingListDto> GetByIDAsync(Guid id)
        {
            try
            {
                var shoppingList = await shoppingListRepository.GetByIDAsync(id);

                if (shoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras com esse ID");
                
                return shoppingList.AsDto();
            }
            catch (Exception)
            {
                throw;
            } 
        }

        public async Task<ShoppingListDto> CreateAsync(CreateShoppingListDto createShoppingListDto)
        {
            try
            {
                var existingShoppingLists = await shoppingListRepository.GetAllByUserAsync(createShoppingListDto.UserCpf);

                if(existingShoppingLists != null && existingShoppingLists.Count() == 5)
                    throw new Exception("Usuário já tem número máximo de listas cadastradas.");

                var shoppingList = new Entities.ShoppingList
                {
                    UserCpf = createShoppingListDto.UserCpf,
                    Name = createShoppingListDto.Name,
                    Products = new List<ProductDto>()
                };

                await shoppingListRepository.CreateAsync(shoppingList);

                return await GetByIDAsync(shoppingList.Id);

            }
            catch (Exception)
            {
                throw;
            }            
        }
        
        public async Task UpdateAsync(UpdateShoppingListDto updateShoppingListDto)
        {
            try
            {
                var existingShoppingList = await shoppingListRepository.GetByIDAsync(updateShoppingListDto.Id);

                if (existingShoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras com esse Id.");

                existingShoppingList.Name = updateShoppingListDto.Name;

                await shoppingListRepository.UpdateAsync(existingShoppingList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddProductAsync(Guid listID, ProductDto productDto)
        {
            try
            {
                var existingShoppingList = await shoppingListRepository.GetByIDAsync(listID);

                if (existingShoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras com esse ID.");

                existingShoppingList.Products.Add(productDto);

                await shoppingListRepository.UpdateAsync(existingShoppingList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveProductAsync(Guid listID, string gtinToRemove)
        {
            try
            {
                var existingShoppingList = await shoppingListRepository.GetByIDAsync(listID);

                if (existingShoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuário.");

                existingShoppingList.Products.Remove(existingShoppingList.Products.FirstOrDefault(product => product.Gtin == gtinToRemove));

                await shoppingListRepository.UpdateAsync(existingShoppingList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BestPlaceDto> FindBestBuyPlace(SearchDto pesquisaDto)
        {
            List<ElasticDto> elasticProductsDto = new List<ElasticDto>();
            IEnumerable<IGrouping<long, ElasticDto>> groupedProducts;
            IOrderedEnumerable<IGrouping<long, ElasticDto>> orderedGroups;
            int highScore;

            try
            {
                var shoppingList = await shoppingListRepository.GetByIDAsync(pesquisaDto.ListID);

                if (shoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuário.");

                // Para cada produto na lista de compras
                foreach (var product in shoppingList.Products)
                {
                    // Realiza a pesquisa no Elasticsearch
                    var result = await SearchAProductList(pesquisaDto, product.Gtin);
                    var currentList = result.ToList();

                    if (elasticProductsDto.Count() == 0)
                        elasticProductsDto = currentList;
                    else
                        elasticProductsDto.AddRange(currentList);
                }

                // Grouped by Cnpj
                groupedProducts = elasticProductsDto.GroupBy(product => product.Estabelecimento.CodCnpjEstab);

                // Verificar qual grupo tem mais elementos // Ordena pelo Count
                orderedGroups = groupedProducts.OrderByDescending(group => group.Count());

                highScore = orderedGroups.Max(group => group.Count());

                // Pega apenas grupos com a maior quantidade de elementos
                groupedProducts = orderedGroups.TakeWhile(group => group.Count() == highScore);

                // Ordena por menor valor e por menor distância
                orderedGroups = groupedProducts.OrderBy(group => group.Sum(product => product.VlrItem)).ThenByDescending(g => g.FirstOrDefault().Estabelecimento.KmDistancia);

                // Pega o primeiro grupo de produtos da lista
                var products = orderedGroups.FirstOrDefault();
                var productInfo = products.First();
                decimal amount = 0;

                List<BestPlaceProductDto> productsDto = new List<BestPlaceProductDto>();
                foreach (var product in products)
                {
                    amount += product.VlrItem;
                    productsDto.Add(product.AsDto());
                }

                BestPlaceDto bestPlace = new BestPlaceDto()
                {
                    Name = productInfo.Estabelecimento.NomeContrib,
                    Cnpj = productInfo.Estabelecimento.CodCnpjEstab.ToString(),
                    Distance = productInfo.Estabelecimento.KmDistancia,
                    ProductDtos = productsDto,
                    Amount = amount
                };

                return bestPlace;
               
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal async Task<IEnumerable<ElasticDto>> SearchAProductList(SearchDto pesquisaDto, string gtin)
        {
            //Configuração de filtro escolhido
            List<Func<QueryContainerDescriptor<ElasticDto>, QueryContainer>> query = new List<Func<QueryContainerDescriptor<ElasticDto>, QueryContainer>>();

            query.Add((m => m
                        .Match(mp => mp
                        .Field(fi => fi.TipoVenda)
                        .Query("1"))));

            query.Add(m => m.DateRange(d => d
                         .Field(f => f.DthEmiNFe)
                         .GreaterThanOrEquals(DateTime.Now.AddDays(-(pesquisaDto.DayLimit)))));

            query.Add((m => m
                   .Match(mp => mp
                   .Field(fi => fi.Gtin)
                   .Query(gtin))));


            var request = new Func<SearchDescriptor<ElasticDto>, ISearchRequest>(s => s
                   .From(0)
                   .Size(500)
                   .Index("sefaz_mprs_item")
                   .Query(q => q
                        .Bool(b => b
                        .Must(query)
                        .Filter(fi => fi
                                .GeoDistance(geo => geo
                                    .Distance(new Distance(pesquisaDto.MaxDistance, DistanceUnit.Kilometers))
                                    .Location(new GeoLocation(pesquisaDto.Latitude, pesquisaDto.Longitude))
                                    .Field(fs => fs.Estabelecimento.Localizacao)
                                    ))))
                  .Sort(y => y
                    .Descending(SortSpecialField.Score)
                  ));

            //Query de consulta no Elastic Search
            ISearchResponse<ElasticDto> SearchResponse = await elasticClient.SearchAsync<ElasticDto>(request);

            // Calcula a distância para cada um dos produtos retornados
            foreach (var item in SearchResponse.Documents)
            {
                item.Estabelecimento.KmDistancia = GeoCalculator
                    .CalculateDistance(item.Estabelecimento.NroLatitude, item.Estabelecimento.NroLongitude,
                                      pesquisaDto.Latitude, pesquisaDto.Longitude, 'K');

                if (item.CodGrupoAnp.HasValue && item.CodGrupoAnp.Value > 0)
                {
                    item.VlrItem = item.VlrItem + item.VlrDescItem;
                    item.VlrDescItem = 0;
                }
            }

            // Prepara as lambdas
            Func<ElasticDto, Object> campoOrdemPrincipal;
            Func<ElasticDto, Object> campoOrdemComplementar;

            campoOrdemPrincipal = f => f.VlrItem;
            campoOrdemComplementar = f => f.Estabelecimento.KmDistancia;

            IEnumerable<ElasticDto> itens;

            // Ordena todos produtos retornados por Preço e Distância
            itens = SearchResponse.Documents.OrderByDescending(campoOrdemPrincipal).ThenBy(campoOrdemComplementar);

            //Remove os duplicados
            return itens.GroupBy(item => new { item.TexDesc, item.Estabelecimento.CodCnpjEstab }).Select(item => item.First()).Take(500);
        }
    }

}

