using System;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.EnterpriseServices.Internal;

namespace Models
{
    public class Teachers: Record
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public string Avatar { get; set; }
    }
}