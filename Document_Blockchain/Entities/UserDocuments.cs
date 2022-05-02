using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("user_documents")]
    public class UserDocuments
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserDocuments")]
        public User User { get; set; }

        [Column("document_id")]
        public long DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        [InverseProperty("UserDocuments")]
        public Document Document { get; set; }

        [Column("alternate_name")]
        public string AlternateName { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("remaining_quantity")]
        public int RemainingQuantity { get; set; }

        [Column("order_id")]
        public long OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("UserDocuments")]
        public UserOrders UserOrders { get; set; }

        [Column("is_edited")]
        public bool? IsEdited { get; set; }

        [Column("edited_document")]
        public string EditedDocument { get; set; }

        [Column("is_recent")]
        public bool? IsRecent { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }

        [InverseProperty("UserDocuments")]
        public virtual ICollection<EnvolopeDetails> EnvolopeDetails { get; set; }

        [InverseProperty("UserDocuments")]
        public virtual ICollection<HashDetails> HashDetails { get; set; }
    }
}
