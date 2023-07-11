using Procergs.ShoppingList.Service.Dtos;
using Procergs.ShoppingList.Service.Interfaces;
using System.Collections.Generic;

namespace Procergs.ShoppingList.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IShoppingListService shoppingListService;

        public ProductService(IShoppingListService shoppingListService)
        {
            this.shoppingListService = shoppingListService;
        }
 
        public async Task<List<ProductDto>> GetProductsFromListAsync(Guid listID, string gtin)
        {
            try
            {
                var shoppingList = await shoppingListService.GetByIDAsync(listID);

                if (shoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras com esse ID");

                var existingProducts = shoppingList.ProductDtos;

                return existingProducts;

            }
            catch (Exception)
            {
                throw;
            }  
        }

        public async Task AddAsync(AddProductDto addProductDto)
        {
            try
            {
                List<ProductDto> existingProducts = await GetProductsFromListAsync(addProductDto.ListID, addProductDto.Gtin);

                if (existingProducts != null && existingProducts.Count() == 30)
                    throw new NullReferenceException("Lista de Compras está cheia!");

                var existingProduct = existingProducts.FirstOrDefault(product => product.Gtin == addProductDto.Gtin);

                if (existingProduct != null)
                    throw new Exception("Esse produto já está cadastrado nessa lista!");

                ProductDto newProduct = new ProductDto(addProductDto.Gtin, addProductDto.Name, 0);

                await shoppingListService.AddProductAsync(addProductDto.ListID, newProduct);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(RemoveProductDto removeProductDto)
        {
            try
            {
                List<ProductDto> existingProducts = await GetProductsFromListAsync(removeProductDto.ListID, removeProductDto.Gtin);

                if (existingProducts == null)
                    throw new NullReferenceException("Esta lista de compras não existe!");

                await shoppingListService.RemoveProductAsync(removeProductDto.ListID, removeProductDto.Gtin); 
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
