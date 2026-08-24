namespace day05_S.Data
{
    public class EventDbContextcs : dbContext
    {
        public EventDbContextcs(DbContextOptions<EventDbContextsc> options) : base(options) { }

        public object Events { get; internal set; }
    }
}
