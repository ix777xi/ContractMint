using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{

    [Table("subscription_token")]
    public class SubscriptionToken
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("secret_key")]
        public string SecretKey { get; set; }

        [Column("publishKey")]
        public string PublishKey { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
