﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Pdf>().ToTable("pdfs");
        modelBuilder.Entity<Writer>().ToTable("Writers");
    }
        
}
