using Microsoft.AspNetCore.Mvc;
using Procergs.ShoppingList.Service.Dtos;
using Procergs.ShoppingList.Service.Interfaces;

namespace Procergs.ShoppingList.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IShoppingListService shoppingListService;
        private readonly Guid userID = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        public ProductService(IShoppingListService shoppingListService)
        {
            this.shoppingListService = shoppingListService;
            
        }
 
        public async Task<ProductDto> GetByGtinAsync(string gtin)
        {
            try
            {
                var shoppingList = await shoppingListService.GetByUserIDAsync(this.userID);

                return shoppingList.ProductDtos.Find(product => product.Gtin == gtin);

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
                ProductDto existingProduct = await GetByGtinAsync(addProductDto.Gtin);

                if (existingProduct != null)
                    throw new NullReferenceException("Produto já está na lista de compras!");

                ProductDto newProduct = new ProductDto(addProductDto.Gtin, addProductDto.Name, 0);

                await shoppingListService.AddProductAsync(userID, newProduct);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(string gtinToRemove)
        {
            try
            {
                ProductDto existingProduct = await GetByGtinAsync(gtinToRemove);

                if (existingProduct == null)
                    throw new NullReferenceException("Produto não existe na lista de compras!");

                await shoppingListService.RemoveProductAsync(userID, gtinToRemove); 
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
