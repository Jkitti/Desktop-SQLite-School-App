using System.Collections.Generic;

namespace CapstoneUWP.Model.Display_Model
{
    public class StudentGradeDisplay
    {
        public List<StudentGradeItemGrade> GradeItems { get; set; }
        public string Student { get; set; }

        public double Grade { get; set; }

        public int studentID { get; set; }

        public int courseID { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="StudentGradeDisplay"/> class.
        /// </summary>
        /// <param name="student">The student.</param>
        /// <param name="grade">The grade.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="courseId">The course identifier.</param>
        public StudentGradeDisplay(List<StudentGradeItemGrade> gradeItems, string student, double grade, int studentId, int courseId)
        {
            this.GradeItems = gradeItems;
            this.Student = student;
            this.Grade = grade;
            this.studentID = studentId;
            this.courseID = courseId;
        }
    }
}
