using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Students : Record
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Code { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
    }
}