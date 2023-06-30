namespace Procergs.ShoppingList.Service.Interfaces
{
    public interface IProductDto
    {
        public Guid ListID { get; }
        public string Gtin  { get; }
    }
}
