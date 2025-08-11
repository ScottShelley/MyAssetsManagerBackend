using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MyAssetsManagerBackend.enums;

namespace MyAssetsManagerBackend.Entities;

public class MediaSource
{
    [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonRepresentation(BsonType.Double)]
        public double? Duration { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("BusinessRule")]
        public string BusinessRule { get; set; }

        [BsonElement("RuntimeBusinessRule")]
        public string RuntimeBusinessRule { get; set; }

        [BsonElement("Url")]
        public string Url { get; set; }

        public string ContentType { get; set; }

        public string PosterUrl { get; set; }

        public int Sequence { get; set; } = 0;

        public string Configuration { get; set; }

        [BsonRepresentation(BsonType.String)]
        public SegmentType? SegmentType { get; set; }

        // Navigation properties
        // MongoDB doesn't support lazy loading natively; loading strategy depends on your application

        public List<InteractiveComponent> InteractiveComponents { get; set; } = new();

        public ObjectId? VideoSequenceId { get; set; }
        public VideoSequence VideoSequence { get; set; }

        public ObjectId? SourceFileId { get; set; }
        public SourceFile SourceFile { get; set; }

        [BsonIgnore]
        public decimal? Fps
        {
            get => SourceFile?.Metadata?.Fps;
        }

        [BsonIgnore]
        public int? IcCount { get; set; }

        public string TtsConfig { get; set; }

        public string Uuid { get; set; }

        // // Helper properties for asset info
        // [BsonIgnore]
        // public ObjectId? AssetId => SourceFile?.Id;

        [BsonIgnore]
        public Dimensions AssetDimensions => SourceFile?.Metadata?.Dimensions;

        [BsonIgnore]
        public string AssetThumbnailUrl => SourceFile?.Thumbnail?.ThumbnailUrls?.Medium;

        public override string ToString()
        {
            return $"id={Id}, type={SegmentType}, sf.id={SourceFile?.Id}";
        }

        // MongoDB doesn't have JPA lifecycle callbacks; you handle this manually
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

        // Ignored setter for Url, per your Java code
        public void SetUrl(string url)
        {
            // Do nothing
        }
}