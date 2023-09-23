using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class LocalShipperCPContext : DbContext
    {
        public LocalShipperCPContext()
        {
        }

        public LocalShipperCPContext(DbContextOptions<LocalShipperCPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<History> Histories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<PackageAction> PackageActions { get; set; }
        public virtual DbSet<PackageType> PackageTypes { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Transport> Transports { get; set; }
        public virtual DbSet<TransportType> TransportTypes { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }
        public virtual DbSet<Zone> Zones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DBLocalShipper"));
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email");

                entity.Property(e => e.FcmToken).HasColumnName("fcm_token");

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("fullname");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(225)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(225)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Role");
            });

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.ToTable("Batch");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BatchDescription)
                    .HasColumnType("text")
                    .HasColumnName("batch_description");

                entity.Property(e => e.BatchName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("batch_name");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StoreId).HasColumnName("storeId");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Batch_Store");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.BrandDescription)
                    .HasMaxLength(255)
                    .HasColumnName("brand_description");

                entity.Property(e => e.BrandName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("brand_name");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IconUrl)
                    .HasMaxLength(255)
                    .HasColumnName("iconUrl");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("imageUrl");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Brands)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Brand_Account");
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("History");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("action");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.HistoryDescription)
                    .HasColumnType("text")
                    .HasColumnName("history_description");

                entity.Property(e => e.StoreId).HasColumnName("storeId");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Histories)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_History_Store");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcceptTime)
                    .HasColumnType("datetime")
                    .HasColumnName("accept_time");

                entity.Property(e => e.BatchId).HasColumnName("batchId");

                entity.Property(e => e.CancelTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cancel_time");

                entity.Property(e => e.CancleReason)
                    .HasColumnType("text")
                    .HasColumnName("cancle_reason");

                entity.Property(e => e.CompleteTime)
                    .HasColumnType("datetime")
                    .HasColumnName("complete_time");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DistancePrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("distance_price");

                entity.Property(e => e.OrderTime)
                    .HasColumnType("datetime")
                    .HasColumnName("order_time");

                entity.Property(e => e.Other)
                    .HasColumnType("text")
                    .HasColumnName("other");

                entity.Property(e => e.PickupTime)
                    .HasColumnType("datetime")
                    .HasColumnName("pickup_time");

                entity.Property(e => e.ShipperId).HasColumnName("shipperId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("storeId");

                entity.Property(e => e.SubtotalPrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("subtotal_price");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_price");

                entity.Property(e => e.TrackingNumber)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("tracking_number");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Batch");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Shipper");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Store");
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.ToTable("Package");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionId).HasColumnName("actionId");

                entity.Property(e => e.BatchId).HasColumnName("batchId");

                entity.Property(e => e.CancelReason)
                    .HasMaxLength(255)
                    .HasColumnName("cancel_reason");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.CustomerAddress)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("customer_address");

                entity.Property(e => e.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("customer_email");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("customer_name");

                entity.Property(e => e.CustomerPhone)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("customer_phone");

                entity.Property(e => e.DistancePrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("distance_price");

                entity.Property(e => e.PackageHeight).HasColumnName("package_height");

                entity.Property(e => e.PackageLength).HasColumnName("package_length");

                entity.Property(e => e.PackageWeight).HasColumnName("package_weight");

                entity.Property(e => e.PackageWidth).HasColumnName("package_width");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SubtotalPrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("subtotal_price");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_price");

                entity.Property(e => e.TypeId).HasColumnName("typeId");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.Packages)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Package_PackageAction");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.Packages)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Package_Batch");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Packages)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Package_PackageType");
            });

            modelBuilder.Entity<PackageAction>(entity =>
            {
                entity.ToTable("PackageAction");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionType)
                    .HasMaxLength(255)
                    .HasColumnName("actionType");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createdAt")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<PackageType>(entity =>
            {
                entity.ToTable("PackageType");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createdAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PackageType1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("package_type");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.PackageId).HasColumnName("packageId");

                entity.Property(e => e.PaymentCode)
                    .HasMaxLength(50)
                    .HasColumnName("payment_code");

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("payment_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentImage).HasColumnName("payment_image");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasColumnName("payment_method");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PackageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Package");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Rating");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ByStoreId).HasColumnName("by_storeId");

                entity.Property(e => e.Comment)
                    .HasColumnType("text")
                    .HasColumnName("comment");

                entity.Property(e => e.RatingTime)
                    .HasColumnType("datetime")
                    .HasColumnName("rating_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RatingValue).HasColumnName("rating_value");

                entity.Property(e => e.ShipperId).HasColumnName("shipperId");

                entity.HasOne(d => d.ByStore)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.ByStoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_Store");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.ShipperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_Shipper");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.ToTable("Shipper");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.AdressShipper)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("adress_shipper");

                entity.Property(e => e.EmailShipper)
                    .HasMaxLength(100)
                    .HasColumnName("email_shipper");

                entity.Property(e => e.Fcmtoken)
                    .HasMaxLength(255)
                    .HasColumnName("fcmtoken");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.PhoneShipper)
                    .HasMaxLength(50)
                    .HasColumnName("phone_shipper");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TransportId).HasColumnName("transportId");

                entity.Property(e => e.WalletId).HasColumnName("walletId");

                entity.Property(e => e.ZoneId).HasColumnName("zoneId");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shipper_Account");

                entity.HasOne(d => d.Transport)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.TransportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shipper_Transport");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shipper_Wallet");

                entity.HasOne(d => d.Zone)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.ZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shipper_Zone");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.BrandId).HasColumnName("brandId");

                entity.Property(e => e.CloseTime).HasColumnName("close_time");

                entity.Property(e => e.OpenTime).HasColumnName("open_time");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("store_address");

                entity.Property(e => e.StoreDescription)
                    .HasColumnType("text")
                    .HasColumnName("store_description");

                entity.Property(e => e.StoreEmail)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("store_email");

                entity.Property(e => e.StoreName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("store_name");

                entity.Property(e => e.StorePhone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("store_phone");

                entity.Property(e => e.TemplateId).HasColumnName("templateId");

                entity.Property(e => e.WalletId).HasColumnName("walletId");

                entity.Property(e => e.ZoneId).HasColumnName("zoneId");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_Account");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_Brand");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("FK_Store_Template");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_Wallet");

                entity.HasOne(d => d.Zone)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.ZoneId)
                    .HasConstraintName("FK_Store_Zone");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("Template");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.TemplateName)
                    .HasMaxLength(100)
                    .HasColumnName("template_name");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OrderId).HasColumnName("orderId");

                entity.Property(e => e.TransactionDescription)
                    .HasColumnType("text")
                    .HasColumnName("transaction_description");

                entity.Property(e => e.TransactionMethod)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("transaction_method");

                entity.Property(e => e.TransactionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WalletId).HasColumnName("walletId");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Order");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Wallet");
            });

            modelBuilder.Entity<Transport>(entity =>
            {
                entity.ToTable("Transport");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LicencePlate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("licence_plate");

                entity.Property(e => e.TransportColor)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("transport_color");

                entity.Property(e => e.TransportImage)
                    .HasMaxLength(255)
                    .HasColumnName("transport_image");

                entity.Property(e => e.TransportRegistration)
                    .HasMaxLength(255)
                    .HasColumnName("transport_registration");

                entity.Property(e => e.TypeId).HasColumnName("typeId");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Transports)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transport_TransportType");
            });

            modelBuilder.Entity<TransportType>(entity =>
            {
                entity.ToTable("TransportType");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransportType1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("transport_type");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("balance")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<WalletTransaction>(entity =>
            {
                entity.ToTable("WalletTransaction");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FromWalletId).HasColumnName("from_walletId");

                entity.Property(e => e.ToWalletId).HasColumnName("to_walletId");

                entity.Property(e => e.TransactionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("transaction_type");

                entity.HasOne(d => d.FromWallet)
                    .WithMany(p => p.WalletTransactionFromWallets)
                    .HasForeignKey(d => d.FromWalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WalletTransaction_Wallet");

                entity.HasOne(d => d.ToWallet)
                    .WithMany(p => p.WalletTransactionToWallets)
                    .HasForeignKey(d => d.ToWalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WalletTransaction_Wallet1");
            });

            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("Zone");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Latitude)
                    .HasColumnType("decimal(10, 8)")
                    .HasColumnName("latitude");

                entity.Property(e => e.Longitude)
                    .HasColumnType("decimal(11, 8)")
                    .HasColumnName("longitude");

                entity.Property(e => e.Radius)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("radius");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ZoneDescription)
                    .HasColumnType("text")
                    .HasColumnName("zone_description");

                entity.Property(e => e.ZoneName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("zone_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
