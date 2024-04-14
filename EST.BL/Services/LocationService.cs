using EST.BL.Interfaces;
using EST.DAL.DataAccess.EF;
using EST.DAL.Models;
using EST.Domain.DTOs;
using EST.Domain.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EST.BL.Services;

public class LocationService : ILocationService
{
    private readonly ExpensesContext _context;

    public LocationService(ExpensesContext context)
    {
        _context = context;
    }
    public async Task<List<LocationDTO>> GetUserSavedLocation(Guid userId)
    {
        var locations = await _context.Locations
            .Where(l => l.UserId == userId && l.Save == true)
            .ToListAsync();
        if (locations.Count > 0)
        {
            return locations.Select(l => new LocationDTO()
            {
                Name = l.Name,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                Address = l.Address,
                Save = l.Save
            }).ToList();
        }
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status404NotFound,
            Title = "Not found",
            Detail = "There is no locations saved by user on the server"
        };
    }

    public async Task<CreatedLocationDTO> Create(LocationDTO locationDto, Guid userId)
    {
        var location = new Location
        {
            Name = locationDto.Name,
            Latitude = locationDto.Latitude,
            Longitude = locationDto.Longitude,
            Address = locationDto.Address,
            Save = locationDto.Save,
            UserId = userId
        };
        _context.Locations.Add(location);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            var createdLocation = await _context.Locations
                .Where(l => l.Address == location.Address && l.UserId == userId)
                .FirstOrDefaultAsync();
            return new CreatedLocationDTO()
            {
                Id = createdLocation.Id,
                Name = createdLocation.Name,
            };
        }
        else
        {
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't create location",
                Detail = "Error occured while creating location on server"
            };
        }
    }

    public async Task<bool> AddLocationToExpense(AddLocationToExpenseDTO location)
    {
        var resultExpense = await _context.Expenses.AnyAsync(e => e.Id == location.ExpenseId);
        var resultLocation = await _context.Locations.AnyAsync(l => l.Id == location.LocationId);
        if (!resultExpense || !resultLocation)
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Title = "Wrong id",
                Detail = "There is no such location or expense with this id's on db"
            };
        var expenseLocation = new ExpenseLocation()
        {
            ExpenseId = location.ExpenseId,
            LocationId = location.LocationId
        };
        _context.ExpensesLocations.Add(expenseLocation);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<LocationDTO> Update(LocationDTO locationDto, Guid userId)
    {
        var location = await _context.Locations
            .Where(l => l.Address == locationDto.Address && l.UserId == userId)
            .FirstOrDefaultAsync();
        if (location != null)
        {
            location.Name = locationDto.Name;
            location.Latitude = locationDto.Latitude;
            location.Longitude = locationDto.Longitude;
            location.Address = locationDto.Address;
            location.Save = locationDto.Save;

            _context.Locations.Update(location);
            if (await _context.SaveChangesAsync() <= 0)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't update location",
                    Detail = "Error occurred while updating location on server"
                };
            
            var updatedLocation = await _context.Locations
                .Where(l => l.Address == locationDto.Address && l.UserId == userId)
                .FirstOrDefaultAsync();
            return new LocationDTO()
            {
                Name = updatedLocation.Name,
                Latitude = updatedLocation.Latitude,
                Longitude = updatedLocation.Longitude,
                Address = updatedLocation.Address,
                Save = updatedLocation.Save
            };
        }
        throw new ApiException()
        {
            StatusCode = StatusCodes.Status404NotFound,
            Title = "Not found",
            Detail = "There is no such location on the server"
        };
    }

    public async Task<bool> Delete(Guid locationId)
    {
        var location = await _context.Locations
            .Where(l => l.Id == locationId)
            .FirstOrDefaultAsync();
        if (location != null)
        {
            _context.Locations.Remove(location);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        throw new ApiException()
        {
            StatusCode = StatusCodes.Status404NotFound,
            Title = "Not found",
            Detail = "There is no such location on the server"
        };
    }
}