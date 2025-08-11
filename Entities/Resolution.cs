using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyAssetsManagerBackend.Entities;

public class Resolution
{
    public Resolution()
    {
    }

    public Resolution(double horizontal, double vertical)
    {
        Horizontal = Math.Round((decimal)horizontal, 2, MidpointRounding.AwayFromZero);
        Vertical = Math.Round((decimal)vertical, 2, MidpointRounding.AwayFromZero);
    }

    public Resolution(decimal horizontal, decimal vertical)
    {
        Horizontal = decimal.Round(horizontal, 2, MidpointRounding.AwayFromZero);
        Vertical = decimal.Round(vertical, 2, MidpointRounding.AwayFromZero);
    }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Horizontal { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Vertical { get; set; }

}