using MongoDB.Bson.Serialization.Attributes;

namespace MyAssetsManagerBackend.Entities;

public class Dimensions
{
    public Dimensions()
    {
    }

    public Dimensions(int width, int height)
    {
        Width = width;
        Height = height;
    }

    [BsonElement("Width")]
    public int Width { get; set; }

    [BsonElement("Height")]
    public int Height { get; set; }
}