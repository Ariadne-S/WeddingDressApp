using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Entities
{
    public class Weddings
    {
        public Guid WeddingId { get; set; }
        public string EventName { get; set; }
        public DateTimeOffset EventDate { get; set; }
    }
}

//WeddingId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
//SequentialId Int identity NOT NULL,
//EventName VarChar(64) NOT NULL,
//EventDate DateTime2 NOT NULL