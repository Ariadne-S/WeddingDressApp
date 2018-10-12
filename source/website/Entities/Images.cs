using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Entities
{
    public class DressImages
    {
        public Guid DressId { get; set; }
        public Guid ImageID { get; set; }
        public bool Favourite { get; set; }
    }
}
