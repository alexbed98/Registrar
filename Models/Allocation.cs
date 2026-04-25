using System;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Allocation : Record
    {
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
        public string Year { get; set; }
    }
}