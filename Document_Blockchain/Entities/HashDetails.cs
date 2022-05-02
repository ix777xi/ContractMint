using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("hash_details")]
    public class HashDetails
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("hash")]
        public string Hash { get; set; }

        [Column("blockchain_trx")]
        public string BlockchainTrx { get; set; }

        [Column("ipfs")]
        public string IPFS { get; set; }

        [Column("blockchain_id")]
        public string BlockchainId { get; set; }

        [Column("document_id")]
        public long DocumentId { get; set; }

        [Column("user_document_id")]
        public long? UserDocumentId { get; set; }

        [ForeignKey(nameof(UserDocumentId))]
        [InverseProperty("HashDetails")]
        public UserDocuments UserDocuments { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("HashDetails")]
        public virtual User User { get; set; }

        [ForeignKey(nameof(DocumentId))]
        [InverseProperty("HashDetails")]
        public Document Document { get; set; }
    }
}
