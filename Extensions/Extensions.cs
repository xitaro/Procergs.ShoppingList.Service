using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Extensions
{
    public static class Extensions
    {
        public static ShoppingListDto AsDto(this Entities.ShoppingList shoppingList)
        {
            return new ShoppingListDto(shoppingList.Id, shoppingList.UserCpf, shoppingList.Name, shoppingList.Products);
        }

        public static BestPlaceProductDto AsDto(this ElasticDto elasticProductDto)
        {
            return new BestPlaceProductDto(elasticProductDto.TexDesc, elasticProductDto.VlrItem);
        }
    }
}
