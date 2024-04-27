namespace EST.Domain.DTOs;

public class CreatedLocationDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; }
    public bool Save { get; set; }
}