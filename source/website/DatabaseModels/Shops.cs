using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.DatabaseModels
{
    public class Shops
    {
        public Guid ShopID { get; set; }
        public string ShopName { get; set; }
        public string ShopWebsite { get; set; }
        public string ShopLocation { get; set; }
        public Guid ImageId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
