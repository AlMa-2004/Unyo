using System;
using System.Threading;
using System.Threading.Tasks;
using Unyo.Data;

namespace Unyo.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private ICategoryRepository? _categories;
    private IEventRepository? _events;
    private IEventRegistrationRepository? _eventRegistrations;
    private ITicketRepository? _tickets;
    private IVenueRepository? _venues;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Repositories are lazily initialized to avoid unnecessary instantiation if some repositories are not used during a unit of work.
    public ICategoryRepository Categories
        => _categories ??= new CategoryRepository(_context);

    public IEventRepository Events
        => _events ??= new EventRepository(_context);

    public IEventRegistrationRepository EventRegistrations
        => _eventRegistrations ??= new EventRegistrationRepository(_context);

    public ITicketRepository Tickets
        => _tickets ??= new TicketRepository(_context);

    public IVenueRepository Venues
        => _venues ??= new VenueRepository(_context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}