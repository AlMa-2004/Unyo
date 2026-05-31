using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unyo.Repositories;

public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }
    IEventRepository Events { get; }
    IEventRegistrationRepository EventRegistrations { get; }
    ITicketRepository Tickets { get; }
    IVenueRepository Venues { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}