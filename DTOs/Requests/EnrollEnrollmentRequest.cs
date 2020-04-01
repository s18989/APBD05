using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD05.DTOs.Requests
{
    public class EnrollEnrollmentRequest
    {
        public int Semester { get; set; }
        public int IdStudy { get; set; }
        public DateTime StartDate { get; set; }
    }
}
