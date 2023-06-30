using FluentValidation;
using Nest;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Procergs.ShoppingList.Service.Dtos
{
    [ElasticsearchType(RelationName = "item")]
    public record ElasticDto
    {
        [property: JsonPropertyName("codIntItem")] 
        public long CodIntItem { get; init; }

        [property: JsonPropertyName("gtin")]
        public long Gtin { get; init; }

        [property: JsonPropertyName("texDesc")]
        public string TexDesc { get; init; }

        [property: JsonPropertyName("vlrItem")]
        public decimal VlrItem { get; set; }

        [property: JsonPropertyName("vlrDescItem")] 
        public decimal VlrDescItem { get; set; }

        [property: JsonPropertyName("tipoVenda")] 
        public byte TipoVenda { get; init; }

        [property: JsonPropertyName("codGrupoAnp")] 
        public byte? CodGrupoAnp { get; init; }

        [property: JsonPropertyName("dthEmiNFe")] 
        public DateTime? DthEmiNFe { get; init; }

        [property: JsonPropertyName("estabelecimento")] 
        public EstabelecimentoDto Estabelecimento { get; init; }
    }

    public record SearchDto(Guid ListID, double Latitude, double Longitude, double MaxDistance, byte DayLimit);

    public class SearchDtoValidator : AbstractValidator<SearchDto>
    {
        public SearchDtoValidator()
        {
            RuleFor(x => x.ListID).NotNull().NotEmpty();
            RuleFor(x => x.Latitude).NotNull();
            RuleFor(x => x.Longitude).NotNull();
            RuleFor(x => x.MaxDistance).NotNull().InclusiveBetween(0.1, 30);
            RuleFor(x => (int)x.DayLimit).NotNull().InclusiveBetween(1, 90);
        }
    }
}