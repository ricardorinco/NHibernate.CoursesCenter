using System;

namespace Persistence.POCO
{
    public class Report
    {
        // CourseType
        public virtual string CourseTypeIdentification { get; set; }

        // Course
        public virtual int CourseBestSeller { get; set; }
        public virtual string CourseIdentification { get; set; }

        // Student
        public virtual int StudentRegistration { get; set; }
        public virtual string StudentIdentification { get; set; }

        // Instructor
        public virtual string InstructorIdentification { get; set; }

        // Request
        public virtual int RequestId { get; set; }
        public virtual DateTime RequestDateTime { get; set; }

        // Total
        public virtual int TotalCourseMinistered { get; set; }
        public virtual int TotalCourseRequest { get; set; }
        public virtual decimal TotalCoursePrice { get; set; }
    }
}
