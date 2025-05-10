using EventBooking.Core.Entities;
using EventBooking.Core.IRepository;
using EventBooking.Infrastructure.Data;

namespace EventBooking.Infrastructure.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
