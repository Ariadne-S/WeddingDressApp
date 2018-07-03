using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Entities
{
    public class DressEntity
    {
        public Guid DressId { get; set; }
	    public string DressName { get; set; }
        public string DressWebpage { get; set; }
        public decimal Price { get; set; }
        public string ProductDescription { get; set; }
        public DressType? DressType { get; set; }
        public Guid RecommendedBy { get; set; }
        public DressApproval DressApproval { get; set; }
        public int? Rating { get; set; }
        public Guid ShopId { get; set; }
        public Guid WeddingId { get; set; }
        public Guid ImageId { get; set; }
    }
}
