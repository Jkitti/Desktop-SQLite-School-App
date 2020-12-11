namespace CapstoneUWP.Model.Repo_Model
{
    public class StudentCourseGrade
    {

        /// <summary>
        /// Gets or sets the course identifier.
        /// </summary>
        /// <value>
        /// The course identifier.
        /// </value>
        public int CourseID { get; set; }

        /// <summary>
        /// Gets or sets the student identifier.
        /// </summary>
        /// <value>
        /// The student identifier.
        /// </value>
        public int StudentID { get; set; }

        /// <summary>
        /// Gets or sets the grade.
        /// </summary>
        /// <value>
        /// The grade.
        /// </value>
        public double Grade { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentCourseGrade"/> class.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <param name="student">The student.</param>
        /// <param name="grade">The grade.</param>
        public StudentCourseGrade(int course, int student, double grade)
        {
            this.CourseID = course;
            this.StudentID = student;
            this.Grade = grade;
        }
    }
}
