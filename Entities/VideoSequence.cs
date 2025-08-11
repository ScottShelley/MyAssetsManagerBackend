using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyAssetsManagerBackend.Entities;

public class VideoSequence
    // : IIdentifiable<ObjectId>, IMagnetLink
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("videoProjectId")]
    public ObjectId? VideoProjectId { get; set; }

    [BsonIgnore]
    public VideoProject VideoProject { get; set; }

    [BsonElement("mediaSources")]
    public List<MediaSource> MediaSources { get; set; } = new List<MediaSource>();

    [BsonElement("sequence")]
    public int Sequence { get; set; }

    [BsonElement("autoAdvance")]
    public bool AutoAdvance { get; set; } = false;

    [BsonElement("uuid")]
    public string UUID { get; set; }

    public void AddMediaSource(MediaSource msNew)
    {
        if (!MediaSources.Contains(msNew))
        {
            MediaSources.Add(msNew);
            msNew.VideoSequenceId = Id; // Assuming MediaSource tracks parent ID
        }
    }

    public override string ToString()
    {
        return $"id={Id}, seq={Sequence}, autoAdvance={AutoAdvance}, # of mediaSources: {MediaSources?.Count}";
    }
}
