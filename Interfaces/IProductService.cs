using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Interfaces
{
    public interface IProductService
    {
        public Task<List<ProductDto>> GetProductsFromListAsync(Guid listID, string gtin);
        public Task AddAsync(AddProductDto addProductDto);
        public Task DeleteAsync(RemoveProductDto removeProductDto);

    }
}