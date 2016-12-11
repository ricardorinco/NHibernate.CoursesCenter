using System;
using System.Collections.Generic;

namespace Persistence.POCO
{
    public class Student
    {
        public Student()
        {
            Requests = new List<Request>();
        }

        public virtual int Id { get; set; }
        public virtual string Identification { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual string Email { get; set; }
        public virtual int Registration { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }
}