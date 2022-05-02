using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("category")]
    public class Category
    {
        public Category()
        {
            Document = new HashSet<Document>();
            DocumentRequest = new HashSet<DocumentRequest>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("photo_path")]
        public string PhotoPath { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("is_primary")]
        public bool IsPrimary { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Document> Document { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<DocumentRequest> DocumentRequest { get; set; }
    }
}
