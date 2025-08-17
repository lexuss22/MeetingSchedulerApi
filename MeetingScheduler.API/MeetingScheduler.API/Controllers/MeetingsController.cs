using DI.BLL.Services.Interface;
using DI.Contracts.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly IMeetingServices _meetingServices;

        public MeetingsController(IMeetingServices meetingServices)
        {
            _meetingServices = meetingServices;
        }

        [HttpPost]
        public IActionResult CreateMeeting(MeetingDto meetingDto)
        {
            var result = _meetingServices.ScheduleMeeting(meetingDto);
            return Ok(result);
        }
    }
}
