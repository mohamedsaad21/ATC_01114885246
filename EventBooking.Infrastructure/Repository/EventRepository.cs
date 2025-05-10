using EventBooking.Core.Entities;
using EventBooking.Core.IRepository;
using EventBooking.Infrastructure.Data;

namespace EventBooking.Infrastructure.Repository
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        private readonly ApplicationDbContext _db;
        public EventRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
