using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("user_authentication")]
    public class UserAuthentication
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserAuthentication")]
        public User User { get; set; }

        [Column("otp")]
        public string OTP { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("expire_time")]
        public DateTime ExpireTime { get; set; }

    }
}
