using EShop.Models.EFModels.Attribute;
using EShop.Models.EFModels.Category;
using EShop.Models.EFModels.Order;
using EShop.Models.EFModels.Product;
using EShop.Models.EFModels.ShoppingCart;
using EShop.Models.EFModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<CardInfo> CardInfo { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<ShoppingCartProduct> ShoppingCartProduct { get; set; }
        public DbSet<ProductProperty> ProductProperty { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<CategoryCategory> CategoryCategory { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddress { get; set; }
        public DbSet<ProductDiscount> ProductDiscount { get; set; }
        public DbSet<ProductAd> ProductAd { get; set; }
        public DbSet<OrderReview> OrderReview { get; set; }
        public DbSet<AttributeValue> AttributeValue { get; set; }
        public DbSet<Attribute> Attribute { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValue { get; set; }
        public DbSet<ShoppingCartProductHistory> ShoppingCartProductHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Renaming default ASP.NET Identity tables (AspnetUsers to Users, etc.)
            builder.Entity<ApplicationUser>(entity => { entity.ToTable("Users"); });
            builder.Entity<IdentityRole>(entity => { entity.ToTable("Roles"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });

            builder.Entity<Attribute>()
                .HasIndex(c => c.Name)
                .IsUnique();

            builder.Entity<AttributeValue>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // unique name for product
            builder.Entity<Product>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // unique name for category
            builder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // many-to-many mapping
            builder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);

            // many-to-many mapping
            builder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            builder.Entity<ProductDiscount>()
                .HasAlternateKey(c => c.ProductId)
                .HasName("AlternateKey_ProductId");

            builder.Entity<ProductAttributeValue>()
                .HasAlternateKey(c => new {c.ProductId, c.AttributeValueId})
                .HasName("AlternateKey_ProductId_AttributeValueId");

            //one-to-one mapping (unique)
            builder.Entity<ProductDiscount>()
                .HasOne(pd => pd.Product)
                .WithOne(p => p.ProductDiscount)
                .HasForeignKey<ProductDiscount>(pd => pd.ProductId);

            builder.Entity<Product>()
                .HasOne(p => p.ProductDiscount)
                .WithOne(pd => pd.Product);

            //one-to-one mapping (unique)
            builder.Entity<ProductAd>()
                .HasOne(pa => pa.Product)
                .WithOne(p => p.ProductAd)
                .HasForeignKey<ProductAd>(pa => pa.ProductId);

            // many-to-many mapping
            builder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId);

            // many-to-many mapping
            builder.Entity<ProductProperty>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.ProductProperies)
                .HasForeignKey(pp => pp.ProductId);

            //one-to-one mapping (unique)
            builder.Entity<Category>()
                .HasOne(c => c.CategoryCategory)
                .WithOne(cc => cc.Category);

            // many-to-many mapping
            builder.Entity<ProductAttributeValue>()
                .HasOne(pav => pav.Product)
                .WithMany(p => p.ProductAttributeValues)
                .HasForeignKey(pav => pav.ProductId);
        }
    }
}