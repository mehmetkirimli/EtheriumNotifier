using Application.Dto;
using Application.ServicesImpl;
using Microsoft.AspNetCore.Mvc;

namespace EtheriumNotifier.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IEthereumService _ethereumService;

        public TransactionController(IEthereumService ethereumService)
        {
            _ethereumService = ethereumService;
        }

        /// <summary>
        /// Son N bloktaki işlemleri zincirden okur (sadece gösterim için, veri kaydetmez).
        /// </summary>
        [HttpGet("fetch-transactions")]
        public async Task<IActionResult> GetRecentTransaction([FromQuery] int blockCount = 5)
        {
            try
            {
                var transactions = await _ethereumService.FetchTransactionAsync(blockCount);
                return Ok(transactions);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }

        /// <summary>
        /// Blok zinciriden işlemleri çeker ve veritabanına kayıt eder. Redis cache kontrolü yapar (idempotent).
        /// </summary>
        [HttpPost("fetch-and-save-transactions")]
        public async Task<IActionResult> FetchAndSaveTransactions([FromQuery] int blockCount = 5)
        {
            try
            {
                await _ethereumService.SaveTransactionsAsync(blockCount);
                return Ok("Transactions fetched and saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }

        /// <summary>
        /// Tüm kaydedilen işlemleri InMemory veritabanından getirir.
        /// </summary>
        [HttpGet("get-all-transactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                var transactions = await _ethereumService.GetAllSavedTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }

        /// <summary>
        /// Ethereum ağındaki işlemleri filtreler ve listeler 
        /// GET: api/transaction/filter
        /// </summary>

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredTransactions([FromQuery] TransactionFilterRequestDto dto)
        {
            try
            {
                var result = await _ethereumService.GetFilteredTransactionsAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }


        [HttpGet("by-hash/{hash}")]
        public async Task<IActionResult> GetTransactionByHash([FromRoute] string hash)
        {
            try
            {
                var result = await _ethereumService.GetTransactionByHashAsync(hash);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }
    }
}
