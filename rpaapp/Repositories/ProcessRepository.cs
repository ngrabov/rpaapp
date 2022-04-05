using rpaapp.Data;
using rpaapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace rpaapp.Repositories;

public class ProcessRepository : IProcessRepository
{
    private ApplicationDbContext context;
    private readonly UserManager<Writer> userManager;
    private readonly SignInManager<Writer> signInManager;

    public ProcessRepository(ApplicationDbContext context, UserManager<Writer> userManager, SignInManager<Writer> signInManager)
    {
        this.context = context;
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public async Task<List<ProcessType>> GetProcessesAsync()
    {
        return await context.Processes.ToListAsync();
    }

    public async Task AddProcessAsync(ProcessType process)
    {
        await context.Processes.AddAsync(process);
        await context.SaveChangesAsync();
    }

    public async Task<ProcessType> GetProcessTypeAsync(int? id)
    {
        var process = await context.Processes.FirstOrDefaultAsync(c => c.id == id);
        return process;
    }

    public async Task RemoveProcessAsync(ProcessType process)
    {
        context.Processes.Remove(process);
        await context.SaveChangesAsync();
    }
}