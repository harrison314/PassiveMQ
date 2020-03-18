using Harrison314.PassiveMQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Contracts
{
    public interface IQueuRepository
    {
        Task<QueuDto> GetByIdAsync(Guid id);

        Task<List<QueuDto>> GetAllAsync();

        Task<QueuDto> GetByNameOrDefaultAsync(string name);

        Task<Guid> CreateAsync(QueuCreateReqDto queuDto);

        Task DeleteAsync(Guid id);
    }
}
