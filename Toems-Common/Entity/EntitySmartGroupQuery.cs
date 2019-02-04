using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("smart_group_queries")]
    public class EntitySmartGroupQuery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("smart_group_query_id")]
        public int Id { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("order")]
        public int Order { get; set; }

        [Column("and_or")]
        public string AndOr { get; set; }

        [Column("left_parenthesis")]
        public string LeftParenthesis { get; set; }

        [Column("query_table")]
        public string Table { get; set; }

        [Column("query_field")]
        public string Field { get; set; }

        [Column("operator")]
        public string Operator { get; set; }

        [Column("value")]
        public string Value { get; set; }

        [Column("right_parenthesis")]
        public string RightParenthesis { get; set; }
    }
}