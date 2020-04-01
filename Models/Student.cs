using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD05.Models
{
    public class Student
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public string Studies { get; set; }
    }
}
