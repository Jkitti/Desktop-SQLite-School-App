namespace CapstoneUWP.Model.Repo_Model
{
    public class GradeItemGrade
    {
        /// <summary>
        /// Gets or sets the assignment identifier.
        /// </summary>
        /// <value>
        /// The assignment identifier.
        /// </value>
        public int assignmentID { get; set; }

        /// <summary>
        /// Gets or sets the student identifier.
        /// </summary>
        /// <value>
        /// The student identifier.
        /// </value>
        public int studentID { get; set; }

        /// <summary>
        /// Gets or sets the grade.
        /// </summary>
        /// <value>
        /// The grade.
        /// </value>
        public int Grade { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GradeItemGrade"/> is graded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if graded; otherwise, <c>false</c>.
        /// </value>
        public bool Graded { get; set; }
    }
}
