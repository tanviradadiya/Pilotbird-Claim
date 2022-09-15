using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pilotbird.Claim.EFCore.DBModels.DB;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Pilotbird.Claim.EFCore.DBModels
{
    public partial class PilotbirdClaimDbContext : DbContext
    {
        public PilotbirdClaimDbContext(DbContextOptions<PilotbirdClaimDbContext> options)
       : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Data Source=192.168.5.10;Initial Catalog=Pilotbird_Claim;Persist Security Info=True;User ID=sa;Password=CFvgbhnj12#");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
