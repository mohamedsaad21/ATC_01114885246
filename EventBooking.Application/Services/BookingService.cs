using AutoMapper;
using EventBooking.Application.Dtos.Booking;
using EventBooking.Application.Dtos.Event;
using EventBooking.Application.Services.IService;
using EventBooking.Core.Entities;
using EventBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace EventBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public BookingService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<EventDTO>> GetBookingsAsync(string ApplicationUserId)
        {
            var events = _mapper.Map<List<EventDTO>>(await _db.UsersEvents
                .Where(x => x.ApplicationUserId == ApplicationUserId).ToListAsync());
            return events;
        }
        public async Task CreateBookingAsync(BookingDTO bookingDTO)
        {
            var userEvent = _mapper.Map<UsersEvents>(bookingDTO);
            await _db.UsersEvents.AddAsync(userEvent);
            await _db.SaveChangesAsync();
        }
    }
}
