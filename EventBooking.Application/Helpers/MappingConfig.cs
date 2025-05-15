using AutoMapper;
using EventBooking.Application.Dtos.Booking;
using EventBooking.Application.Dtos.Category;
using EventBooking.Application.Dtos.Event;
using EventBooking.Core.Entities;

namespace EventBooking.Helpers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryCreateDTO>().ReverseMap();
            CreateMap<Category, CategoryUpdateDTO>().ReverseMap();

            CreateMap<Event, EventDTO>().ReverseMap();
            CreateMap<Event, EventCreateDTO>().ReverseMap();
            CreateMap<Event, EventUpdateDTO>().ReverseMap();

            CreateMap<UsersEvents, BookingDTO>().ReverseMap();
        }
    }
}
