using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class GameController : Controller
{
    private readonly Service _gameService;
    
    public GameController(Service gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    [Route("games")]
    public async Task<IActionResult> GetAllGames(int? gameId, int? homeTeamId, int? awayTeamId, int? season)
    {
        var games = _gameService.GetAllGames(gameId, homeTeamId, awayTeamId, season);
        return Ok(games);
    }
}