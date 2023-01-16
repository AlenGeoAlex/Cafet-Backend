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
    public DbSet<FoodCategory> Categories { get; set; }
    public DbSet<Food> Foods { get; set; }
    
    public DbSet<DailyStock> Stocks { get; set; }

    public CafeContext(DbContextOptions<CafeContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        try
        {        
            base.OnModelCreating(modelBuilder);

            EntityTypeBuilder<DailyStock> stockBui = modelBuilder.Entity<DailyStock>();
            

            EntityTypeBuilder<Role> roleBuilder = modelBuilder.Entity<Role>();
            //Adding Default Roles
                 roleBuilder.HasData(Role.Administrator);
                 roleBuilder.HasData(Role.CafetStaff);
                 roleBuilder.HasData(Role.Customer);


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

                 userBuilder
                     .HasIndex(u => u.EmailAddress)
                     .IsUnique();
                 

                     

                 var foodBuilder = modelBuilder.Entity<Food>();
         
                 foodBuilder
                     .Property(f => f.FoodImage)
                     .HasDefaultValue("default.png");
         
         
         
                 //Adding Default Administrator
                 //userBuilder.HasData(User.DefaultAdmin);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}