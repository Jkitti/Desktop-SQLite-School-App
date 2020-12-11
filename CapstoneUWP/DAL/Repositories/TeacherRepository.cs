using System;
using System.Collections.Generic;
using CapstoneUWP.Model;
using CapstoneUWP.Model.Repo_Model;
using DataAccessLibrary;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace CapstoneUWP.DAL.Repositories
{
    public class TeacherRepository
    {
        private readonly MySqlConnection dbConnection;
        /// <summary>
        /// Initializes a new instance of the <see cref="TeacherRepository"/> class.
        /// </summary>
        public TeacherRepository()
        {
            this.dbConnection = DbConnection.getConnection();
        }


        /// <summary>
        /// Gets a list of courses by teacher identifier.
        /// </summary>
        /// <param name="id">The teacher identifier.</param>
        /// <returns>List of courses matching teacher id</returns>
        public void WriteCoursesById(int id)
        {
            const string sqlStatementGetById =
                "SELECT cid, name, crn,section_number, credit_hours, remaining_seats, max_seats, meeting_times, location, teacher FROM cs4982s19c.Course WHERE teacher = @id";

            List<Course> courses = new List<Course>();
            using (this.dbConnection)
            {
                this.dbConnection.Open();
                var query = this.dbConnection.CreateCommand();
                query.CommandText = sqlStatementGetById;
                query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                query.Parameters["@id"].Value = id;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        courses.Add(new Course
                        {
                            CourseId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Crn = reader.GetInt32(2),
                            SectionNumber = reader.GetInt32(3),
                            CreditHours = reader.GetInt32(4),
                            RemainingSeats = reader.GetInt32(5),
                            MaxSeats = reader.GetInt32(6),
                            MeetingTimes = reader.GetString(7),
                            Location = reader.GetString(8),
                            TeacherId = reader.GetInt32(9)

                        });
            }
            this.WriteCoursesToLocalDb(courses);
        }

        /// <summary>
        /// Gets all credentials.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pass">The password.</param>
        /// <returns></returns>
        public List<Credentials> GetAllCredentials(int id, string pass)
        {
            const string sqlStatementGetAll =
                "SELECT cid, password FROM cs4982s19c.Credentials WHERE cid = @id AND password = @pass";

            List<Credentials> creds = new List<Credentials> ();
            using (this.dbConnection)
            {
                this.dbConnection.Open();
                var query = this.dbConnection.CreateCommand();
                query.CommandText = sqlStatementGetAll;
                query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                query.Parameters["@id"].Value = id;
                query.Parameters.Add("@pass", MySqlDbType.VarChar);
                query.Parameters["@pass"].Value = pass;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        creds.Add(new Credentials
                        {
                            Id = reader.GetInt32(0),
                            Password = reader.GetString(1),
                        });
            }
            return creds;
        }

        /// <summary>
        /// Gets the teacher information.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public List<Teacher> GetTeacherInfo(int id)
        {
            const string sqlStatementGetAll =
                "SELECT pid, fname, lname, office_location, office_hours, phone FROM cs4982s19c.Person, cs4982s19c.Teacher WHERE pid = @id AND tid = @id";

            List<Teacher> teachers = new List<Teacher>();
            using (this.dbConnection)
            {
                this.dbConnection.Open();
                var query = this.dbConnection.CreateCommand();
                query.CommandText = sqlStatementGetAll;
                query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                query.Parameters["@id"].Value = id;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        teachers.Add(new Teacher()
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Office = reader.GetString(3),
                            Hours = reader.GetString(4),
                            Phone = reader.GetString(5)
                        });
            }
            return teachers;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public String GetUserName(int id)
        {
            string first = "";
            string last = "";

            const string sqlStatementGetById =
                "SELECT fname, lname FROM Person WHERE pid = @id";
            using (SqliteConnection db =
                DataAccess.LocalConnection)
            {
                db.Open();
                var query = db.CreateCommand();
                query.CommandText = sqlStatementGetById;
                query.Parameters.Add("@id", SqliteType.Integer, 11);
                query.Parameters["@id"].Value = id;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                    {
                        first = reader.GetString(0);
                        last = reader.GetString(1);
                    }
            }

            String full = first + " " + last;
            return full;
        }

        /// <summary>
        /// Writes the credentials to local database.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pass">The pass.</param>
        public void WriteCredentialsToLocalDb(int id, string pass)
        {
            List<Credentials> creds = this.GetAllCredentials(id, pass);

            foreach (var credentials in creds)
            {
                var query = "INSERT INTO Credentials (cid, password) VALUES ("+ credentials.Id + ",'" + credentials.Password +
                            "')";
                DataAccess.AddData(query);
            }


            this.WriteTeacherToLocalDb(id);
            this.WriteCoursesById(id);
        }

        /// <summary>
        /// Writes the teacher to local database.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void WriteTeacherToLocalDb(int id)
        {
            List<Teacher> teachers = this.GetTeacherInfo(id);
            foreach (var teacher in teachers)
            {
                var query = "INSERT INTO Person (pid, fname, lname) VALUES (" + teacher.Id + ",'" + teacher.FirstName +
                            "','" + teacher.LastName + "')";
                DataAccess.AddData(query);

                var teacherquery = "INSERT INTO Teacher (tid, office_location, office_hours, phone) VALUES (" + teacher.Id + ",'" + teacher.Office +
                                   "','" + teacher.Hours + "','" + teacher.Phone + "')";
                DataAccess.AddData(teacherquery);
            }
        }

        /// <summary>
        /// Writes the courses to local database.
        /// </summary>
        /// <param name="courses">The courses.</param>
        public void WriteCoursesToLocalDb(List<Course> courses)
        {
            foreach (var course in courses)
            {
                var query = "INSERT INTO Course (cid, name, crn, section_number, credit_hours, remaining_seats, max_seats, meeting_times, location, teacher)" +
                            "VALUES (" + course.CourseId + ",'" + course.Name +"'," + course.Crn + "," + course.SectionNumber +"," + course.CreditHours +
                            "," + course.RemainingSeats + "," + course.MaxSeats +",'" + course.MeetingTimes + "','" + course.Location + "'," + course.TeacherId +")";
                DataAccess.AddData(query);
            }
            var courseRepo = new CourseRepository();
            courseRepo.WriteCourseItemsToFile(courses);
        }

        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pass">The pass.</param>
        /// <returns></returns>
        public bool ValidateCredentials(int id, string pass)
        {
            const string sqlStatementGetAll =
                "SELECT cid, password FROM Credentials WHERE cid = @id AND password = @pass";

            List<Credentials> logins = new List<Credentials>();
            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                try
                {
                    db.Open();
                    var query = db.CreateCommand();
                    query.CommandText = sqlStatementGetAll;
                    query.Parameters.Add("@id", SqliteType.Integer, 11);
                    query.Parameters["@id"].Value = id;
                    query.Parameters.Add("@pass", SqliteType.Text);
                    query.Parameters["@pass"].Value = pass;
                    using (var reader = query.ExecuteReader())
                        while (reader.Read())
                            logins.Add(new Credentials {
                                Id = reader.GetInt32(0),
                                Password = reader.GetString(1),
                            });
                }
                catch (Exception)
                {
                    return false;
                }

                foreach (var cred in logins)
                {
                    if (cred.Id == id && cred.Password == pass)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Checks for local data.
        /// </summary>
        /// <returns></returns>
        public bool CheckForLocalData()
        {
            const string sqlStatementGetAll =
                "SELECT cid, password FROM Credentials WHERE 1";

            List<Credentials> logins = new List<Credentials>();
            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                try
                {
                    db.Open();
                    var query = db.CreateCommand();
                    query.CommandText = sqlStatementGetAll;
                    using (var reader = query.ExecuteReader())
                        while (reader.Read())
                            logins.Add(new Credentials {
                                Id = reader.GetInt32(0),
                                Password = reader.GetString(1),
                            });
                }
                catch (Exception)
                {

                    return false;
                }
            }

            if (logins.Count > 0)
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Gets all courses.
        /// </summary>
        /// <returns>List of all courses</returns>
        public IList<Course> GetAllCourses()
        {
            List<Course> courses = new List<Course>();

            const string sqlStatementGetById =
                "SELECT cid, name, crn,section_number, credit_hours, remaining_seats, max_seats, meeting_times, location, teacher FROM Course";
            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                db.Open();
                var query = db.CreateCommand();
                query.CommandText = sqlStatementGetById;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        courses.Add(new Course
                        {
                            CourseId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Crn = reader.GetInt32(2),
                            SectionNumber = reader.GetInt32(3),
                            CreditHours = reader.GetInt32(4),
                            RemainingSeats = reader.GetInt32(5),
                            MaxSeats = reader.GetInt32(6),
                            MeetingTimes = reader.GetString(7),
                            Location = reader.GetString(8),
                            TeacherId = reader.GetInt32(9)

                        });
            }

            return courses;
        }
    }

    public class Credentials
    {
        public int Id { get; set; }

        public  string Password { get; set; }
    }
}
