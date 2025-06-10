using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;

namespace Application.ServicesImpl
{
    public interface IEthereumService
    {
        Task<List<ExternalTransactionDto>> SaveTransactionsAsync(int blockCount = 10);
        Task<ExternalTransactionDto> GetTransactionByHashAsync(string hash);
        Task<List<ExternalTransactionDto>> FetchTransactionAsync(int blockCount = 10);
        Task<List<ExternalTransactionDto>> GetAllSavedTransactionsAsync();
        Task<List<ExternalTransactionDto>> GetFilteredTransactionsAsync(TransactionFilterRequestDto dto);
        Task NotifyAllAsync(List<ExternalTransactionDto> transactions);
    }
}
