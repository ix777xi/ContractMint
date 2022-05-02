using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("envelope_detail")]
    public class EnvolopeDetails
    {
        public EnvolopeDetails()
        {
            DocumentSignerDetails = new HashSet<DocumentSignerDetails>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("envelope_id")]
        public string EnvelopeId { get; set; }

        [Column("document_id")]
        public long DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        [InverseProperty("EnvolopeDetails")]
        public Document Document { get; set; }

        [Column("user_document_id")]
        public long? UserDocumentId { get; set; }

        [ForeignKey(nameof(UserDocumentId))]
        [InverseProperty("EnvolopeDetails")]
        public UserDocuments UserDocuments { get; set; }

        [Column("sent_by")]
        public long SentBy { get; set; }

        [ForeignKey(nameof(SentBy))]
        [InverseProperty("EnvolopeDetails")]
        public User User { get; set; }

        [Column("mail_status")]
        public string MailStatus { get; set; }

        [Column("error_details")]
        public string ErrorDetails { get; set; }

        [Column("sent_time")]
        public DateTime SentTime { get; set; }

        [Column("signed_document_url")]
        public string SignedDocumentURL { get; set; }

        [InverseProperty("EnvolopeDetails")]
        public virtual ICollection<DocumentSignerDetails> DocumentSignerDetails { get; set; }
    }
}
