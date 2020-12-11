using System.Collections.Generic;
using CapstoneUWP.DAL.Repositories;
using CapstoneUWP.Model;
using CapstoneUWP.Model.Repo_Model;

namespace CapstoneUWP.Controller
{
    class RubricController
    {
        private RubricRepository repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RubricController"/> class.
        /// </summary>
        public RubricController()
        {
            this.repo = new RubricRepository();
        }

        /// <summary>
        /// Gets all rubric items.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public List<RubricItem> GetAllRubricItems(int courseId)
        {
            return this.repo.GetAllRubricItems(courseId);
        }

        /// <summary>
        /// Rubrics the percentage total.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public double RubricPercentageTotal(int courseId)
        {
            return this.repo.GetRubricPercentageTotal(courseId);
        }

        /// <summary>
        /// Adds the rubric item.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="grade">The grade.</param>
        public void AddRubricItem(int courseId, string type, double grade)
        {
            this.repo.AddRubricItem(courseId, type, grade);
        }

        /// <summary>
        /// Edits the rubric item.
        /// </summary>
        /// <param name="rubricId">The rubric identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="grade">The grade.</param>
        public void EditRubricItem(int rubricId, string type, double grade)
        {
            this.repo.EditRubricItem(rubricId, type, grade);
        }

        /// <summary>
        /// Deletes the rubric item.
        /// </summary>
        /// <param name="rubricId">The rubric identifier.</param>
        public void DeleteRubricItem(int rubricId)
        {
            this.repo.DeleteRubricItem(rubricId);
        }
    }
}
