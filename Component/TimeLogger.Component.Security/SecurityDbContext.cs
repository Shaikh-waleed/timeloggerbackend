using TimeLogger.Component.Security.Entities;
using TimeLogger.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Component.Security
{
    public abstract class SecurityDbContext : DbContextIdentity<UserIdentity, RoleIdentity, string> //, IDbContextSecurity
    {
        protected SecurityDbContext(DbContextOptions options) : base(options)
        {
        }

        protected SecurityDbContext()
        {
        }
    }

    public abstract class DbContextIdentity<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>, new()
        where TRole : IdentityRole<TKey>, new()
    {
        protected DbContextIdentity(DbContextOptions options) : base(options)
        {
        }

        protected DbContextIdentity()
        {
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserIdentity>(entity =>
            {
                entity.HasMany(e => e.UserRoles)
                      .WithOne(e => e.User)
                      .HasForeignKey(ur => ur.UserId)
                      .IsRequired();

                entity.HasMany(e => e.Logins)
                      .WithOne(e => e.User)
                      .HasForeignKey(ur => ur.UserId)
                      .IsRequired();
            });

            builder.Entity<RoleIdentity>(entity =>
            {
                entity.HasMany(e => e.UserRoles)
                      .WithOne(x => x.Role)
                      .HasForeignKey(ur => ur.RoleId)
                      .IsRequired();
            });

            builder.Entity<PreviousPassword>(entity =>
            {
                entity.HasKey(e => new { e.PasswordHash, e.UserId });
            });
        }

        public virtual DbSet<TwoFactorType> TwoFactorTypes { get; set; }
        public virtual DbSet<PreviousPassword> PreviousPasswords { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<StatusType> StatusTypes { get; set; }
        public virtual DbSet<Addresses> Addresses { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
    }
}
