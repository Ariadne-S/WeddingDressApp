using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Website.Models
{
    public class SaveDressModel
    {
        [Required, MaxLength(200)]
        public string Url { get; set; }
        [Required, MaxLength(30)]
        public string Name { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        public Guid? Shop { get; set; }
        public decimal? Price { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [Required]
        public DressType? DressType { get; set; }
    }
}