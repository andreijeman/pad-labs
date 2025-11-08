namespace Api.Options;

public class MongoDbSettings
{
    public required string DefaultConnection { get; set; } 
    public required string DatabaseName { get; set; } 
}