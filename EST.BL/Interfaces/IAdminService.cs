using EST.Domain.DTOs;
using EST.Domain.Pagination;

namespace EST.BL.Interfaces;

public interface IAdminService
{
    Task<PagedResponse<List<ItemDTO>>> GetItemsForAdminToReview(PaginationFilter filter, string userId, CancellationToken token);
    Task<List<TodaysExpensesByCategoriesDTO>> GetStatistic(CancellationToken token);
    Task<GeneralInfoOfTodayDTO> GetGeneralInfo(CancellationToken token);

    Task<bool> ChangeItemsVisibility(ItemDTO itemDto, CancellationToken token);
    Task<List<UserWithPhotoDTO>> GetUsersByQuery(string user, CancellationToken token);
    Task<UsersCreatedInfoDTO> GetUsersCreatedInfo(Guid userId, CancellationToken token);
}