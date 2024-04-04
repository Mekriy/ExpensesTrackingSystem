using EST.Domain.DTOs;

namespace EST.BL.Interfaces
{
    public interface IReviewService
    {
        Task<double> GetItemsRating(Guid itemId, CancellationToken token);
        Task<ReviewDTO> GetReveiwDTO(Guid id, CancellationToken token);
        Task<bool> Create(Guid itemId, Guid userId, ReviewDTO reviewDTO);
        Task<bool> Delete(Guid reviewId);
        Task<bool> Exist(Guid reviewId);
        Task<bool> SaveAsync();

    }
}
