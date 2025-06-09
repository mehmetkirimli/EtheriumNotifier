using Application.ServicesImpl;
using Microsoft.AspNetCore.Mvc;

namespace EtheriumNotifier.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EthereumController : ControllerBase
    {
        private readonly IEthereumService _ethereumService;

        public EthereumController(IEthereumService ethereumService)
        {
            _ethereumService = ethereumService;
        }

        /// <summary>
        /// Son N bloktaki işlemleri zincirden okur (sadece gösterim için, veri kaydetmez).
        /// </summary>
        [HttpGet("fetch-transactions")]
        public async Task<IActionResult> GetRecentTransaction([FromQuery] int blockCount = 5)
        {
            var transactions = await _ethereumService.FetchTransactionAsync(blockCount);
            return Ok(transactions);
        }

        /// <summary>
        /// Zincirden işlemleri çeker ve veritabanına kayıt eder. Redis cache kontrolü yapar (idempotent).
        /// </summary>
        [HttpPost("fetch-and-save-transactions")]
        public async Task<IActionResult> FetchAndSaveTransactions([FromQuery] int blockCount = 5)
        {
            await _ethereumService.SaveTransactionsAsync(blockCount);
            return Ok("Transactions fetched and saved successfully.");
        }

        /// <summary>
        /// Tüm kaydedilen işlemleri InMemory veritabanından getirir.
        /// </summary>
        [HttpGet("get-all-transactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _ethereumService.GetAllSavedTransactionsAsync();
            return Ok(transactions);
        }

    }
}
