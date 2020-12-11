namespace CapstoneUWP.Model.Display_Model
{
    public class StudentGradeItemGrade
    {
        public string Student { get; set; }

        public string Course { get; set; }

        public string itemName { get; set; }

        public int Grade { get; set; }

        public int assignmentID { get; set; }

        public int studentID { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentGradeItemGrade"/> class.
        /// </summary>
        /// <param name="student">The student.</param>
        /// <param name="course">The course.</param>
        /// <param name="item">The item.</param>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="grade">The grade.</param>
        public StudentGradeItemGrade(string student, string course, string item, int assignmentId, int studentId, int grade)
        {
            this.Student = student;
            this.Course = course;
            this.itemName = item;
            this.Grade = grade;
            this.assignmentID = assignmentId;
            this.studentID = studentId;
        }
    }
}
