using System.Collections.Generic;

namespace Persistence.POCO
{
    public class Instructor
    {
        public Instructor()
        {
            Courses = new List<Course>();
        }

        public virtual int Id { get; set; }
        public virtual string Identification { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}