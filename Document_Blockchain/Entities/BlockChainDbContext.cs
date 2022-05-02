using Microsoft.EntityFrameworkCore;
using System;

namespace Document_Blockchain.Entities
{
    public partial class BlockChainDbContext : DbContext
    {
        public string ConnectionString;

        public BlockChainDbContext(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public BlockChainDbContext(DbContextOptions<BlockChainDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<PasswordResetToken> PasswordResetToken { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<UserOrders> UserOrders { get; set; }
        public virtual DbSet<UserCart> UserCart { get; set; }
        public virtual DbSet<UserDocuments> UserDocuments { get; set; }
        public virtual DbSet<TransactionLog> TransactionLog { get; set; }
        public virtual DbSet<EnvolopeDetails> EnvolopeDetails { get; set; }
        public virtual DbSet<PaymentLedger> PaymentLedger { get; set; }
        public virtual DbSet<DocumentSignerDetails> DocumentSignerDetails { get; set; }
        public virtual DbSet<Country> Country { get; set; }

        public virtual DbSet<DocusignAccessToken> DocusignAccessToken { get; set; }

        public virtual DbSet<SubscriptionToken> SubscriptionToken { get; set; }

        public virtual DbSet<DocumentRequest> DocumentRequest { get; set; }

        public virtual DbSet<RequestStatusMaster> RequestStatusMaster { get; set; }

        public virtual DbSet<UserAuthentication> UserAuthentications { get; set; }

        public virtual DbSet<HashDetails> HashDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
                optionsBuilder.UseMySql("ConnectionStrings : DefaultConnection", serverVersion);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
