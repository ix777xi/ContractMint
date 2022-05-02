using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("user_cart")]
    public class UserCart
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserCart")]
        public User User { get; set; }

        [Column("document_id")]
        public long DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        [InverseProperty("UserCart")]
        public Document Document { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("order_status")]
        public string OrderStatus { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }


        [Column("user_order_id")]
        public long? UserOrderId { get; set; }

        [ForeignKey(nameof(UserOrderId))]
        [InverseProperty("UserCart")]
        public UserOrders UserOrders { get; set; }

    }
}
