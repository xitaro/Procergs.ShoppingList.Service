using Microsoft.AspNetCore.Http.HttpResults;
using Procergs.ShoppingList.Service.Dtos;
using Procergs.ShoppingList.Service.Extensions;
using Procergs.ShoppingList.Service.Interfaces;
using Procergs.ShoppingList.Service.Repositories;

namespace Procergs.ShoppingList.Service.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListRepository _shoppingListRepository;

        public ShoppingListService(IShoppingListRepository shoppingListRepository) 
        {
            _shoppingListRepository = shoppingListRepository;
        }

        public async Task<ShoppingListDto> GetByUserIDAsync(Guid userId)
        {
            try
            {
                var shoppingList = await _shoppingListRepository.GetByUserIDAsync(userId);

                if (shoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuário.");

                return shoppingList.AsDto();
            }
            catch (Exception)
            {
                throw;
            } 
        }

        public async Task<ShoppingListDto> CreateAsync(CreateShoppingListDto createShoppingListDto)
        {
            try
            {
                var existingShoppingList = await _shoppingListRepository.GetByUserIDAsync(createShoppingListDto.userID);

                if (existingShoppingList != null)
                    throw new Exception("Já existe uma lista cadastrada para este usuário.");

                var shoppingList = new Entities.ShoppingList
                {
                    UserID = createShoppingListDto.userID,
                    Name = createShoppingListDto.Name,
                    Products = new List<ProductDto>()
                };

                await _shoppingListRepository.CreateAsync(shoppingList);

                return await GetByUserIDAsync(shoppingList.UserID);

            }
            catch (Exception)
            {
                throw;
            }            
        }
        
        public async Task UpdateAsync(UpdateShoppingListDto updateShoppingListDto)
        {
            try
            {
                var existingShoppingList = await _shoppingListRepository.GetByUserIDAsync(updateShoppingListDto.userID);

                if (existingShoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuário.");

                existingShoppingList.Name = updateShoppingListDto.Name;

                await _shoppingListRepository.UpdateAsync(existingShoppingList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddProductAsync(Guid userID, ProductDto productDto)
        {
            try
            {
                var existingShoppingList = await _shoppingListRepository.GetByUserIDAsync(userID);

                if (existingShoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuário.");

                existingShoppingList.Products.Add(productDto);

                await _shoppingListRepository.UpdateAsync(existingShoppingList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveProductAsync(Guid userID, string gtinToRemove)
        {
            try
            {
                var existingShoppingList = await _shoppingListRepository.GetByUserIDAsync(userID);

                if (existingShoppingList == null)
                    throw new NullReferenceException("Não encontrou nenhuma lista de compras para este usuário.");

                existingShoppingList.Products.Remove( existingShoppingList.Products.Single(product => product.Gtin == gtinToRemove));

                await _shoppingListRepository.UpdateAsync(existingShoppingList);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
