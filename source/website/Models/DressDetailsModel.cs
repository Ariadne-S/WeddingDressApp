using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{

    public class DressDetailsModel
    {
        public string DressWebpage { get; set; }    
        public string Name { get; set; }
        public string Image { get; set; }
        public string Shop { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string Approval { get; set; }
        public string Rating { get; set; }
        public string Recommendation { get; set; }
        public List<string> Comments { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset DateComment { get; set; }
        public string NewComment { get; set; }
        public DressType DressType { get; set; }
    }
    
}
