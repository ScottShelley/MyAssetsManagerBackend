using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyAssetsManagerBackend.Entities;

public class VideoProject
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [BsonElement("name")]
    public DateTime Name { get; set; } = DateTime.UtcNow;
}