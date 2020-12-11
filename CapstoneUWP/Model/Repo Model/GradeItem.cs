namespace CapstoneUWP.Model.Repo_Model
{
    /// <summary>
    /// Class for a grade item object
    /// </summary>
    public class GradeItem
    {
        /// <summary>
        /// Gets the assignment identifier.
        /// </summary>
        /// <value>
        /// The assignment identifier.
        /// </value>
        public int AssignmentId { get; set; }

        /// <summary>
        /// Gets the course identifier.
        /// </summary>
        /// <value>
        /// The course identifier.
        /// </value>
        public int CourseId { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets the maximum grade.
        /// </summary>
        /// <value>
        /// The maximum grade.
        /// </value>
        public int MaxGrade { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeItem"/> class.
        /// </summary>
        /// <param name="aid">The aid.</param>
        /// <param name="cid">The cid.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="type">The type.</param>
        /// <param name="max">The maximum.</param>
        public GradeItem(int aid, int cid, string name, string description, string type, int max)
        {
            this.AssignmentId = aid;
            this.CourseId = cid;
            this.Name = name;
            this.Description = description;
            this.Type = type;
            this.MaxGrade = max;
        }
    }
}
