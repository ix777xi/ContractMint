using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("documents")]
    public class Document
    {
        public Document()
        {
            UserCart = new HashSet<UserCart>();
            TransactionLog = new HashSet<TransactionLog>();
            UserDocuments = new HashSet<UserDocuments>();
            EnvolopeDetails = new HashSet<EnvolopeDetails>();
            HashDetails = new HashSet<HashDetails>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("document_name")]
        public string DocumentName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("alternate_name")]
        public string AlternateName { get; set; }

        [Column("document_path")]
        public string DocumentPath { get; set; }

        [Column("upload_date")]
        public DateTime UploadDate { get; set; }

        [Column("price", TypeName = "decimal(16 ,2)")]
        public decimal Price { get; set; }

        [Column("gas_fee")]
        public decimal GasFee { get; set; }

        [Column("category_id")]
        public long Category_Id { get; set; }

        [ForeignKey(nameof(Category_Id))]
        [InverseProperty("Document")]
        public Category Category { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("is_primary")]
        public bool IsPrimary { get; set; }

        [InverseProperty("Document")]
        public virtual ICollection<UserCart> UserCart { get; set; }

        [InverseProperty("Document")]
        public virtual ICollection<TransactionLog> TransactionLog { get; set; }

        [InverseProperty("Document")]
        public virtual ICollection<UserDocuments> UserDocuments { get; set; }

        [InverseProperty("Document")]
        public virtual ICollection<EnvolopeDetails> EnvolopeDetails { get; set; }

        [InverseProperty("Document")]
        public virtual ICollection<HashDetails> HashDetails { get; set; }
    }
}
