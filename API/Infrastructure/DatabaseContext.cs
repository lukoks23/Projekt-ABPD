using API.Entities;
using API.Entities.BusinessEntities;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Version = API.Entities.BusinessEntities.Version;

namespace API.Infrastructure;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;
    private DbContextOptions _opt;
    
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<City> Cities { get; set; }
    public virtual DbSet<Street> Streets { get; set; }
    public virtual DbSet<Address> Addresses { get; set; }
    public virtual DbSet<Invidual> Inviduals { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Entity> Entities { get; set; }
    public virtual DbSet<Subscription> Subscriptions { get; set; }
    public virtual DbSet<License> Licenses { get; set; }
    public virtual DbSet<Billing> Billings { get; set; }
    public virtual DbSet<Contract> Contracts { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<BillingType> BillingTypes { get; set; }
    public virtual DbSet<Discount> Discounts { get; set; }
    public virtual DbSet<SoftwareCategory> SoftwareCategories { get; set; }
    public virtual DbSet<Software> Softwares { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<SoftDisc> SoftDiscs { get; set; }
    public virtual DbSet<SoftwareCost> SoftwareCosts { get; set; }
    public virtual DbSet<Version> Versions { get; set; }
    public virtual DbSet<AvailableVersion> AvailableVersions { get; set; }

    public DatabaseContext(DbContextOptions opt) : base(opt)
    {
    }

    public DatabaseContext(DbContextOptions opt, IConfiguration configuration) :
        base(opt)
    {
        this._configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    }


    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (_configuration == null)
        {
            modelBuilder.Entity<Address>()
                .Property(a => a.ApartmentNumber)
                .IsRequired(false);
            return;
        }

        
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_configuration["DB:DefaultSchema"]);

        modelBuilder.Entity<Role>().HasData(
            new Role {Id = 1, Name = "Admin"},
            new Role {Id = 2, Name = "Worker"}
        );
        modelBuilder.Entity<User>().HasData(
            new User {UserName = "Admin",PasswordHash = "AQAAAAIAAYagAAAAEHEF/gZKV53d1t2Yi7rjJJR9X+RSH7z7qNajMBmi/ZDGRXaCuBL0CFI0+SMsQ4IsvA==",ExpiresAt = new DateTime(2000,1,1),RoleId = 1},
            new User {UserName = "Worker",PasswordHash = "AQAAAAIAAYagAAAAENx1VsgLhsq97yO7TiUbzrqchjs8cRvbhsgXlz8zfZlXQEaYcxYvlPCLaUFXAMuB/A==",ExpiresAt = new DateTime(2000,1,1), RoleId = 2}
        );
        modelBuilder.Entity<BillingType>().HasData(
            new BillingType {Id = 1, Type = "License"},
            new BillingType {Id = 2, Type = "Subscription"}
        );
        modelBuilder.Entity<Discount>().HasData(
            new Discount
            {
                Id = 1, Name = "Basic", BillingTypeId = 1, Percent = 33.3m, From = new DateTime(2020, 1, 1),
                To = new DateTime(2030, 1, 1)
            }
        );
        modelBuilder.Entity<SoftwareCategory>().HasData(
            new SoftwareCategory { Id = 1, Category = "Education" }
        );
        modelBuilder.Entity<Software>().HasData(
            new Software { Id = 1, Name = "Rider", Description = "C# IDE", ActualVersion = "1.0.0" }
        );
        modelBuilder.Entity<SoftwareCost>().HasData(
            new SoftwareCost { Id = 1, SoftwareId = 1, Price = 10000.00m, BillingTypeId = 1 }
        );
        modelBuilder.Entity<SoftDisc>().HasData(
            new SoftDisc { Id = 1, SoftwareId = 1, DiscountId = 1 }
        );
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, SoftwareId = 1, CategoryId = 1 }
        );
        modelBuilder.Entity<Version>().HasData(
            new Version { Id = 1, Name = "0.9.0", SoftwareId = 1, PublishDate = new DateTime(2020, 1, 1) },
            new Version { Id = 2, Name = "1.0.0", SoftwareId = 1, PublishDate = new DateTime(2021, 1, 1) }
        );
    }
}