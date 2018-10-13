using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.DatabaseModels
{
    public class Users
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsEmailValidated { get; set; }
    }
}


//Username VarChar(64) NOT NULL,
// Email VarChar(150) NOT NULL,
//IsEmailValidated bit NOT NULL