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
        public virtual DbSet<History> Histories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderHistory> OrderHistories { get; set; }
        public virtual DbSet<PackageAction> PackageActions { get; set; }
        public virtual DbSet<PackageType> PackageTypes { get; set; }
        public virtual DbSet<PriceInZone> PriceInZones { get; set; }
        public virtual DbSet<PriceItem> PriceItems { get; set; }
        public virtual DbSet<PriceL> PriceLs { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RouteEdge> RouteEdges { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
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
                    .HasMaxLength(255)
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

                entity.Property(e => e.ActionId).HasColumnName("actionId");

                entity.Property(e => e.CancelReason)
                    .HasMaxLength(255)
                    .HasColumnName("cancel_reason");

                entity.Property(e => e.CancelTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cancel_time");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.Cod)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("COD");

                entity.Property(e => e.CompleteTime)
                    .HasColumnType("datetime")
                    .HasColumnName("complete_time");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerCity)
                    .HasMaxLength(255)
                    .HasColumnName("customer_city");

                entity.Property(e => e.CustomerCommune)
                    .HasMaxLength(255)
                    .HasColumnName("customer_commune");

                entity.Property(e => e.CustomerDistrict)
                    .HasMaxLength(255)
                    .HasColumnName("customer_district");
                entity.Property(e => e.CustomerLat)
                    .HasColumnType("float")
                    .HasColumnName("customerLat");

                entity.Property(e => e.CustomerLng)
                    .HasColumnType("float")
                    .HasColumnName("customerLng");
                    

                entity.Property(e => e.CustomerEmail)
                    .HasMaxLength(100)
                    .HasColumnName("customer_email");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(100)
                    .HasColumnName("customer_name");

                entity.Property(e => e.CustomerPhone)
                    .HasMaxLength(20)
                    .HasColumnName("customer_phone");

                entity.Property(e => e.Distance)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("distance");

                entity.Property(e => e.DistancePrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("distance_price");
                entity.Property(e => e.Eta).HasColumnName("ETA");

                entity.Property(e => e.OrderTime)
                    .HasColumnType("datetime")
                    .HasColumnName("order_time");

                entity.Property(e => e.Other)
                    .HasMaxLength(255)
                    .HasColumnName("other");

                entity.Property(e => e.PackageHeight).HasColumnName("package_height");

                entity.Property(e => e.PackageLength).HasColumnName("package_length");

                entity.Property(e => e.PackageWeight).HasColumnName("package_weight");

                entity.Property(e => e.PackageWidth).HasColumnName("package_width");

                entity.Property(e => e.PickupTime)
                    .HasColumnType("datetime")
                    .HasColumnName("pickup_time");

                entity.Property(e => e.RouteId).HasColumnName("routeId");

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

                entity.Property(e => e.TypeId).HasColumnName("typeId");
                entity.Property(e => e.Evidence)
                    .HasMaxLength(255)
                    .HasColumnName("evidence");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_PackageAction");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.RouteId)
                    .HasConstraintName("FK_Order_RouteEdge");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipperId)
                    .HasConstraintName("FK_Order_Shipper");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Store");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_PackageType");


            });

            modelBuilder.Entity<OrderHistory>(entity =>
            {
                entity.ToTable("OrderHistory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChangeDate)
                    .HasColumnType("datetime")
                    .HasColumnName("change_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FromStatus).HasColumnName("from_status");

                entity.Property(e => e.OrderId).HasColumnName("orderId");

                entity.Property(e => e.ShipperId).HasColumnName("shipperId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.ToStatus).HasColumnName("to_status");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderHistories)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderHistory_Order");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.OrderHistories)
                    .HasForeignKey(d => d.ShipperId)
                    .HasConstraintName("FK_OrderHistory_Shipper");
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

                entity.Property(e => e.Deleted).HasColumnName("deleted");
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

            modelBuilder.Entity<PriceInZone>(entity =>
            {
                entity.ToTable("PriceInZone");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PriceId).HasColumnName("priceId");

                entity.Property(e => e.ZoneId).HasColumnName("zoneId");

                entity.HasOne(d => d.Price)
                    .WithMany(p => p.PriceInZones)
                    .HasForeignKey(d => d.PriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceInZone_Price");

                entity.HasOne(d => d.Zone)
                    .WithMany(p => p.PriceInZones)
                    .HasForeignKey(d => d.ZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceInZone_Zone");
            });

            modelBuilder.Entity<PriceItem>(entity =>
            {
                entity.ToTable("PriceItem");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApplyFrom)
                    .HasColumnType("date")
                    .HasColumnName("apply_from");

                entity.Property(e => e.ApplyTo)
                    .HasColumnType("date")
                    .HasColumnName("apply_to");

                entity.Property(e => e.MaxAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("max_amount");

                entity.Property(e => e.MaxDistance).HasColumnName("max_distance");

                entity.Property(e => e.MinAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("min_amount");

                entity.Property(e => e.MinDistance).HasColumnName("min_distance");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.PriceId).HasColumnName("priceId");

                entity.HasOne(d => d.PriceNavigation)
                    .WithMany(p => p.PriceItems)
                    .HasForeignKey(d => d.PriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PriceItem_Price");
            });

            modelBuilder.Entity<PriceL>(entity =>
            {
                entity.ToTable("PriceLS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("date")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Datefilter).HasColumnName("datefilter");

                entity.Property(e => e.Hourfilter).HasColumnName("hourfilter");

                entity.Property(e => e.Mode).HasColumnName("mode");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("storeId");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.PriceLs)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_Price_Store");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Rating");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ByStoreId).HasColumnName("by_storeId");

                entity.Property(e => e.Comment)
                    .HasMaxLength(255)
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

            modelBuilder.Entity<RouteEdge>(entity =>
            {
                entity.ToTable("RouteEdge");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Eta)
                    .HasColumnType("datetime")
                    .HasColumnName("ETA");

                entity.Property(e => e.FromStation).HasColumnName("from_station");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Progress).HasColumnName("progress");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.ShipperId).HasColumnName("shipperId");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.ToStation).HasColumnName("to_station");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.RouteEdges)
                    .HasForeignKey(d => d.ShipperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RouteEdge_Shipper");

            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.ToTable("Shipper");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.AddressShipper)
                    .HasMaxLength(100)
                    .HasColumnName("address_shipper");

                entity.Property(e => e.EmailShipper)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("email_shipper");

                entity.Property(e => e.Fcmtoken)
                    .HasMaxLength(255)
                    .HasColumnName("fcmtoken");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.PhoneShipper)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("phone_shipper");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("storeId");

                entity.Property(e => e.TransportId).HasColumnName("transportId");

                entity.Property(e => e.Type).HasColumnName("type");



                entity.Property(e => e.ZoneId).HasColumnName("zoneId");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shipper_Account");

                entity.HasOne(d => d.Transport)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.TransportId)
                    .HasConstraintName("FK_Shipper_Transport");



                entity.HasOne(d => d.Zone)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.ZoneId)
                    .HasConstraintName("FK_Shipper_Zone");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.CloseTime).HasColumnName("close_time");

                entity.Property(e => e.OpenTime).HasColumnName("open_time");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("store_address");

                entity.Property(e => e.StoreDescription)
                    .HasMaxLength(255)
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

                entity.Property(e => e.StoreLat)
                    .HasColumnName("storeLat")
                    .HasColumnType("float");

                entity.Property(e => e.StoreLng)
                    .HasColumnName("storeLng")
                    .HasColumnType("float");

                entity.Property(e => e.TimeDelivery)
                   .HasColumnType("int")
                   .HasColumnName("timeDelivery");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Store_Account");

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

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("imageUrl");

                entity.Property(e => e.TemplateName)
                    .HasMaxLength(100)
                    .HasColumnName("template_name");
            });

            modelBuilder.Entity<Transport>(entity =>
            {
                entity.ToTable("Transport");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

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
                entity.Property(e => e.Type).HasColumnName("type");
                entity.Property(e => e.ShipperId).HasColumnName("shipperId");

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

                entity.HasOne(d => d.Shipper)
                   .WithMany(p => p.Wallet)
                   .HasForeignKey(d => d.ShipperId)
                   .HasConstraintName("FK_Wallet_Shipper");
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

                entity.Property(e => e.OrderId).HasColumnName("orderId");

                entity.Property(e => e.ToWalletId).HasColumnName("to_walletId");
               

                entity.Property(e => e.TransactionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("transaction_type");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.HasOne(d => d.FromWallet)
                    .WithMany(p => p.WalletTransactionFromWallets)
                    .HasForeignKey(d => d.FromWalletId)
                    .HasConstraintName("FK_WalletTransaction_Wallet");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_WalletTransaction_Order");

                entity.HasOne(d => d.ToWallet)
                    .WithMany(p => p.WalletTransactionToWallets)
                    .HasForeignKey(d => d.ToWalletId)
                    .HasConstraintName("FK_WalletTransaction_Wallet1");
            });

            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("Zone");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

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

                entity.Property(e => e.PriceInZoneId).HasColumnName("priceInZoneId");

                entity.Property(e => e.Radius)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("radius");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ZoneDescription)
                    .HasMaxLength(255)
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
