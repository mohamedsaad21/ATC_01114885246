using EventBooking.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventBooking.Application.Dtos.Event
{
    public class EventCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Venue { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
