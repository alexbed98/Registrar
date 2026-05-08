using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Models
{
    public class StudentsRepository : Repository<Student>
    {
        public Dictionary<string, List<Student>> GetAllCohorts()
        {
            Dictionary<string, List<Student>> cohort = new Dictionary<string, List<Student>>();

            var studs = DB.Students.ToList();

            foreach (var stud in studs)
            {
                string year = stud.Code.Substring(0, 4);

                if (!cohort.ContainsKey(year))
                {
                    cohort[year] = new List<Student>();
                }
                cohort[year].Add(stud);
            }
            return cohort;
        }
    }
}