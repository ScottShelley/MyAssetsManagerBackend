using MongoDB.Bson.Serialization.Attributes;

namespace MyAssetsManagerBackend.data.detail;

public class ThumbnailUrls
{
    [BsonElement("Small")]
    public string Small { get; set; }

    [BsonElement("Medium")]
    public string Medium { get; set; }

    [BsonIgnore]
    public string SmallSigned { get; set; }

    [BsonIgnore]
    public string MediumSigned { get; set; }
}