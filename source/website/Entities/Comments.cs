using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Entities
{
    public class Comments
    {
        public Guid CommentID { get; set; }
        public Guid UserId { get; set; }
        public Guid DressId { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CommentDate { get; set; }
    }
}
