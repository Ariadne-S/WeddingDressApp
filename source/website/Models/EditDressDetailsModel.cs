using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class EditDressDetailsModel
    {
            [Required]
            public Guid? DressId { get; set; }
            public string DressWebpage { get; set; }
            public string DressName { get; set; }
            public string Image { get; set; }
            public string Shop { get; set; }
            public string Price { get; set; }
            public string Rating { get; set; }
            public string ProductDescription { get; set; }
            public DressType? DressType { get; set; }
    }
}
