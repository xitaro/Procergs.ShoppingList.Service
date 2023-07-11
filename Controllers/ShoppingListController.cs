using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Procergs.ShoppingList.Service.Dtos;
using Procergs.ShoppingList.Service.Interfaces;

namespace Procergs.ShoppingList.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingListController : ControllerBase
    {
        private readonly IShoppingListService shoppingListService;
        private readonly IValidator<SearchDto> searchValidator;
        private readonly IValidator<IShoppinListDto> shoppingListValidator;

        public ShoppingListController(IShoppingListService shoppingListService,
            IValidator<SearchDto> searchValidator,
            IValidator<IShoppinListDto> shoppingListValidator)
        {
            this.shoppingListService = shoppingListService;
            this.searchValidator = searchValidator;
            this.shoppingListValidator = shoppingListValidator;
        }

        [HttpGet("AllByUser/{userCpf}")]
        public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetAllByUserAsync(string userCpf)
        {
            var shoppingListsDto = await shoppingListService.GetAllByUserAsync(userCpf);

            return Ok(shoppingListsDto);
        }

        [HttpGet("{listID}")]
        public async Task<ActionResult<ShoppingListDto>> GetByIDAsync(Guid listID)
        {
            var shoppingListDto = await shoppingListService.GetByIDAsync(listID);

            return Ok(shoppingListDto);
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingListDto>> CreateAsync(CreateShoppingListDto createShoppingListDto)
        {
            ValidationResult validationResult = await shoppingListValidator.ValidateAsync(createShoppingListDto);

            if (!validationResult.IsValid)
            {
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                return BadRequest(problemDetails);
            }

            var createdShoppingListDto = await shoppingListService.CreateAsync(createShoppingListDto);

            return CreatedAtAction(nameof(GetByIDAsync), new { listID = createdShoppingListDto.Id}, createdShoppingListDto);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync(UpdateShoppingListDto updateShoppingListDto)
        {
            ValidationResult validationResult = await shoppingListValidator.ValidateAsync(updateShoppingListDto);

            if (!validationResult.IsValid)
            {
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                return BadRequest(problemDetails);
            }

            await shoppingListService.UpdateAsync(updateShoppingListDto);

            return NoContent();
        }

        [HttpPost("/elastic")]
        public async Task<ActionResult<BestPlaceDto>> FindBestBuyPlace(SearchDto pesquisaDto)
        {
            ValidationResult validationResult = await searchValidator.ValidateAsync(pesquisaDto);

            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                return BadRequest(problemDetails);
            }

            var bestPlaceDto = await shoppingListService.FindBestBuyPlace(pesquisaDto);
           
            return Ok(bestPlaceDto);
        }
    }
}
