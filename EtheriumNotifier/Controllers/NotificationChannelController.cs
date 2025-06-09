using Application.Dto;
using Application.ServicesImpl;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EtheriumNotifier.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class NotificationChannelController : ControllerBase
    {
        private readonly INotificationChannelService _notificationChannelService;

        public NotificationChannelController(INotificationChannelService notificationChannelService)
        {
            _notificationChannelService = notificationChannelService;
        }

        [HttpPost]
        public async Task<IActionResult> AddChannel([FromBody] CreateNotificationChannelRequestDto dto)
        {
            try
            {
                var result = await _notificationChannelService.AddNotificationChannelAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChannel([FromBody] CreateNotificationChannelRequestDto dto)
        {
            try
            {
                var result = await _notificationChannelService.UpdateNotificationChannelAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{channelId}")]
        public async Task<IActionResult> DeleteChannel([FromRoute] Guid channelId)
        {
            try
            {
                var result = await _notificationChannelService.DeleteNotificationChannelAsync(channelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChannels([FromQuery] ChannelType? channelType, [FromQuery] int? userId)
        {
            var result = await _notificationChannelService.GetNotificationChannelsAsync(channelType, userId);
            return Ok(result);
        }

        [HttpGet("{channelId}")]
        public async Task<IActionResult> GetChannelById([FromRoute] Guid channelId)
        {
            try
            {
                var result = await _notificationChannelService.GetNotificationChannelByIdAsync(channelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
