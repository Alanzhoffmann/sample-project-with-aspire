using System.ComponentModel.DataAnnotations;

namespace AspireSampleApp.Contracts.Commands;

public record CreateProductCommand([Required] [MaxLength(100)] string Name, [MaxLength(500)] string? Description);
