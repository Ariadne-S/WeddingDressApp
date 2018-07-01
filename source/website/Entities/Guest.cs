using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Entities
{
    public class Guest
    {
        public Guid GuestId { get; set; }
        public Guid UserId { get; set; }
        public Guid WeddingId {get; set;}
        public GuestType GuestType { get; set; }
    }
}



//	GuestId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
//  SequentialId Int identity NOT NULL,
//  UserId UniqueIdentifier NOT NULL,
//  WeddingId UniqueIdentifier NOT NULL,
//	GuestType VarChar(20)