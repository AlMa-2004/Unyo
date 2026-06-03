namespace Unyo.Dtos;

public record HomeStatsDto(
    List<EventDto> RecentEvents,
    int TotalEvents,
    int TotalCategories);