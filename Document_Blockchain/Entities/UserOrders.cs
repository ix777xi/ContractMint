using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("user_order")]
    public class UserOrders
    {
        public UserOrders()
        {
            TransactionLog = new HashSet<TransactionLog>();
            PaymentLedger = new HashSet<PaymentLedger>();
            UserCart = new HashSet<UserCart>();
            UserDocuments = new HashSet<UserDocuments>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserOrders")]
        public User User { get; set; }

        [Column("order_amount", TypeName = "decimal(16,4)")]
        public decimal OrderAmount { get; set; }


        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }

        [Column("order_status")]
        public string OrderSatus { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [InverseProperty("UserOrders")]
        public virtual ICollection<TransactionLog> TransactionLog { get; set; }

        [InverseProperty("UserOrders")]
        public virtual ICollection<UserDocuments> UserDocuments { get; set; }

        [InverseProperty("UserOrders")]
        public virtual ICollection<PaymentLedger> PaymentLedger { get; set; }


        [InverseProperty("UserOrders")]
        public virtual ICollection<UserCart> UserCart { get; set; }

    }
}
