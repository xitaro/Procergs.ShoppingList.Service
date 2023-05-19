using System.ComponentModel.DataAnnotations;

namespace Procergs.ShoppingList.Service.Dtos
{
    public record ShoppingListDto(Guid UserID, string Name, List<ProductDto> ProductDtos);

    public record CreateShoppingListDto([Required] Guid userID, [Required] string Name);

    public record UpdateShoppingListDto([Required] Guid userID, [Required] string Name);
}

