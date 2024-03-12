namespace EST.DAL.Models;

public class PhotoFile : BaseEntity
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public User User { get; set; }
    public Guid UserId { get; set; }
}