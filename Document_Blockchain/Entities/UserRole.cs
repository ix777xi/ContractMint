using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("user_role")]
    public class UserRole
    {

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserRole")]
        public virtual User User { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty("UserRole")]
        public virtual Role Role { get; set; }
    }
}
