using System;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Registrations : Record
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int Year { get; set; }
    }
}