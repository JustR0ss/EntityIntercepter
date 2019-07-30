using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterceptorForTemporalTables.Models
{
    public class StudentBook
    {
        public int StudentBookId { get; set; }
        public int StudentId { get; set; }
        public int BookId { get; set; }

        public virtual Book Book { get; set; }
        public virtual Student Student { get; set; }
    }
}