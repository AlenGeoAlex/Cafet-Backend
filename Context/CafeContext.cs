using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Cafet_Backend.Context;

public class CafeContext : DbContext
{

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<FoodCategory?> Categories { get; set; }
    public DbSet<Food> Foods { get; set; }

    public CafeContext(DbContextOptions<CafeContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        var cartBuilder = modelBuilder.Entity<Cart>();
        cartBuilder.Property(entity => entity.CartData)
            .IsRequired()
            .HasConversion(
                outp => (outp == null) ? JsonConvert.SerializeObject(new Dictionary<int, int>()) : JsonConvert.SerializeObject(outp),
                input => JsonConvert.DeserializeObject<Dictionary<int, int>>(input) ?? new Dictionary<int, int>()
            );

        //Configuring Users
        var userBuilder = modelBuilder.Entity<User>();
        userBuilder
            .Property(u => u.Activated)
            .HasDefaultValue(false);

        userBuilder
            .Property(u => u.Deleted)
            .HasDefaultValue(false);
        
        userBuilder
            .Property(u => u.WalletBalance)
            .HasDefaultValue(0.0);

        userBuilder
            .Property(u => u.FirstName)
            .IsRequired(false);
        
        userBuilder
            .Property(u => u.LastName)
            .IsRequired(false);

        userBuilder
            .Property(u => u.ProfileImage)
            .IsRequired()
            .HasDefaultValue("default.png");

        var foodBuilder = modelBuilder.Entity<Food>();

        foodBuilder
            .Property(f => f.FoodImage)
            .HasDefaultValue("default.png");
        
        
        //Adding Default Roles
        EntityTypeBuilder<Role> roleBuilder = modelBuilder.Entity<Role>();
        roleBuilder.HasData(Role.Administrator);
        roleBuilder.HasData(Role.CafetStaff);
        roleBuilder.HasData(Role.Customer);

    }
}