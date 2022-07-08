using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuoteAPI.Models
{
    public class Quote
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("Text")]
        public string? QuoteText { get; set; }
        [BsonElement("Source")]
        public string? QuoteSource { get; set; }
        [BsonElement("Tags")]
        public List<string>? Tags { get; set; }
        [BsonElement("Modified")]
        public DateTime? LastModified { get; set; }
        [BsonElement("Views")]
        public int? Views { get; set; }
    }
}
