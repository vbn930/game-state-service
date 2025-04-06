using Microsoft.AspNetCore.Mvc;

using GameStateService.Services;
using GameStateService.Dtos;
using GameStateService.Models;
using GameStateService.Utils;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleController : ControllerBase
    {
        private readonly MemoryCacheService _memoryCacheService;
        private readonly GameFlowManager _gameFlowManager;
        private readonly GameBattleHandler _gameBattleHandler;

        public BattleController(MemoryCacheService memoryCacheService, GameFlowManager gameFlowManager, GameBattleHandler gameBattleHandler)
        {
            _memoryCacheService = memoryCacheService;
            _gameFlowManager = gameFlowManager;
            _gameBattleHandler = gameBattleHandler;
        }

        [HttpGet("{userId}/summary")]
        public async Task<IActionResult> GetBattleSummary(string userId)
        {
            try
            {
                string battleSummary = await _gameBattleHandler.GetBattleSummaryAsync(userId);
                return Content(battleSummary, "text/plain");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("{userId}/attack")]
        public async Task<IActionResult> AttackMonster(string userId, [FromQuery] bool skillUsed = false)
        {
            try
            {
                string attackMessage = await _gameBattleHandler.AttackMonsterAsync(userId, skillUsed);
                return Content(attackMessage, "text/plain");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}