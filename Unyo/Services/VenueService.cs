using Microsoft.EntityFrameworkCore;
using Unyo.Models;
using Unyo.Repositories;

namespace Unyo.Services;

public class VenueService : IVenueService
{
    private readonly IUnitOfWork _unitOfWork;

    public VenueService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Venue>> GetAllVenuesAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Venues.GetAllAsync(cancellationToken);
    }

    public async Task<Venue?> GetVenueByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Venues.GetByIdWithEventsAsync(id, cancellationToken);
    }

    public async Task<Venue> CreateVenueAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Venues.AddAsync(venue, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return venue;
    }

    public async Task<bool> DeleteVenueAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Venues.GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;

        _unitOfWork.Venues.Delete(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateVenueAsync(Venue venue, CancellationToken cancellationToken)
    {
        var existingVenue = await _unitOfWork.Venues.GetByIdAsync(venue.Id, cancellationToken);

        if (existingVenue == null)
        {
            return false;
        }

        existingVenue.Name = venue.Name;
        existingVenue.Address = venue.Address;
        existingVenue.Capacity = venue.Capacity;
        existingVenue.ImagePath = venue.ImagePath;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}