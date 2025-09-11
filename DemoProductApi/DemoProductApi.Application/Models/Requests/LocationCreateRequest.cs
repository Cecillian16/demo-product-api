namespace DemoProductApi.Application.Models.Requests;

public class LocationCreateRequest
{
    public string Name { get; set; } = default!;
    public string? Type { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}