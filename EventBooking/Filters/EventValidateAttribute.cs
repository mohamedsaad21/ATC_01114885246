using EventBooking.Application.Dtos.Event;
using EventBooking.Application.Models;
using EventBooking.Core.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
namespace EventBooking
{
    public class EventValidateAttribute : Attribute, IActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventValidateAttribute(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var dto = context.ActionArguments.Values
            .FirstOrDefault(v => v is EventCreateDTO || v is EventUpdateDTO);

            if (dto == null)
            {
                context.Result = new BadRequestObjectResult(new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "Invalid event data" }
                });
                return;
            }

            var errors = new List<string>();

            int categoryId = 0;
            decimal price = 0;

            if (dto is EventCreateDTO createDTO)
            {
                categoryId = createDTO.CategoryId;
                price = createDTO.Price;
            }
            else if (dto is EventUpdateDTO updateDTO)
            {
                categoryId = updateDTO.CategoryId;
                price = updateDTO.Price;
            }

            if (price < 0)
                errors.Add("Price must be a positive value");

            var category = _unitOfWork.Category.GetAsync(u => u.Id == categoryId).Result;
            if (category is null)
                errors.Add("Category doesn't exist");

            if (errors.Count > 0)
            {
                context.Result = new BadRequestObjectResult(new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = errors
                });
            }

        }
    }
}
