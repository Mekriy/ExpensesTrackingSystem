namespace EST.Domain.DTOs;

public class UsersCreatedInfoDTO
{
    public List<ItemDTO> Items { get; set; }
    public List<CategoryDTO> Categories { get; set; }
    public List<LocationDTO> Locations { get; set; }
}