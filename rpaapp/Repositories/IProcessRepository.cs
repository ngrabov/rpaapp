using rpaapp.Models;

namespace rpaapp.Repositories;

public interface IProcessRepository
{
    Task AddProcessAsync(ProcessType process);
    Task RemoveProcessAsync(ProcessType process);
    Task<List<ProcessType>> GetProcessesAsync();
    Task<ProcessType> GetProcessTypeAsync(int? id);
}