namespace RankingServer.Controllers;

using Microsoft.AspNetCore.Mvc;
using RankingServer.Models;
using RankingServer.Services;

[ApiController]
[Route("[controller]")]
public class RankingController : ControllerBase
{
    private readonly RankingService _rankingService;

    public RankingController(RankingService rankingService)
    {
        _rankingService = rankingService;
    }

    // 1. 유저 점수 추가 API
    [HttpPost("add")]
    public async Task<IActionResult> AddUserScore([FromBody] UserScore userScore)
    {
        await _rankingService.AddScoreAsync(userScore);
        return Ok("Score added successfully.");
    }

    // 2. 상위 10명의 랭킹 목록 API
    [HttpPost("top")]
    public async Task<IActionResult> GetTopRankings()
    {
        var topRankings = await _rankingService.GetTopRankingAsync();
        return Ok(topRankings);
    }
}
