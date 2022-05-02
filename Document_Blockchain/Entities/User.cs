using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("user")]
    public class User
    {
        public User()
        {
            UserRole = new HashSet<UserRole>();
            RefreshToken = new HashSet<RefreshToken>();
            UserOrders = new HashSet<UserOrders>();
            UserCart = new HashSet<UserCart>();
            TransactionLog = new HashSet<TransactionLog>();
            PaymentLedger = new HashSet<PaymentLedger>();
            EnvolopeDetails = new HashSet<EnvolopeDetails>();
            HashDetails = new HashSet<HashDetails>();
            DocumentRequest = new HashSet<DocumentRequest>();
            UserDocuments = new HashSet<UserDocuments>();
            UserAuthentication = new HashSet<UserAuthentication>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("profile_picture")]
        public string ProfilePicture { get; set; }

        [Column("international_dailing_code")]
        public string InternationalDailingCode { get; set; }

        [Column("contact_number")]
        public long ContactNumber { get; set; }

        [Column("email_Id")]
        public string EmailId { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("is_admin")]
        public bool Isadmin { get; set; }

        [Column("country_id")]
        public long? CountryId { get; set; }

        [Column("is_emailverified")]
        public bool? Is_EmailVerified { get; set; }

        [ForeignKey(nameof(CountryId))]
        [InverseProperty("User")]
        public Country Country { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserRole> UserRole { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<RefreshToken> RefreshToken { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserOrders> UserOrders { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserCart> UserCart { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<TransactionLog> TransactionLog { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserDocuments> UserDocuments { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<PaymentLedger> PaymentLedger { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<EnvolopeDetails> EnvolopeDetails { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<HashDetails> HashDetails { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<DocumentRequest> DocumentRequest { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserAuthentication> UserAuthentication { get; set; }
    }
}
