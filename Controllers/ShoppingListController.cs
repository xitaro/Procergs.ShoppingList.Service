
using Microsoft.AspNetCore.Mvc;
using Procergs.ShoppingList.Service.Dtos;
using Procergs.ShoppingList.Service.Interfaces;
using Procergs.ShoppingList.Service.Repositories;

namespace Procergs.ShoppingList.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingListController : ControllerBase
    {
        private readonly IShoppingListService _shoppingListService;

        public ShoppingListController(IShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }  

        [HttpGet("{userId}")]
        public async Task<ActionResult<ShoppingListDto>> GetByUserIDAsync(Guid userId)
        {
            var shoppingListDto = await _shoppingListService.GetByUserIDAsync(userId);

            return Ok(shoppingListDto);
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingListDto>> CreateAsync(CreateShoppingListDto createShoppingListDto)
        {
            var createdShoppingListDto = await _shoppingListService.CreateAsync(createShoppingListDto);

            return CreatedAtAction(nameof(GetByUserIDAsync), new {userID = createdShoppingListDto.UserID}, createdShoppingListDto);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync(UpdateShoppingListDto updateShoppingListDto)
        {
            await _shoppingListService.UpdateAsync(updateShoppingListDto);

            return NoContent();
        }
    }
}
