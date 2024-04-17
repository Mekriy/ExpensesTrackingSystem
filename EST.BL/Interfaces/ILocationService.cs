using EST.Domain.DTOs;

namespace EST.BL.Interfaces;

public interface ILocationService
{
    Task<List<DropdownLocationDTO>> GetUserSavedLocation(Guid userId);
    Task<CreatedLocationDTO> Create(LocationDTO locationDto, Guid userId);
    Task<bool> AddLocationToExpense(AddLocationToExpenseDTO location);
    Task<LocationDTO> Update(LocationDTO locationDto, Guid userId);
    Task<bool> Delete(Guid locationId);
    Task<LocationDTO> UpdateLocationExpense(UpdateLocationExpenseDTO updateDto, Guid userParseId);
}