using EST.Domain.DTOs;

namespace EST.BL.Interfaces;

public interface ILocationService
{
    Task<List<LocationDTO>> GetUserSavedLocation(Guid userId);
    Task<LocationDTO> Create(LocationDTO locationDto, Guid userId);
    Task<bool> AddLocationToExpense(Guid expenseId, Guid locationId);
    Task<LocationDTO> Update(LocationDTO locationDto, Guid userId);
    Task<bool> Delete(Guid locationId);
}