namespace EST.Domain.DTOs
{
    public class UpdateCategoryDTO
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public Guid UserId { get; set; }
    }
}
