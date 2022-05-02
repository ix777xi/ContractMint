using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("refresh_token")]
    public class RefreshToken
    {

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("expires")]
        public DateTime Expires { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("RefreshToken")]
        public virtual User User { get; set; }
    }
}

