using Application.ServicesImpl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContext;

namespace EtheriumNotifier.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IEthereumService _ethereumService;
        private readonly DatabaseContext _databaseContext;

        public TestController(IEthereumService ethereumService , DatabaseContext db)
        {
            _ethereumService = ethereumService;
            _databaseContext = db;
        }

        [HttpGet("recent-transaction")]
        public async Task<IActionResult> GetRecentTransaction() 
        {
            var txs = await _ethereumService.GetRecentTransactionAsync(1);
            return Ok(txs);
        }

        [HttpGet("fetch-and-save")]
        public async Task<IActionResult> FetchAndSaveExternalTransactions() 
        {
            await _ethereumService.FetchAndSaveRecentTransactionsAsync(5);
            return Ok("Transactions fetched and saved successfully.");
        }

        [HttpGet("all-transactions")]
        public async Task<IActionResult> GetAllTransaction()
        {
            var transactions = await _databaseContext.ExternalTransactions.ToListAsync();
            return Ok(transactions);
        }
    }
}
