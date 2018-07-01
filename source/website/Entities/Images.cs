using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Entities
{
    public class Images
    {
        public Guid ImageID { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileData { get; set; }
        public string Hash { get; set; }
        public bool ImageFavourite { get; set; }
            }
}
