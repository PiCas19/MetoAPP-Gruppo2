
namespace BackendProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationSettingsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] UserNotificationSettings[] settings)
        {
            if (settings == null)
                return BadRequest("Invalid payload.");

            NotificationSettingsRepository.Settings.Clear();
            NotificationSettingsRepository.Settings.AddRange(settings);
            return Ok(new { message = "Settings updated successfully." });
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(NotificationSettingsRepository.Settings);
        }
    }
}