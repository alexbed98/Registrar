using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class AllocationsRepository : Repository<Allocation>
    {
        public Teacher GetTeacherByAlloc(string year, int courseId)
        {
            var allocations = DB.Allocations.ToList();

            foreach (var alloc in allocations)
            {
                if (alloc.CourseId == courseId && alloc.Year == int.Parse(year))
                {
                    var teacher = DB.Teachers.Get(alloc.TeacherId);
                    return teacher;
                }
            }

            return null;
        }
    }
}