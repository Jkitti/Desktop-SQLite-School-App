using System;
using System.Collections.Generic;
using System.Linq;
using CapstoneUWP.Model;
using CapstoneUWP.Model.Display_Model;
using CapstoneUWP.Model.Repo_Model;
using DataAccessLibrary;
using Microsoft.Data.Sqlite;

namespace CapstoneUWP.DAL.Repositories
{
    class CourseGradeRepository
    {
        private SqliteConnection db;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseGradeRepository"/> class.
        /// </summary>
        public CourseGradeRepository()
        {
            this.db = DataAccess.LocalConnection;
        }

        /// <summary>
        /// Gets the student grades by course identifier.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public List<StudentGradeDisplay> GetStudentGradesbyCourseId(int courseId)
        {
            List<StudentGradeDisplay> formattedGrades = new List<StudentGradeDisplay>();

            List<StudentCourseGrade> grades = new List<StudentCourseGrade>();
            const string sqlStatementGetById =
                "SELECT cid, sid, grade from `Course_student_participant_grade` WHERE cid = @id";
            using (this.db)
            {
                this.db.Open();
                var query = this.db.CreateCommand();
                query.CommandText = sqlStatementGetById;
                query.Parameters.Add("@id",SqliteType.Integer, 11);
                query.Parameters["@id"].Value = courseId;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        grades.Add(new StudentCourseGrade(
                            Convert.ToInt32(reader.GetString(0)),
                            Convert.ToInt32(reader.GetString(1)),
                            Convert.ToInt32(reader.GetDouble(2))));
            }
            this.db.Close();
            var courseRepo = new CourseRepository();
            foreach (var grade in grades)
            {
                var student = courseRepo.LocalGetStudentById(grade.StudentID);
                var gradeItems = this.GetStudentGradeItemGrades(grade.StudentID, courseId);
                var studentName = student.FirstName + " " + student.LastName;
                StudentGradeDisplay item = new StudentGradeDisplay(gradeItems, studentName, grade.Grade, grade.StudentID, grade.CourseID);
                formattedGrades.Add(item);
            }

            return formattedGrades;
        }

        /// <summary>
        /// Gets the student grade item grades.
        /// </summary>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public List<StudentGradeItemGrade> GetStudentGradeItemGrades(int studentId, int courseId)
        {
            var gradeRepo = new GradeItemRepository();
            List<GradeItemGrade> grades = (List<GradeItemGrade>) gradeRepo.GetAllGradeItemGradeByCourse(courseId);
            var studentGrades = gradeRepo.CreateAllStudentGradeItemGrades(grades, courseId);
            var selectedItems = from gi in studentGrades
                                where gi.studentID == studentId &&
                                      gi.Grade != 0
                                select gi;

            var studentItems = selectedItems.ToList<StudentGradeItemGrade>();

            return studentItems;

        }
    }
}
