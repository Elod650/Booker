namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfoController(IInfoRepository infoRepository) : ControllerBase
{
    [HttpGet("currency")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> GetCurency()
    {
        var info = infoRepository.GetCurrency();
        return Ok(info);
    }
}
