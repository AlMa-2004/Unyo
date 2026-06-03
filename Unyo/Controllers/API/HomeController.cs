using Microsoft.AspNetCore.Mvc;
using Unyo.Services;
using Unyo.Dtos;
using Unyo.Mappings;

namespace Unyo.Controllers.API;

[ApiController]
[Route("api/home")]
public class HomeApiController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ICategoryService _categoryService;

    public HomeApiController(IEventService eventService, ICategoryService categoryService)
    {
        _eventService = eventService;
        _categoryService = categoryService;
    }

    // GET: /api/home/stats
    [HttpGet("stats")]
    [ProducesResponseType(typeof(HomeStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HomeStatsDto>> GetHomeStats(CancellationToken cancellationToken)
    {
        var events = await _eventService.GetAllEventsAsync(cancellationToken);
        var recentEvents = events.Take(3).Select(e => e.MapToDto()).ToList();

        var totalEvents = events.Count();

        var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);

        var statsDto = new HomeStatsDto(
            RecentEvents: recentEvents,
            TotalEvents: totalEvents,
            TotalCategories: categories.Count()
        );

        return Ok(statsDto);
    }
}