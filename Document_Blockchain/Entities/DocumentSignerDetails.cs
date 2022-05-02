using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("document_signer_details")]
    public class DocumentSignerDetails
    {
        [Key]
        [Column("id")]
        public long id { get; set; }

        [Column("envelope_id")]
        public long EnvelopeId { get; set; }

        [ForeignKey(nameof(EnvelopeId))]
        [InverseProperty("DocumentSignerDetails")]
        public EnvolopeDetails EnvolopeDetails { get; set; }

        [Column("signer_email")]
        public string SignerEmail { get; set; }

        [Column("signer_name")]
        public string SignerName { get; set; }

        [Column("signer_routing")]
        public long SignerRouting { get; set; }

        [Column("signature_status")]
        public string SignatureStatus { get; set; }

        [Column("signature_time")]
        public string SignatureTime { get; set; }
    }
}
