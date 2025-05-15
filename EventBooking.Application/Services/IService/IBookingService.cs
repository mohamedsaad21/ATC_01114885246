using EventBooking.Application.Dtos.Booking;
using EventBooking.Application.Dtos.Event;
namespace EventBooking.Application.Services.IService
{
    public interface IBookingService
    {
        Task<List<EventDTO>> GetBookingsAsync(string ApplicationUserId);
        Task CreateBookingAsync(BookingDTO bookingDTO);
    }
}
