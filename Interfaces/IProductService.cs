using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Interfaces
{
    public interface IProductService
    {
        public Task<ProductDto> GetByGtinAsync(string gtin);
        public Task AddAsync(AddProductDto addProductDto);
        public Task DeleteAsync(string gtin);

    }
}