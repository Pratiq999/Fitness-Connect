﻿using FitnessConnect.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessConnect.Areas.Identity.Data;

public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }
    public DbSet<Contact> Contact { get; set; }
    public DbSet<Logger> Logger { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>();
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
