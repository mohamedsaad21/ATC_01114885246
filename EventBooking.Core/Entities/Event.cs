using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EventBooking.Core.Entities
{
    public class Event
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Venue { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        [ForeignKey(nameof(Category))]
        // Foreign key for Category
        public int CategoryId { get; set; }
        public Category Category { get; set; } // Navigation property
        public List<UsersEvents> UsersEvents { get; set; }
    }
}
