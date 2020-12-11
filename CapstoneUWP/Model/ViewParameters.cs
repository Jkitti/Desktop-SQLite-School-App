using System.Collections.Generic;
using CapstoneUWP.Model.Repo_Model;

namespace CapstoneUWP.Model
{
    /// <summary>
    ///     Represents the view parameters to be passes between pages
    /// </summary>
    public class ViewParameters
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        /// <value>
        ///     The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        /// <value>
        /// The name of the course.
        /// </value>
        public string CourseName { get; set; }

        /// <summary>
        /// Gets or sets the course identifier.
        /// </summary>
        /// <value>
        /// The course identifier.
        /// </value>
        public int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the grade item.
        /// </summary>
        /// <value>
        /// The grade item.
        /// </value>
        public GradeItem GradeItem { get; set; }

        /// <summary>
        /// Gets or sets the grades.
        /// </summary>
        /// <value>
        /// The grades.
        /// </value>
        public Queue<GradeItemGrade> Grades { get; set; }

        /// <summary>
        /// Gets or sets the rubric item.
        /// </summary>
        /// <value>
        /// The rubric item.
        /// </value>
        public RubricItem RubricItem { get; set; }
        #endregion
    }
}
