using Jewellis.Models;
using Microsoft.EntityFrameworkCore;

namespace Jewellis.Data
{
    public class JewellisDbContext : DbContext
    {
        public JewellisDbContext(DbContextOptions<JewellisDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserCartProduct> UserCartProducts { get; set; }
        public DbSet<UserWishlistProduct> UserWishlistProducts { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderVsProduct> OrdersVsProducts { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Branch> Branches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region [Users] table

            // Default value on system datetime columns:
            modelBuilder.Entity<User>()
                .Property(e => e.DateRegistered)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<User>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [UserCartProducts] table

            // Sets 2 columns as PK, since it's a connection table:
            modelBuilder.Entity<UserCartProduct>()
                .HasKey(e => new { e.UserId, e.ProductId });

            // Default value on system datetime columns:
            modelBuilder.Entity<UserCartProduct>()
                .Property(e => e.DateAdded)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [UserWishlistProducts] table

            // Sets 2 columns as PK, since it's a connection table:
            modelBuilder.Entity<UserWishlistProduct>()
                .HasKey(e => new { e.UserId, e.ProductId });

            // Default value on system datetime columns:
            modelBuilder.Entity<UserWishlistProduct>()
                .Property(e => e.DateAdded)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [NewsletterSubscribers] table

            // Default value on system datetime columns:
            modelBuilder.Entity<NewsletterSubscriber>()
                .Property(e => e.DateJoined)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [Products] table

            // Default value on system datetime columns:
            modelBuilder.Entity<Product>()
                .Property(e => e.DateAdded)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Product>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [ProductCategories] table

            // Default value on system datetime columns:
            modelBuilder.Entity<ProductCategory>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [ProductTypes] table

            // Default value on system datetime columns:
            modelBuilder.Entity<ProductType>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [Sales] table

            // Default value on system datetime columns:
            modelBuilder.Entity<Sale>()
                .Property(e => e.DateCreated)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Sale>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [Orders] table

            // Since the table contains two FK to the same table, we need to change the delete-cascade setting:
            modelBuilder.Entity<Order>()
                .HasOne(o => o.BillingAddress)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            // Default value on system datetime columns:
            modelBuilder.Entity<Order>()
                .Property(e => e.DateCreated)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [OrdersVsProducts] table

            // Sets 2 columns as PK, since it's a connection table:
            modelBuilder.Entity<OrderVsProduct>()
                .HasKey(e => new { e.OrderId, e.ProductId });

            #endregion

            #region [DeliveryMethods] table

            // Default value on system datetime columns:
            modelBuilder.Entity<DeliveryMethod>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [Addresses] table

            // Default value on system datetime columns:
            modelBuilder.Entity<Address>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [Contacts] table

            // Default value on system datetime columns:
            modelBuilder.Entity<Contact>()
                .Property(e => e.DateCreated)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Contact>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion

            #region [Branches] table

            // Default value on system datetime columns:
            modelBuilder.Entity<Branch>()
                .Property(e => e.DateLastModified)
                .HasDefaultValueSql("GETDATE()");

            #endregion
        }

    }
}
