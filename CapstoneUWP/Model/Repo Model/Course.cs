namespace CapstoneUWP.Model.Repo_Model
{
    public class Course
    {
        /// <summary>
        /// Gets or sets the course identifier.
        /// </summary>
        /// <value>
        /// The course identifier.
        /// </value>
        public int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the CRN.
        /// </summary>
        /// <value>
        /// The CRN.
        /// </value>
        public int Crn { get; set; }

        /// <summary>
        /// Gets or sets the section number.
        /// </summary>
        /// <value>
        /// The section number.
        /// </value>
        public int SectionNumber { get; set; }

        /// <summary>
        /// Gets or sets the credit hours.
        /// </summary>
        /// <value>
        /// The credit hours.
        /// </value>
        public int CreditHours { get; set; }

        /// <summary>
        /// Gets or sets the remaining seats.
        /// </summary>
        /// <value>
        /// The remaining seats.
        /// </value>
        public int RemainingSeats { get; set; }

        /// <summary>
        /// Gets or sets the maximum seats.
        /// </summary>
        /// <value>
        /// The maximum seats.
        /// </value>
        public int MaxSeats { get; set; }

        /// <summary>
        /// Gets or sets the meeting times.
        /// </summary>
        /// <value>
        /// The meeting times.
        /// </value>
        public string MeetingTimes { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the teacher identifier.
        /// </summary>
        /// <value>
        /// The teacher identifier.
        /// </value>
        public int TeacherId { get; set; }
    }
}
