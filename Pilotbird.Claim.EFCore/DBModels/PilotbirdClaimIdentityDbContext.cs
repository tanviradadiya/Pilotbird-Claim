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
    public class PilotbirdClaimIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public PilotbirdClaimIdentityDbContext(DbContextOptions<PilotbirdClaimIdentityDbContext> options) : base(options)
        {
        }
    }
}
