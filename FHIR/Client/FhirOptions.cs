namespace FHIR.Client;

public class FhirOptions
{
    public required string Url { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
}