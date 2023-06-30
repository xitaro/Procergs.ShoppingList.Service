using Nest;
using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace Procergs.ShoppingList.Service.Dtos
{
    [ElasticsearchType(RelationName = "estabelecimento")]
    public record EstabelecimentoDto
    {
        [JsonPropertyName("codCnpjEstab")]
        public long Cnpj { get; set; }

        [JsonPropertyName("nomeContrib")]
        public string Name { get; set; }

        [JsonPropertyName("nomeLograd")]
        public string Adress { get; set; }

        [JsonPropertyName("nroLograd")]
        public string AdressNumber { get; set; }

        [JsonPropertyName("nomeBairro")]
        public string District { get; set; }

        [JsonPropertyName("nomeMunic")]
        public string City { get; set; }

        [JsonPropertyName("nroCEP")]
        public long Cep { get; set; }

        [JsonPropertyName("nroLatitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("nroLongitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("localizacao")]
        [GeoPoint]
        public GeoLocation Location { get; set; }

        [JsonPropertyName("kmDistancia")]
        public double MaxDistance { get; set; }
    }
}