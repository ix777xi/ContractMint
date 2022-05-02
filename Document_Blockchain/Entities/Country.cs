using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("country")]
    public class Country
    {
        public Country()
        {
            User = new HashSet<User>();
        }

        [Column("id")]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("coutry_code")]
        public string CountryCode { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<User> User { get; set; }
    }
}
