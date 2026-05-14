using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class Course : Record
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int Session { get; set; }

        [JsonIgnore] public string Caption => "[" + Session + "] " + Code + " " + Title;

        [JsonIgnore] public List<Registration> Registrations => DB.Registrations.ToList().Where(r => r.CourseId == Id).ToList();

        [JsonIgnore] public List<Allocation> Allocations => DB.Allocations.ToList().Where(r => r.CourseId == Id).ToList();

        [JsonIgnore]
        public List<Student> Students
        {
            get
            {
                var students = new List<Student>();
                foreach (var registration in Registrations.OrderBy(r => r.Student.Code))
                {
                    students.Add(registration.Student);
                }
                return students;
            }
        }

        [JsonIgnore]
        public List<Teacher> Teachers
        {
            get
            {
                var teachers = new List<Teacher>();
                foreach (var allocation in Allocations.OrderBy(r => r.Teacher.Code))
                {
                    teachers.Add(allocation.Teacher);
                }
                return teachers;
            }
        }

        [JsonIgnore] public bool IsNextSession => NextSession.ValidSessions.Contains(Session);

        [JsonIgnore] public List<Course> NextSessionCourses => DB.Courses.ToList().Where(r => r.Session == Session && r.IsNextSession).ToList();

        [JsonIgnore]
        public SelectList NextSessionCoursesToSelectList => SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");

        [JsonIgnore] public List<Registration> NextSessionRegistrations => DB.Registrations.ToList().Where(r => r.CourseId == Id && r.IsNextSession).ToList();

        [JsonIgnore]
        public List<Student> NextSessionStudents
        {
            get
            {
                var students = new List<Student>();
                foreach (var registration in Registrations
                    .Where(r => r.IsNextSession)
                    .OrderBy(r => r.Student.Code))
                {
                    students.Add(registration.Student);
                }
                return students;
            }
        }

        [JsonIgnore]
        public SelectList NextSessionStudentsToSelectList => SelectListUtilities<Student>.Convert(NextSessionStudents, "Caption");

        public void DeleteAllRegistrations()
        {
            foreach (Registration registration in Registrations)
                DB.Registrations.Delete(registration.Id);
        }

        public void DeleteAllAllocations()
        {
            foreach (Allocation allocation in Allocations)
                DB.Allocations.Delete(allocation.Id);
        }

        public void DeleteNextSessionRegistrations()
        {
            foreach (Registration registration in NextSessionRegistrations)
                DB.Registrations.Delete(registration.Id);
        }

        public void UpdateRegistrations(List<int> selectedStudentsId)
        {
            DeleteNextSessionRegistrations();
            if (selectedStudentsId != null)
                foreach (int studentId in selectedStudentsId)
                {
                    DB.Registrations.Add(new Registration { StudentId = studentId, CourseId = Id });
                }
        }
    }
}