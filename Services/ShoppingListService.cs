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

        private IEnumerable<IGrouping<long, ElasticDto>> bestPlacesByPrice;

        public ShoppingListService(IShoppingListRepository shoppingListRepository, IElasticClient elasticClient) 
        {
            this.shoppingListRepository = shoppingListRepository;
            this.elasticClient = elasticClient;
        }

        public async Task<IEnumerable<ShoppingListDto>> GetAllByUserAsync(Guid userID)
        {

            try
            {
                var shoppingLists = await shoppingListRepository.GetAllByUserAsync(userID);

                if (shoppingLists == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuario.");

                return shoppingLists.Select(shoppingList => shoppingList.AsDto()); ;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ShoppingListDto> GetByIDAsync(Guid userId)
        {
            try
            {
                var shoppingList = await shoppingListRepository.GetByIDAsync(userId);

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
                var existingShoppingLists = await shoppingListRepository.GetAllByUserAsync(createShoppingListDto.UserID);

                if(existingShoppingLists != null && existingShoppingLists.Count() == 5)
                    throw new Exception("Usuário já tem número máximo de listas cadastradas.");

                var shoppingList = new Entities.ShoppingList
                {
                    UserID = createShoppingListDto.UserID,
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
        
        public async Task UpdateAsync(Guid listID, UpdateShoppingListDto updateShoppingListDto)
        {
            try
            {
                var userShoppingLists = await shoppingListRepository.GetAllByUserAsync(updateShoppingListDto.UserID);

                if (userShoppingLists == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuário.");

                var existingShoppingList = userShoppingLists.FirstOrDefault(shoppingList => shoppingList.Id ==  listID);

                if (existingShoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras com esse ID.");

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

                existingShoppingList.Products.Remove( existingShoppingList.Products.Single(product => product.Gtin == gtinToRemove));

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
            IEnumerable<IGrouping<long, ElasticDto>> GroupedProducts;
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
                GroupedProducts = elasticProductsDto.GroupBy(product => product.Estabelecimento.Cnpj);

                // Verificar qual grupo tem mais elementos // Ordena pelo Count
                orderedGroups = GroupedProducts.OrderByDescending(group => group.Count());

                highScore = orderedGroups.Max(group => group.Count());

                // Pega apenas grupos com a maior quantidade de elementos
                GroupedProducts = orderedGroups.TakeWhile(group => group.Count() == highScore);

                // Ordena por menor valor e por menor distância
                orderedGroups = GroupedProducts.OrderBy(group => group.Sum(product => product.VlrItem)).ThenByDescending(g => g.FirstOrDefault().Estabelecimento.MaxDistance);

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
                    Name = productInfo.Estabelecimento.Name,
                    Cnpj = productInfo.Estabelecimento.Cnpj.ToString(),
                    Distance = productInfo.Estabelecimento.MaxDistance,
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
                   .Size(200)
                   .Index("sefaz_mprs_item")
                   .Query(q => q
                        .Bool(b => b
                        .Must(query)
                        .Filter(fi => fi
                                .GeoDistance(geo => geo
                                    .Distance(new Distance(pesquisaDto.MaxDistance, DistanceUnit.Kilometers))
                                    .Location(new GeoLocation(pesquisaDto.Latitude, pesquisaDto.Longitude))
                                    .Field(fs => fs.Estabelecimento.Location)
                                    ))))
                  .Sort(y => y
                    .Descending(SortSpecialField.Score)
                  ));

            //Query de consulta no Elastic Search
            ISearchResponse<ElasticDto> SearchResponse = await elasticClient.SearchAsync<ElasticDto>(request);

            // Calcula a distância para cada um dos produtos retornados
            foreach (var item in SearchResponse.Documents)
            {
                item.Estabelecimento.MaxDistance = GeoCalculator
                    .CalculateDistance(item.Estabelecimento.Latitude, item.Estabelecimento.Longitude,
                                      pesquisaDto.Latitude, pesquisaDto.Longitude, 'K');

                if (item.CodGrupoAnp.HasValue && item.CodGrupoAnp.Value > 0)
                {
                    item.VlrItem = item.VlrItem + item.VlrDescItem;
                    item.VlrDescItem = 0;
                }
            }

            // Prepara as lambdas
            Func<ElasticDto, Object> campoOrdemPrincipal = null;
            Func<ElasticDto, Object> campoOrdemComplementar = null;

            campoOrdemPrincipal = f => f.VlrItem;
            campoOrdemComplementar = f => f.Estabelecimento.MaxDistance;


            IEnumerable<ElasticDto> itens = null;

            // Ordena todos produtos retornados por Preço e Distância
            itens = SearchResponse.Documents.OrderByDescending(campoOrdemPrincipal).ThenBy(campoOrdemComplementar);

            //Remove os duplicados
            return itens.GroupBy(item => new { item.TexDesc, item.Estabelecimento.Cnpj }).Select(item => item.First()).Take(200);
        }
    }

}

