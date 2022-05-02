using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Blockchain.Entities
{
    [Table("docusign_accesstoken")]
    public class DocusignAccessToken
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("accesstoken")]
        public string AccessToken { get; set; }

        [Column("token_type")]
        public string TokenType { get; set; }

        [Column("created_time")]
        public DateTime CreatedTime { get; set; }

        [Column("expire_time")]
        public DateTime ExpaireTime { get; set; }
    }
}
