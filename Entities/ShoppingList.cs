using Procergs.ShoppingList.Service.Dtos;

namespace Procergs.ShoppingList.Service.Entities
{
    public class ShoppingList
    {
        public Guid Id { get; set; }
        public string UserCpf { get; set; }
        public string Name { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
