using System.ComponentModel.DataAnnotations.Schema;
namespace EventBooking.Core.Entities
{
    public class UsersEvents
    {
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
