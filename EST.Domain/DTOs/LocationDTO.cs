namespace EST.Domain.DTOs;

public class LocationDTO
{
    public string Name { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string Address { get; set; }
    public bool Save { get; set; }
}