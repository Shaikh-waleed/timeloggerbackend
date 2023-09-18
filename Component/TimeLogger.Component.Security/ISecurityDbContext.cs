using TimeLogger.Component.Security.Entities;
using TimeLogger.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeLogger.Component.Security
{
    public interface ISecurityDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        DbSet<Data.Entity.ApplicationUser> Users { get; set; }
        DbSet<TwoFactorType> TwoFactorTypes { get; set; }
        DbSet<PreviousPassword> PreviousPasswords { get; set; }
        DbSet<Status> Statuses { get; set; }
        DbSet<StatusType> StatusTypes { get; set; }
        DbSet<Addresses> Addresses { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<City> Cities { get; set; }
        DbSet<Company> Companies { get; set; }
    }
}
