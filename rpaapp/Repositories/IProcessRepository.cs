using System.Threading.Tasks;
using rpaapp.Models;

namespace rpaapp.Repositories;

public interface IProcessRepository
{
    Task AddProcessAsync(ProcessType process);
}