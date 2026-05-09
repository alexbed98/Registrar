using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class Teacher: Record
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }

        [JsonIgnore] public string FullName => LastName + " " + FirstName;

        [JsonIgnore] public List<Allocation> Allocations => DB.Allocations.ToList().Where(r => r.TeacherId == Id).ToList();

        [JsonIgnore] public List<Allocation> NextSessionAllocations => DB.Allocations.ToList().Where(r => r.TeacherId == Id && r.IsNextSession).ToList();

        [JsonIgnore]
        public List<Course> Courses
        {
            get
            {
                var courses = new List<Course>();
                foreach (var allocation in Allocations.OrderBy(r => r.Course.Code))
                {
                    courses.Add(allocation.Course);
                }
                return courses;
            }
        }

        [JsonIgnore]
        public List<Course> NextSessionCourses
        {
            get
            {
                var courses = new List<Course>();
                foreach (var allocation in NextSessionAllocations.OrderBy(r => r.Course.Code))
                {
                    courses.Add(allocation.Course);
                }
                return courses;
            }
        }

        public void DeleteAllAllocations()
        {
            foreach (Allocation allocation in Allocations)
                DB.Allocations.Delete(allocation.Id);
        }

        public void DeleteNextSessionAllocations()
        {
            foreach (Allocation allocation in NextSessionAllocations)
                DB.Allocations.Delete(allocation.Id);
        }

        public void UpdateAllocations(List<int> selectedCoursesId)
        {
            DeleteNextSessionAllocations();
            if (selectedCoursesId != null)
                foreach (int courseId in selectedCoursesId)
                {
                    DB.Allocations.Add(new Allocation { TeacherId = Id, CourseId = courseId });
                }
        }
    }
}