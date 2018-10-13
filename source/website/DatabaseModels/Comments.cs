using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.DatabaseModels
{
    public class Comments
    {
        public Guid CommentID { get; set; }
        public Guid DressId { get; set; }
        public string Comment { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public Boolean Deleted { get; set; }
        public DateTimeOffset DeletedAt { get; set; }
    }
}
