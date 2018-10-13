using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{

    public class DressDetailsModel
    {
        public Guid? DressId { get; set; }
        public string DressWebpage { get; set; }    
        public string Name { get; set; }
        public Guid ImageId { get; set; }
        public Guid Shop { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string Approval { get; set; }
        public int? Rating { get; set; }
        public Guid CreatedBy { get; set; }
        public List<string> Comments { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset DateComment { get; set; }
        public string NewComment { get; set; }
        public DressType DressType { get; set; }
    }
    
}
