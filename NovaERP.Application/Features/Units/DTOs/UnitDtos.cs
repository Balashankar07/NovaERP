namespace NovaERP.Application.Features.Units.DTOs;

public class UnitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUnitDto
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateUnitDto
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
