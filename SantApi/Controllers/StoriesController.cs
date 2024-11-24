using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SantApi.Models;
using SantApi.Services;
using SantApi.Settings;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StoriesController : ControllerBase
{
    private readonly IStoryService _storyService;
    private readonly HackerNewsSettings _settings;

    public StoriesController(IStoryService storyService, IOptions<HackerNewsSettings> settings)
    {
        _storyService = storyService;
        _settings = settings.Value;
    }

    /// <summary>
    /// Retrieves the top N stories from Hacker News.
    /// </summary>
    /// <param name="n">The number of stories to retrieve. Default is 10.</param>
    /// <returns>Returns a list of stories sorted by score in descending order.</returns>
    /// <response code="200">Returns the list of stories.</response>
    /// <response code="400">If the parameter `n` is less than 1 or bigger than max value.</response>
    /// <response code="500">If there was an internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StoryResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBestStories([FromQuery] int n = 10)
    {
        var validationResult = ValidateN(n);
        if (validationResult != null)
        {
            return validationResult;
        }

        var stories = await _storyService.GetBestStoriesAsync(n);
        return Ok(stories);
    }

    private IActionResult? ValidateN(int n)
    {
        if (n < 1 || n > _settings.MaxStories)
        {
            return BadRequest(new { message = $"The field 'n' must be between 1 and {_settings.MaxStories}." });
        }

        return null;
    }
}
