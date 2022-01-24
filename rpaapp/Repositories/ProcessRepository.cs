using rpaapp.Data;
using rpaapp.Models;

namespace rpaapp.Repositories;

public class ProcessRepository : IProcessRepository
{
    private ApplicationDbContext context;

    public ProcessRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task AddProcessAsync(ProcessType process)
    {
        await context.Processes.AddAsync(process);
        await context.SaveChangesAsync();
    }
}