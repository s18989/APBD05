using APBD05.DTOs.Requests;
using APBD05.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD05.Services
{
    public interface IStudentDBService
    {
        Enrollment EnrollStudent(EnrollStudentRequest student);
        Enrollment PromoteStudents(int semester, string studies);
        bool CheckIndex(string index);

    }
}
