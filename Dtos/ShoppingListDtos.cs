using FluentValidation;
using Procergs.ShoppingList.Service.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Procergs.ShoppingList.Service.Dtos
{
    public record ShoppingListDto( Guid Id, string UserCpf, string Name, List<ProductDto> ProductDtos);

    public record CreateShoppingListDto(string UserCpf, string Name) : IShoppinListDto;

    public record UpdateShoppingListDto(Guid Id, string Name) : IShoppinListDto;

    public class ShoppingListDtoValidator : AbstractValidator<IShoppinListDto>
    {
        public ShoppingListDtoValidator()
        {
            RuleFor(x => x).SetInheritanceValidator(v =>
            {
                v.Add(new CreateShoppingListDtoValidator());
                v.Add(new UpdateShoppingListDtoValidator());
            });
        }
    }

    public class CreateShoppingListDtoValidator : AbstractValidator<CreateShoppingListDto>
    {
        public CreateShoppingListDtoValidator()
        {
            RuleFor(x => x.UserCpf).NotNull().NotEmpty();
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(50);
        }
    }

    public class UpdateShoppingListDtoValidator : AbstractValidator<UpdateShoppingListDto>
    {
        public UpdateShoppingListDtoValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
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

