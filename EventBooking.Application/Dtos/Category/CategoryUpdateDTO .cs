using System.ComponentModel.DataAnnotations;

namespace EventBooking.Application.Dtos.Category
{
    public class CategoryUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
