/* using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using Microsoft.AspNetCore.Identity;

namespace rpaapp.Data;

public class TwoContext : IdentityDbContext<Writer, IdentityRole<int>, int>
{
    public TwoContext(DbContextOptions<TwoContext> options)
        : base(options)
    {
    }

    public DbSet<Pdf> pdfs { get; set; }
    public DbSet<Writer> Writers { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Txt> Txts { get; set; }
    public DbSet<ProcessType> Processes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Pdf>().ToTable("pdfs");
        modelBuilder.Entity<Writer>().ToTable("Writers");
        modelBuilder.Entity<Document>().ToTable("Documents");
        modelBuilder.Entity<Txt>().ToTable("Txts");
        modelBuilder.Entity<ProcessType>().ToTable("Processes");
    }
        
} */