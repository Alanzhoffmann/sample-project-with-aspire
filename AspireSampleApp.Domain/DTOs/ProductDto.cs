namespace AspireSampleApp.Domain.DTOs;

public record ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool HasThirdPartyData { get; set; }
}
