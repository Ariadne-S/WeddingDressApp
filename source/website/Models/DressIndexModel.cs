using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class DressIndexModel
    {
        public List<DressItem> Dresses { get; set; }
        public DressType DressType { get; set; }
    }

    public class DressItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
        public string Shop { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string Rating { get; set; }
        public string Approval { get; set; }
        public string RecommendedBy { get; set; }
    }
}
