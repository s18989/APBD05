using APBD05.DTOs.Requests;
using APBD05.DTOs.Responses;
using APBD05.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APBD05.Services
{
    public class SqlServerStudentDBService : IStudentDBService
    {
        public Enrollment EnrollStudent(EnrollStudentRequest student)
        {
            var st = new Student();
            st.IndexNumber = student.IndexNumber;
            st.FirstName = student.FirstName;
            st.LastName = student.LastName;
            st.BirthDate = student.BirthDate;
            st.Studies = student.Studies;

            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s18989;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                var enrollment = new Enrollment();

                try
                {
                    com.CommandText = "SELECT 1 FROM Studies WHERE Name = @name";
                    com.Parameters.AddWithValue("name", st.Studies);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        //return BadRequest();
                    }

                    var studies = new EnrollStudiesResponse();
                    studies.IdStudy = Int16.Parse(dr["IdStudy"].ToString());
                    studies.Name = dr["Name"].ToString();

                    com.CommandText = "SELECT 1 FROM Enrollments WHERE IdStudy = @idStudy AND Semester = 1 ORDER BY StartDate DESC";
                    com.Parameters.AddWithValue("idStudy", studies.IdStudy);

                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        var enrollmentReq = new EnrollEnrollmentRequest();

                        enrollmentReq.Semester = 1;
                        enrollmentReq.IdStudy = studies.IdStudy;
                        enrollmentReq.StartDate = DateTime.Now;

                        com.CommandText = "INSERT INTO Enrollment (Semester, IdStudy, StartDate) VALUES (@semester, @idStudy, @startDate)";
                        com.Parameters.AddWithValue("semester", enrollmentReq.Semester);
                        com.Parameters.AddWithValue("idStudy", enrollmentReq.IdStudy);
                        com.Parameters.AddWithValue("startDate", enrollmentReq.StartDate);

                        com.CommandText = "SELECT 1 FROM Enrollments WHERE IdStudy = @idStudy AND Semester = 1 ORDER BY StartDate DESC";
                        com.Parameters.AddWithValue("idStudy", studies.IdStudy);

                        dr = com.ExecuteReader();

                        enrollment.IdEnrollment = Int16.Parse(dr["IdEnrollment"].ToString());
                        enrollment.IdStudy = Int16.Parse(dr["IdStudy"].ToString());
                        enrollment.Semester = Int16.Parse(dr["Semester"].ToString());
                        enrollment.StartDate = DateTime.Parse(dr["StartDate"].ToString());


                    }
                    else if (dr.Read())
                    {
                        enrollment.IdEnrollment = Int16.Parse(dr["IdEnrollment"].ToString());
                        enrollment.IdStudy = Int16.Parse(dr["IdStudy"].ToString());
                        enrollment.Semester = Int16.Parse(dr["Semester"].ToString());
                        enrollment.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                    }

                    com.CommandText = "Select 1 From Student Where IndexNumber = @index";
                    com.Parameters.AddWithValue("index", st.IndexNumber);

                    if (dr.Read())
                    {
                        //return BadRequest();
                    }

                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@index, @fname, @lname, @birthDate, @idEnrollment";
                    com.Parameters.AddWithValue("index", st.IndexNumber);
                    com.Parameters.AddWithValue("fname", st.FirstName);
                    com.Parameters.AddWithValue("lname", st.LastName);
                    com.Parameters.AddWithValue("birthDate", st.BirthDate);
                    com.Parameters.AddWithValue("idEnrollment", enrollment.IdEnrollment);

                    tran.Commit();
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                }
                return enrollment;
            }
        }

        public Enrollment PromoteStudents(int semester, string studies)
        {
            var enrollment = new Enrollment();
            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s18989;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "SELECT 1 FROM Enrollment WHERE Semester = @semester AND Studies = (SELECT 1 FROM Studies WHERE Name = @name)";
                com.Parameters.AddWithValue("name", studies);
                com.Parameters.AddWithValue("semester", semester);

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    //return NotFound();
                }

                com.CommandText = "PromoteStudents";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.AddWithValue("Studies", studies);
                com.Parameters.AddWithValue("Semester", semester);

                dr = com.ExecuteReader();

                enrollment.IdEnrollment = Int16.Parse(dr["IdEnrollment"].ToString());
                enrollment.IdStudy = Int16.Parse(dr["IdStudy"].ToString());
                enrollment.Semester = Int16.Parse(dr["Semester"].ToString());
                enrollment.StartDate = DateTime.Parse(dr["StartDate"].ToString());

            }
            return enrollment;
        }

        public Boolean CheckIndex(string index)
        {
            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s18989;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "SELECT 1 FROM Student WHERE IndexNumber = @index";
                com.Parameters.AddWithValue("index", index);

                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
