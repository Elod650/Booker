namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfoController(IInfoRepository infoRepository) : ControllerBase
{
    [HttpGet("currency")]
    public async Task<ActionResult<string>> GetCurency()
    {
        var info = infoRepository.GetCurrency();
        return Ok(info);
    }
}
