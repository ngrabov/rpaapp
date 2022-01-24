using rpaapp.Data;
using rpaapp.Models;

namespace rpaapp.Repositories;

public class ProcessRepository : IProcessRepository
{
    private ApplicationDbContext context;
    //private TwoContext twoContext;

    public ProcessRepository(ApplicationDbContext context/* , TwoContext twoContext */)
    {
        //this.twoContext = twoContext;
        this.context = context;
    }

    public async Task AddProcessAsync(ProcessType process)
    {
        await context.Processes.AddAsync(process);
        await context.SaveChangesAsync();/* 
        await twoContext.Processes.AddAsync(process);
        await twoContext.SaveChangesAsync(); */
    }
}