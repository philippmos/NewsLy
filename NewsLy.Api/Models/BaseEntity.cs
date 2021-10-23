using Dapper.Contrib.Extensions;

namespace NewsLy.Api.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}