using Application.Dto;
using Application.ServicesImpl;
using Microsoft.AspNetCore.Mvc;

namespace EtheriumNotifier.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] NotificationLogFilterRequestDto filter)
        {
            try
            {
                var logs = await _notificationService.GetNotificationLogsAsync(filter);
                return Ok(logs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }
    }

}
