using rpaapp.Models;

namespace rpaapp.Repositories;

public interface IProcessRepository
{
    Task AddProcessAsync(ProcessType process);
    Task<List<ProcessType>> GetProcessesAsync();
}