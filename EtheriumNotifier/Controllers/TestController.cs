using Application.ServicesImpl;
using Microsoft.AspNetCore.Mvc;

namespace EtheriumNotifier.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IEthereumService _ethereumService;

        public TestController(IEthereumService ethereumService)
        {
            _ethereumService = ethereumService;
        }

        [HttpGet("recent-transaction")]
        public async Task<IActionResult> GetRecentTransaction() 
        {
            var txs = await _ethereumService.GetRecentTransactionAsync(1);
            return Ok(txs);
        }

    }
}
