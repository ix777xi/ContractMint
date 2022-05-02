using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("document_requests")]
    public class DocumentRequest
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("DocumentRequest")]
        public User User { get; set; }

        [Column("document_description")]
        public string DocumentDescription { get; set; }

        [Column("category_id")]
        public long? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("DocumentRequest")]
        public Category Category { get; set; }

        [Column("status")]
        public long Status { get; set; }

        [ForeignKey(nameof(Status))]
        [InverseProperty("DocumentRequest")]
        public RequestStatusMaster RequestStatusMaster { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; }
    }
}
