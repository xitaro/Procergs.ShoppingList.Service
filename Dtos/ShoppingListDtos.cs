using FluentValidation;
using Procergs.ShoppingList.Service.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Procergs.ShoppingList.Service.Dtos
{
    public record ShoppingListDto( Guid Id, Guid UserID, string Name, List<ProductDto> ProductDtos) : IShoppinListDto;

    public record CreateShoppingListDto(Guid UserID, string Name) : IShoppinListDto;

    public record UpdateShoppingListDto(Guid UserID, string Name) : IShoppinListDto;

    public class ShoppingListDtoValidator : AbstractValidator<IShoppinListDto>
    {
        public ShoppingListDtoValidator()
        {
            RuleFor(x => x.UserID).NotNull().NotEmpty();
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(50);
        }
    }

    public record BestPlaceDto
    {
        public string Name { get; init; }
        public string Cnpj { get; init; }
        public double Distance { get; init; }
        public List<BestPlaceProductDto> ProductDtos { get; init; }
        public decimal Amount { get; init; }
    }
}

