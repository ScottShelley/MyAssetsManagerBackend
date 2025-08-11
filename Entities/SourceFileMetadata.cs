using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyAssetsManagerBackend.Entities;

public class SourceFileMetadata
{
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Duration { get; set; }

    public Dimensions Dimensions { get; set; } = new Dimensions();

    public Resolution Resolution { get; set; } = new Resolution();

    [BsonElement("Thumbnails")]
    public List<string> Thumbnails { get; set; } = new List<string>();

    [BsonElement("Frames")]
    public List<string> Frames { get; set; } = new List<string>();

    public int SelectedThumbnail { get; set; }

    public int SelectedFrame { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Fps { get; set; }

    // To be removed after migration
    public string ThumbnailUrl { get; set; }

    public int FrameCount { get; set; }

    public int Depth { get; set; }

    public string Type { get; set; }

    public long Size { get; set; }

    public string OriginalUrl { get; set; }

    public int Bitrate { get; set; }

    public string Subcategory { get; set; }

    public string HighlightColour { get; set; }
}