using Nest;
using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace Procergs.ShoppingList.Service.Dtos
{

    // Esse record está em portugues pois representa o objeto da api do elastic search

    [ElasticsearchType(RelationName = "estabelecimento")]
    public record EstabelecimentoDto
    {
        [JsonPropertyName("codCnpjEstab")]
        public long CodCnpjEstab { get; set; }

        [JsonPropertyName("nomeContrib")]
        public string NomeContrib { get; set; }

        [JsonPropertyName("nomeLograd")]
        public string NomeLograd { get; set; }

        [JsonPropertyName("nroLograd")]
        public string NroLograd { get; set; }

        [JsonPropertyName("nomeBairro")]
        public string NomeBairro { get; set; }

        [JsonPropertyName("nomeMunic")]
        public string NomeMunic { get; set; }

        [JsonPropertyName("nroCEP")]
        public long NroCEP { get; set; }

        [JsonPropertyName("nroLatitude")]
        public double NroLatitude { get; set; }

        [JsonPropertyName("nroLongitude")]
        public double NroLongitude { get; set; }

        [JsonPropertyName("localizacao")]
        [GeoPoint]
        public GeoLocation Localizacao { get; set; }

        [JsonPropertyName("kmDistancia")]
        public double KmDistancia { get; set; }
    }
}