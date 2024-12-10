namespace Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentationAttribute(BsonType.ObjectId)]
    public ObjectId id { get; set; }
    public string? name { get; set; }
    
    public string? email { get; set; }

    public DateTime birthday { get; set; }

    public Boolean isAdmin { get; set; }
}