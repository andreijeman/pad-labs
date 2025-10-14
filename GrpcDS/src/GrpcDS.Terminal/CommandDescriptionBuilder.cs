namespace GrpcDS.Terminal;

public class CommandDescriptionBuilder
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Signature { get; init; }
    

    public string Build() => 
    $"""
    Command: 
    | Name: {Name}
    | Description: {Description}
    | Signature: {Signature}
    """;
}