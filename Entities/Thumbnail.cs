using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MyAssetsManagerBackend.data.detail;

namespace MyAssetsManagerBackend.Entities;

public class Thumbnail
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }

    public ThumbnailUrls ThumbnailUrls { get; set; }

    public Thumbnail()
    {
    }

    // Manually manage timestamps like in JPA lifecycle callbacks
    public void OnCreate()
    {
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void OnUpdate()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}