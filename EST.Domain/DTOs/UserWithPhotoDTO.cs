namespace EST.Domain.DTOs;

public class UserWithPhotoDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FileName { get; set; }
    public string RoleName { get; set; }
}