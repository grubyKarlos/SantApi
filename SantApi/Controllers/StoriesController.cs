using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StoriesController : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}
