using AutoMapper;
using EventBooking.Application.Dtos.Event;
using EventBooking.Application.Models;
using EventBooking.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using EventBooking.Core.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;
namespace EventBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            this._response = new();
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetEvents(int pageSize = 3, int pageNumber = 1)
        {
            try
            {
                _response.Result = _mapper.Map<IEnumerable<EventDTO>>(await _unitOfWork.Event.GetAllAsync(includeProperties: "Category", pageSize:pageSize, pageNumber:pageNumber));
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetEvent")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetEvent(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var Event = await _unitOfWork.Event.GetAsync(u => u.Id == id, false, includeProperties:"Category");
                if (Event == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                _response.Result = _mapper.Map<EventDTO>(Event);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Role_Admin)]
        [ServiceFilter(typeof(EventValidateAttribute))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> CreateEvent([FromBody] EventCreateDTO eventDTO, IFormFile? file)
        {
            try
            {
                var Event = _mapper.Map<Event>(eventDTO);       
                var wwwRootPath = _webHostEnvironment.ContentRootPath;
                if (file != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var eventPath = Path.Combine(wwwRootPath, @"images\events");
                    using(var fileStream = new FileStream(Path.Combine(eventPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    Event.ImageUrl = @"\images\events\" + fileName;
                }
                await _unitOfWork.Event.CreateAsync(Event);
                await _unitOfWork.SaveAsync();

                _response.Result = CreatedAtRoute("GetEvent", new { Id = Event.Id }, Event);
                _response.StatusCode = HttpStatusCode.Created;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpDelete("{id:int}", Name = "DeleteEvent")]
        [Authorize(Roles = Roles.Role_Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> DeleteEvent(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var Event = await _unitOfWork.Event.GetAsync(u => u.Id == id, false);
                if (Event == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
                await _unitOfWork.Event.RemoveAsync(Event);
                await _unitOfWork.SaveAsync();
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpPut("{id:int}", Name = "UpdateEvent")]
        [Authorize(Roles = Roles.Role_Admin)]
        [ServiceFilter(typeof(EventValidateAttribute))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateEvent(int id, [FromBody] EventUpdateDTO eventDTO, IFormFile? file)
        {
            try
            {
                if (id == 0  || id != eventDTO.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var Event = await _unitOfWork.Event.GetAsync(u => u.Id == id, false);
                if (Event == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
                var wwwRootPath = _webHostEnvironment.ContentRootPath;
                if (file != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var eventPath = Path.Combine(wwwRootPath, @"images\events");
                    using (var fileStream = new FileStream(Path.Combine(eventPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    Event.ImageUrl = @"\images\events\" + fileName;
                }
                Event = _mapper.Map<Event>(eventDTO);

                await _unitOfWork.Event.UpdateAsync(Event);
                await _unitOfWork.SaveAsync();

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
    }
}
