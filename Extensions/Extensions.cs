using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Extensions
{
    public static class Extensions
    {
        public static ShoppingListDto AsDto(this Entities.ShoppingList shoppingList)
        {
            return new ShoppingListDto(shoppingList.UserID, shoppingList.Name, shoppingList.Products);
        }
    }
}
