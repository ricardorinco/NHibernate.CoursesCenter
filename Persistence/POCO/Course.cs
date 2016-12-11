namespace Persistence.POCO
{
    public class Course
    {
        public virtual int Id { get; set; }
        public virtual string Identification { get; set; }
        public virtual decimal Price { get; set; }

        public virtual CourseType CourseType { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}