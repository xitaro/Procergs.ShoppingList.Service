using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Procergs.ShoppingList.Service.Dtos;
using Procergs.ShoppingList.Service.Interfaces;

namespace Procergs.ShoppingList.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IValidator<IProductDto> validator;

        public ProductController(IProductService productService, IValidator<IProductDto> validator)
        {
            this.productService = productService;
            this.validator = validator;
        }

        // POST: ProductController/AddAsync
        [HttpPost]
        public async Task<ActionResult<ProductDto>> AddAsync(AddProductDto addProductDto)
        {
            ValidationResult validationResult = await validator.ValidateAsync(addProductDto);

            if (!validationResult.IsValid)
            {
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                return BadRequest(problemDetails);
            }
            
            await this.productService.AddAsync(addProductDto);

            return NoContent();
        }
     
        // POST: ProductController/DeleteAsync
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(RemoveProductDto removeProductDto)
        {
            await this.productService.DeleteAsync(removeProductDto);

            return NoContent();
        }
    }
}
