using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MyAssetsManagerBackend.enums;

namespace MyAssetsManagerBackend.Entities;

public class InteractiveComponentSource
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string ParentId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public EntityStatus WorkflowState { get; set; } = EntityStatus.DRAFT;

    public int WorkflowVersion { get; set; } = 1;

    public bool Deleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string Overlay { get; set; } = "overlay";

    [BsonRepresentation(BsonType.String)]
    public CSSPREPROCESSOR CssPreprocessor { get; set; } = CSSPREPROCESSOR.NONE;

    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

    public Dictionary<string, object> Resources { get; set; } = new Dictionary<string, object>();

    public string Name { get; set; }

    public string ModifiedBy { get; set; }

    public bool Iframe { get; set; } = false;

    public string Html { get; set; }

    public string Javascript { get; set; }

    public string Css { get; set; }

    public string Comments { get; set; }

    public string UUID { get; set; } = Guid.NewGuid().ToString();

    // Optional: If you want to store embedded parent objects
    [BsonIgnore]
    public InteractiveComponentSource Parent { get; set; }
}