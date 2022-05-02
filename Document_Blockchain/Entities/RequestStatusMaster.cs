using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("request_status_master")]
    public class RequestStatusMaster
    {
        public RequestStatusMaster()
        {
            DocumentRequest = new HashSet<DocumentRequest>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [InverseProperty("RequestStatusMaster")]
        public virtual ICollection<DocumentRequest> DocumentRequest { get; set; }
    }
}
