using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("transaction_log")]
    public class TransactionLog
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("TransactionLog")]
        public User User { get; set; }

        [Column("document_id")]
        public long DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        [InverseProperty("TransactionLog")]
        public Document Document { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("order_id")]
        public long OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("TransactionLog")]
        public UserOrders UserOrders { get; set; }

        [Column("amount", TypeName = "decimal(16,4)")]
        public decimal Amount { get; set; }

        [Column("order_status")]
        public string OrderStatus { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

    }
}
