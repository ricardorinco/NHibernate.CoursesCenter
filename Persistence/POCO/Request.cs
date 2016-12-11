using System;
using System.Collections.Generic;

namespace Persistence.POCO
{
    public class Request
    {
        public virtual int Id { get; set; }
        public virtual DateTime RequestDateTime { get; set; }
        public virtual Student Student { get; set; }

        public virtual ICollection<RequestDetail> RequestDetails { get; set; }
    }
}