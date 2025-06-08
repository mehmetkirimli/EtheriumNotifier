using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;

namespace Application.ServicesImpl
{
    public interface IEhtereumService
    {
        Task<List<ExternalTransactionDto>> GetRecentTransactionAsync(int blockCount = 100);
    }
}
