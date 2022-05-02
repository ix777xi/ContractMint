using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("payment_ledger")]
    public class PaymentLedger
    {

        [Key]
        [Column("id", TypeName = "bigint(20)")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(16 ,4)")]
        public decimal Amount { get; set; }

        [Column("transaction_status")]
        public string TransactionStatus { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("last_updated_date")]
        public DateTime LastUpdateDate { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("PaymentLedger")]
        public virtual User User { get; set; }

        [Column("order_id")]
        public long OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("PaymentLedger")]
        public virtual UserOrders UserOrders { get; set; }


    }
}
