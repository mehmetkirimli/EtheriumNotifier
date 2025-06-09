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
        Task SaveTransactionsAsync(int blockCount = 10);
        Task<List<ExternalTransactionDto>> FetchTransactionAsync(int blockCount = 10);
        Task<List<ExternalTransactionDto>> GetAllSavedTransactionsAsync();
        Task<List<ExternalTransactionDto>> GetFilteredTransactionsAsync(TransactionFilterRequestDto dto);
    }
}
