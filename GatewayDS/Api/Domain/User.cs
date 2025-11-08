namespace Api.Domain;

public class User : MongoDocument
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required List<string> Hobbies { get; set; }
}