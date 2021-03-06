using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rpaapp.Models;
using Microsoft.AspNetCore.Identity;

namespace rpaapp.Data;

public class ApplicationDbContext : IdentityDbContext<Writer, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pdf> pdfs { get; set; }
    public DbSet<Writer> Writers { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Txt> Txts { get; set; }
    public DbSet<ProcessType> Processes { get; set; }
    public DbSet<LayoutConfig> Layouts { get; set; }
    public DbSet<PersonInCharge> People { get; set; }
    public DbSet<InvoiceType> Invoices { get; set; }
    public DbSet<Firm> Firms{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Pdf>().ToTable("pdfs");
        modelBuilder.Entity<Writer>().ToTable("Writers");
        modelBuilder.Entity<Document>().ToTable("Documents");
        modelBuilder.Entity<Txt>().ToTable("Txts");
        modelBuilder.Entity<ProcessType>().ToTable("Processes");
        modelBuilder.Entity<LayoutConfig>().ToTable("Layouts");
        modelBuilder.Entity<InvoiceType>().ToTable("Invoices");
        modelBuilder.Entity<PersonInCharge>().ToTable("People");
        modelBuilder.Entity<Firm>().ToTable("Firms");
    }
        
}
