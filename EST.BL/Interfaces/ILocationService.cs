using EST.Domain.DTOs;

namespace EST.BL.Interfaces;

public interface ILocationService
{
    Task<List<DropdownLocationDTO>> GetUserSavedLocation(Guid userId, CancellationToken token);
    Task<CreatedLocationDTO> Create(LocationDTO locationDto, Guid userId);
    Task<LocationDTO> Update(LocationDTO locationDto, Guid userId);
    Task<bool> Delete(Guid locationId);
}