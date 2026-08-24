namespace day05_S.Services
{
    public interface IEventService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<List><EventRegistraion>> GetUserRegistrationsAsync( );
        Task<string> RegisterEventAsync(string userId, string eventId);
        Task<bool> CancelRegistionAsync(string userId, int regId);
        Task<object> GetStatisticsAsync();
    }
}
