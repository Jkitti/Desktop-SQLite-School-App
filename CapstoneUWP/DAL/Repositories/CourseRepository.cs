using System;
using System.Collections.Generic;
using System.Linq;
using CapstoneUWP.Model;
using CapstoneUWP.Model.Repo_Model;
using DataAccessLibrary;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace CapstoneUWP.DAL.Repositories
{
    class CourseRepository
    {
        private readonly MySqlConnection dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseRepository"/> class.
        /// </summary>
        public CourseRepository()
        {
            this.dbConnection = DbConnection.getConnection();
        }

        /// <summary>
        /// Gets all grade items for course from online db.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns>List of grade items for the course</returns>
        public List<GradeItem> DbGetAllGradeItems(List<Course> coursesIds)
        {
            var gradeItems = new List<GradeItem>();
            foreach (var course in coursesIds)
            {
                const string sqlStatementGetById =
                    "SELECT aid, cid, name, description, grade_type, max_grade from `cs4982s19c`.`Course_grade_item` WHERE cid = @id";
                using (this.dbConnection)
                {
                    this.dbConnection.Open();
                    var query = this.dbConnection.CreateCommand();
                    query.CommandText = sqlStatementGetById;
                    query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                    query.Parameters["@id"].Value = course.CourseId;
                    using (var reader = query.ExecuteReader())
                        while (reader.Read())
                            gradeItems.Add(new GradeItem(
                                Convert.ToInt32(reader.GetString(0)),
                                Convert.ToInt32(reader.GetString(1)),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetString(4),
                                Convert.ToInt32(reader.GetString(5))));
                }
            }
            return gradeItems;
        }

        /// <summary>
        /// Gets all students by course online database.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public List<Student> GetAllStudentsByCourseOnlineDb(int id)
        {
            List<Student> studentList = new List<Student>();
            const string sqlStatementGetById =
                "SELECT sid from `cs4982s19c`.`Course_student_participant` WHERE cid = @id";
            using (this.dbConnection)
            {
                this.dbConnection.Open();
                var query = this.dbConnection.CreateCommand();
                query.CommandText = sqlStatementGetById;
                query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                query.Parameters["@id"].Value = id;
                var studentIds = new List<int>();
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                    {
                        studentIds.Add(reader.GetInt32(0));
                    }
                this.dbConnection.Close();
                foreach (var sid in studentIds)
                {
                    studentList.Add(this.DbGetStudentById(sid));
                    DataAccess.AddData("INSERT INTO Course_student_participant (cid, sid) VALUES (" + id + ", " + sid + ")");
                }

            }
            return studentList;
        }

        /// <summary>
        /// Gets all students by course local database.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public List<Student> GetAllStudentsByCourseLocalDb(int id)
        {
            List<Student> studentList = new List<Student>();
            const string sqlStatementGetById =
                "SELECT sid from Course_student_participant WHERE cid = @id";
            using (SqliteConnection db =
                DataAccess.LocalConnection)
            {
                db.Open();
                var query = db.CreateCommand();
                query.CommandText = sqlStatementGetById;
                query.Parameters.Add("@id", SqliteType.Integer, 11);
                query.Parameters["@id"].Value = id;
                var studentIds = new List<int>();
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                    {
                        studentIds.Add(reader.GetInt32(0));
                    }
                this.dbConnection.Close();
                foreach (var sid in studentIds)
                {
                    studentList.Add(this.LocalGetStudentById(sid));
                }

            }
            return studentList;
        }

        /// <summary>
        /// Gets all enrolled students from all courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public List<Student> GetAllEnrolledStudents(List<Course> courses)
        {
            List<Student> sList = new List<Student>();
            foreach (var course in courses)
            {
                var clist = this.GetAllStudentsByCourseOnlineDb(course.CourseId);
                foreach (var student in clist)
                {
                    sList.Add(student);
                }
            }
            return sList;
        }

        /// <summary>
        /// Writes all student course grades to local db.
        /// </summary>
        /// <param name="courses">The courses.</param>
        public void WriteAllStudentCourseGrades(List<Course> courses)
        {
            List <StudentCourseGrade> grades = new List<StudentCourseGrade>();
            foreach (var course in courses)
            {
                var studentgrades = this.GetAllStudentCourseGradesByCourseId(course.CourseId);
                grades = grades.Concat(studentgrades).ToList();
            }
            foreach (var grade in grades)
            {
                DataAccess.AddData("INSERT INTO Course_student_participant_grade (cid, sid, grade) VALUES (" + grade.CourseID + ", " + grade.StudentID + 
                                   "," + grade.Grade +")");
            }
        }

        /// <summary>
        /// Gets all student course grades from online db by course identifier.
        /// </summary>
        /// <param name="coursesId">The courses identifier.</param>
        /// <returns></returns>
        public List<StudentCourseGrade> GetAllStudentCourseGradesByCourseId(int coursesId)
        {
            List<StudentCourseGrade> grades = new List<StudentCourseGrade>();
            const string sqlStatementGetById =
                "SELECT cid, sid, grade from `cs4982s19c`.`Course_student_participant_grade` WHERE cid = @id";
            using (this.dbConnection)
            {
                this.dbConnection.Open();
                var query = this.dbConnection.CreateCommand();
                query.CommandText = sqlStatementGetById;
                query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                query.Parameters["@id"].Value = coursesId;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        grades.Add(new StudentCourseGrade(
                            Convert.ToInt32(reader.GetString(0)),
                            Convert.ToInt32(reader.GetString(1)),
                            Convert.ToInt32(reader.GetDouble(2))));
            }
            this.dbConnection.Close();
            return grades;
        }

        /// <summary>
        /// Gets all rubric items by course identifier from online Db.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>List of rubric items matching course id</returns>
        public IList<RubricItem> DbGetAllRubricItemsByCourseId(int id)
        {
            const string courseRubricQuery = "SELECT rid, cid, assignment_type, grade_percentage FROM cs4982s19c.Course_rubric" +
                                             " where cid = @id";
            IList<RubricItem> items = new List<RubricItem>();
            using (this.dbConnection)
            {
                this.dbConnection.Open();
                var query = this.dbConnection.CreateCommand();
                query.CommandText = courseRubricQuery;
                query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                query.Parameters["@id"].Value = id;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        items.Add(new RubricItem()
                        {
                            RubricId = reader.GetInt32(0),
                            CourseId = reader.GetInt32(1),
                            AssignmentType = reader.GetString(2),
                            GradeWeight = reader.GetDouble(3)
                        });
            }

            return items;
        }

        /// Gets all rubric items by course identifier from online db.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>List of rubric items matching course id</returns>
        public IList<GradeItemGrade> DbGetAllGradedGradeItemsByCourseId(int id)
        {
            GradeItemRepository gRepo = new GradeItemRepository();
            var gradeItems = gRepo.GetAllGradeItemsByCourse(id);
            List<GradeItemGrade> grades = new List<GradeItemGrade>();
            foreach (var gi in gradeItems)
            {
                const string sqlStatementGetById =
                    "SELECT sid, aid, grade, graded from `cs4982s19c`.`Course_student_grade_item_grade` WHERE aid = @id";
                using (this.dbConnection)
                {
                    this.dbConnection.Open();
                    var query = this.dbConnection.CreateCommand();
                    query.CommandText = sqlStatementGetById;
                    query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                    query.Parameters["@id"].Value = gi.AssignmentId;
                    using (var reader = query.ExecuteReader())
                        while (reader.Read())
                            grades.Add(new GradeItemGrade()
                            {
                                Grade = Convert.ToInt32(reader.GetString(2)),
                                assignmentID = reader.GetInt32(1),
                                studentID = reader.GetInt32(0),
                                Graded = reader.GetBoolean(3)
                            });
                }
            }

            return grades;
        }

        /// <summary>
        /// Gets all graded grade items from multiple courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public IList<GradeItemGrade> GetAllGradedGradeItems(List<Course> courses)
        {
            List<GradeItemGrade> gList = new List<GradeItemGrade>();
            foreach (var course in courses)
            {
                var clist = this.DbGetAllGradedGradeItemsByCourseId(course.CourseId);
                foreach (var item in clist)
                {
                    gList.Add(item);
                }
            }
            return gList;
        }

        /// <summary>
        /// Gets the student by identifier from online db.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Student matching the id</returns>
        public Student DbGetStudentById(int id)
        {
            const string sqlStatementGetById =
                "SELECT pid, fname, lname, "
                + " email, phone, degree_program, degree_progress, gpa FROM cs4982s19c.Person," +
                " cs4982s19c.Student WHERE pid = @id  and sid = pid;";

            Student student = null;
            using (this.dbConnection)
            {
                this.dbConnection.Open();
                var query = this.dbConnection.CreateCommand();
                query.CommandText = sqlStatementGetById;
                query.Parameters.Add("@id", MySqlDbType.Int32, 11);
                query.Parameters["@id"].Value = id;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        student = new Student
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Phone = reader.GetString(4),
                            DegreeProgram = reader.GetString(5),
                            DegreeProgress = reader.GetString(6),
                            GPA = reader.GetString(7)
                        };
            }
            return student;
        }

        /// <summary>
        /// the get student by identifier from local db.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Student LocalGetStudentById(int id)
        {
            const string sqlStatementGetById =
                "SELECT pid, fname, lname, "
                + " email, phone, degree_program, degree_progress, gpa FROM Person," +
                "Student WHERE pid = @id  and sid = pid;";

            Student student = null;
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
                        student = new Student
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Phone = reader.GetString(4),
                            DegreeProgram = reader.GetString(5),
                            DegreeProgress = reader.GetString(6),
                            GPA = reader.GetString(7)
                        };
            }
            return student;
        }

        /// <summary>
        /// Gets all course rubric items.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public List<RubricItem> GetAllCourseRubricItems(List<Course> courses)
        {
            List<RubricItem> rList = new List<RubricItem>();
            foreach (var course in courses)
            {
                var clist = this.DbGetAllRubricItemsByCourseId(course.CourseId);
                foreach (var item in clist)
                {
                    rList.Add(item);
                }
            }
            return rList;
        }

        /// <summary>
        /// Writes the course items to local db file.
        /// </summary>
        /// <param name="courses">The courses.</param>
        public void WriteCourseItemsToFile(List<Course> courses)
        {
            this.WriteGradeItemsToLocalDb(courses);
            this.WriteGradedGradeItemsToLocalDb(courses);
            this.WriteStudentsToLocalDb(courses);
            this.WriteRubricItemsToLocalDb(courses);
            this.WriteAllStudentCourseGrades(courses);
        }

        /// <summary>
        /// Writes the grade items to local database.
        /// </summary>
        /// <param name="courses">The courses.</param>
        public void WriteGradeItemsToLocalDb(List<Course> courses)
        {
            List<GradeItem> gItems = this.DbGetAllGradeItems(courses);

            foreach (var gradeItem in gItems)
            {
                var query = "INSERT INTO Course_grade_item (aid, cid, name, description, grade_type, max_grade) VALUES "+
                            "(" + gradeItem.AssignmentId + "," + gradeItem.CourseId + ",'" + gradeItem.Name + "','" + gradeItem.Description + "','" +
                            gradeItem.Type + "'," + gradeItem.MaxGrade + ")";
                DataAccess.AddData(query);
            }
        }

        /// <summary>
        /// Writes the graded grade items to local database.
        /// </summary>
        /// <param name="courses">The courses.</param>
        public void WriteGradedGradeItemsToLocalDb(List<Course> courses)
        {
            var gItems = this.GetAllGradedGradeItems(courses);

            foreach (var gradeItem in gItems)
            {
                var query = "INSERT INTO Course_student_grade_item_grade (sid, aid, grade, graded) VALUES " +
                            "(" + gradeItem.studentID + "," + gradeItem.assignmentID + "," + gradeItem.Grade + "," + Convert.ToInt32(gradeItem.Graded) + ")";
                DataAccess.AddData(query);
            }
        }

        /// <summary>
        /// Writes the students to local database.
        /// </summary>
        /// <param name="courses">The courses.</param>
        public void WriteStudentsToLocalDb(List<Course> courses)
        {
            var students = this.GetAllEnrolledStudents(courses);
            foreach (var student in students)
            {
                var query = "INSERT INTO Person (pid, fname, lname) VALUES (" + student.Id + ",'" + student.FirstName + "','" + student.LastName + "');" +
                    "INSERT INTO Student (sid, email, phone, degree_program, degree_progress, gpa) VALUES " +
                            "(" + student.Id + ",'" + student.Email + "','" + student.Phone + "','" 
                            + student.DegreeProgram +"','" + student.DegreeProgress + "','" + student.GPA + "')";
                DataAccess.AddData(query);
            }
        }

        /// <summary>
        /// Writes the rubric items to local database.
        /// </summary>
        /// <param name="courses">The courses.</param>
        public void WriteRubricItemsToLocalDb(List<Course> courses)
        {
            var rubrics = this.GetAllCourseRubricItems(courses);
            foreach (var item in rubrics)
            {
                var query = "INSERT INTO Course_rubric (rid, cid, assignment_type, grade_percentage) VALUES " +
                            "(" + item.RubricId + "," + item.CourseId + ",'" + item.AssignmentType + "'," + item.GradeWeight +")";
                DataAccess.AddData(query);
            }
        }

    }
}
