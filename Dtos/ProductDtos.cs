namespace Procergs.ShoppingList.Service.Dtos
{
    public record ProductDto(string Gtin, string Name, decimal Price);

    public record AddProductDto(string Gtin, string Name);

    public record SearchProductDto(string[] Gtins);

}
