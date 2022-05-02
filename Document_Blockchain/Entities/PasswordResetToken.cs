using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("password_reset_token")]
    public class PasswordResetToken
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("username")]
        [StringLength(255)]
        public string Username { get; set; }

        [Column("token")]
        [StringLength(255)]
        public string Token { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }


    }
}
