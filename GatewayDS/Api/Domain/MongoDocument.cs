using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Domain;

public abstract class MongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime LastChangedAt { get; set; } = DateTime.UtcNow;
}