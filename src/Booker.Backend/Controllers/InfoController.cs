namespace Booker.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfoController(IInfoService infoService) : ControllerBase
{
    [HttpGet("currency")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> GetCurrency(CancellationToken cancellationToken)
    {
        var info = await infoService.GetCurrency(cancellationToken);
        return Ok(info);
    }
}
