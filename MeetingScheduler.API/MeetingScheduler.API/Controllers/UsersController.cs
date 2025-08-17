using DI.BLL.Services.Interface;
using DI.Contracts.Models.DTO;
using DI.DAL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        // GET: api/users/userId/meetings
        [HttpGet("{userId}/meetings")]
        public IActionResult GetUsers([FromQuery] int userId)
        {
            var result = _userServices.GetUserMeetings(userId);
            if (result == null)
            {
                return NotFound("No meetings found for the specified user.");
            }
            return Ok(result);
        }


        // POST: api/users
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDTO user)
        {
            var result = _userServices.CreateUser(user);
            return Ok(result);
        }
    }
}
