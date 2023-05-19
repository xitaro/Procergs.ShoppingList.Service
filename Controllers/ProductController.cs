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

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        // POST: ProductController/CreateAsync
        [HttpPost]
        public async Task<ActionResult<ProductDto>> AddAsync(AddProductDto addProductDto)
        {
            await this.productService.AddAsync(addProductDto);

            return NoContent();
        }
     
        // POST: ProductController/DeleteAsync/5
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(string gtin)
        {
            await this.productService.DeleteAsync(gtin);

            return NoContent();
        }
    }
}
