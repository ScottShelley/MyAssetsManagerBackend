using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MyAssetsManagerBackend.enums;

namespace MyAssetsManagerBackend.Entities;

public class SourceFile
{
    [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("deleted")]
        public bool Deleted { get; set; } = false;

        [BsonElement("arbitraryOrder")]
        public int ArbitraryOrder { get; set; } = 2;

        [BsonElement("updatedBy")]
        public string UpdatedBy { get; set; }

        [BsonElement("mediaSources")]
        public List<MediaSource> MediaSources { get; set; } = new List<MediaSource>();

        [BsonElement("isPublic")]
        public bool IsPublic { get; set; } = false;

        [BsonElement("jsonMetadata")]
        public Dictionary<string, object> JsonMetadata { get; set; } = new Dictionary<string, object>();

        [BsonElement("metadata")]
        public SourceFileMetadata Metadata { get; set; }

        [BsonElement("payload")]
        public byte[] Payload { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonIgnore] // not persisted, transient
        public string UrlSigned { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("originalFilename")]
        public string OriginalFilename { get; set; }

        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("tags")]
        public string Tags { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("sourceFileType")]
        public SourceFileType SourceFileType { get; set; }

        [BsonElement("parent")]
        public string ParentId { get; set; } // Reference to parent SourceFile Id

        [BsonElement("children")]
        public HashSet<string> ChildrenIds { get; set; } = new HashSet<string>(); // store child Ids

        [BsonElement("thumbnail")]
        public Thumbnail Thumbnail { get; set; }

        [BsonElement("uuid")]
        public string UUID { get; set; }

        public int GetOwnUsedCount()
        {
            return MediaSources?.Count ?? 0;
        }

        public int GetAggregatedUsedCount()
        {
            // If you store children as embedded docs, you could sum their counts here
            return GetOwnUsedCount();
        }
}