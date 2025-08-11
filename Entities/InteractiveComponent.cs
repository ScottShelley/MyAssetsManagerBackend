using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MyAssetsManagerBackend.enums;

namespace MyAssetsManagerBackend.Entities;

public class InteractiveComponent : IIdentifiable<ObjectId>
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        [BsonIgnoreIfNull]
        public InteractiveComponent Parent { get; set; }

        [BsonIgnoreIfNull]
        public HashSet<InteractiveComponent> Children { get; set; } = new HashSet<InteractiveComponent>();

        [BsonElement("Javascript")]
        public string Javascript { get; set; }

        public string Name { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? TimeStart { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? TimeStop { get; set; }

        [BsonRepresentation(BsonType.String)]
        public InteractiveComponentType? Type { get; set; }

        [BsonElement("ComponentOrder")]
        public int ComponentOrder { get; set; } = 0;

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, object> Data { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, object> Metadata { get; set; }

        [BsonIgnore]
        public InteractiveComponentSource InteractiveComponentSource { get; set; }

        // [BsonIgnore]
        // public ObjectId? InteractiveComponentSourceId
        // {
        //     get => InteractiveComponentSource != null ? InteractiveComponentSource.Id : (ObjectId?)null;
        // }

        [BsonElement("Changes")]
        public List<string> Changes { get; set; } = new List<string>();

        [BsonIgnore]
        public MediaSource MediaSource { get; set; }

        // Lifecycle methods, call from your app/service
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

        public int GetOrder() => ComponentOrder;
        public void SetOrder(int order) => ComponentOrder = order;

        public override string ToString()
        {
            return $"id={Id}, name={Name}, timeStart={TimeStart}, timeStop={TimeStop}";
        }
    }

    public interface IIdentifiable<T>
    {
        T Id { get; }
    }
