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
    public DbSet<WalletHistory> WalletHistories { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderItems> OrderItems { get; set; }
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

                 //userBuilder.HasData(User.DefaultAdmin);
                     

                 var foodBuilder = modelBuilder.Entity<Food>();
         
                 foodBuilder
                     .Property(f => f.FoodImage)
                     .HasDefaultValue("default.png");

                 foodBuilder
                     .Property(f => f.Vegetarian)
                     .HasDefaultValue(false);

                 /*foodBuilder
                     .Property(f => f.Tags)
                     .HasConversion(
                         toSql => JsonConvert.SerializeObject(toSql),
                         fromSql => JsonConvert.DeserializeObject<List<string>>(fromSql) ?? new List<string>()
                     );*/

                 EntityTypeBuilder<Order> orderBuilder = modelBuilder.Entity<Order>();
                 
                 orderBuilder.HasOne(o => o.OrderPlacedBy)
                     .WithMany()
                     .OnDelete(DeleteBehavior.NoAction);

                 orderBuilder.HasOne(o => o.OrderPlacedFor)
                     .WithMany()
                     .OnDelete(DeleteBehavior.Cascade);

                 orderBuilder
                     .Property(o => o.Cancelled)
                     .HasDefaultValue(false);

                 EntityTypeBuilder<WalletHistory> walletBuilder = modelBuilder.Entity<WalletHistory>();
                 
                 
                 walletBuilder.HasOne(o => o.Recipient)
                     .WithMany()
                     .HasForeignKey(x => x.RecipientId)
                     .OnDelete(DeleteBehavior.ClientSetNull);
                 
                 walletBuilder.HasOne(o => o.Sender)
                     .WithMany()
                     .HasForeignKey(x => x.SenderId)
                     .OnDelete(DeleteBehavior.ClientSetNull);

                 EntityTypeBuilder<Cart> cartBuilder = modelBuilder.Entity<Cart>();

                 cartBuilder
                     .HasMany<UserCartData>(ud => ud.FoodCartData)
                     .WithOne(c => c.Cart)
                     .HasForeignKey(s => s.CartId);

                 EntityTypeBuilder<UserCartData> cartDataBuilder = modelBuilder.Entity<UserCartData>();
                 cartDataBuilder
                     .HasOne<Food>(c => c.Food)
                     .WithMany()
                     .OnDelete(DeleteBehavior.ClientSetNull);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}