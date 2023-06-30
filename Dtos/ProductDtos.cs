using FluentValidation;
using Procergs.ShoppingList.Service.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace Procergs.ShoppingList.Service.Dtos
{
    public record ProductDto(string Gtin, string Name, decimal Price);

    public record AddProductDto(Guid ListID, string Gtin, string Name) : IProductDto;

    public record RemoveProductDto(Guid ListID, string Gtin) : IProductDto;

    public record BestPlaceProductDto(string Name, decimal Price);
    
    public class IProductDtoValidator : AbstractValidator<IProductDto>
    {
        public IProductDtoValidator()
        {
            RuleFor(x => x).SetInheritanceValidator(v =>
            {
                v.Add(new AddProductValidator());
                v.Add(new RemoveProductValidator());
            });
        }
    }

    public class AddProductValidator : AbstractValidator<AddProductDto>
    {
        public AddProductValidator()
        {
            RuleFor(x => x.ListID).NotNull().NotEmpty();
            RuleFor(x => x.Gtin).NotNull().NotEmpty().Length(13);
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(50);
        }
    }

    public class RemoveProductValidator : AbstractValidator<RemoveProductDto>
    {
        public RemoveProductValidator()
        {
            RuleFor(x => x.ListID).NotNull().NotEmpty();
            RuleFor(x => x.Gtin).NotNull().NotEmpty().Length(13);
        }
    }
}
