namespace CapstoneUWP.Model.Repo_Model
{
    /// <summary>
    /// Class for a rubric item object
    /// </summary>
    public class RubricItem
    {
        /// <summary>
        /// Gets or sets the rubric identifier.
        /// </summary>
        /// <value>
        /// The rubric identifier.
        /// </value>
        public int RubricId { get; set; }

        /// <summary>
        /// Gets or sets the course identifier.
        /// </summary>
        /// <value>
        /// The course identifier.
        /// </value>
        public int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the type of the assignment.
        /// </summary>
        /// <value>
        /// The type of the assignment.
        /// </value>
        public string AssignmentType { get; set; }

        /// <summary>
        /// Gets or sets the grade weight.
        /// </summary>
        /// <value>
        /// The grade weight.
        /// </value>
        public double GradeWeight { get; set; }
    }
}
