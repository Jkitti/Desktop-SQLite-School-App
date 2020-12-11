using System.Collections.Generic;
using CapstoneUWP.DAL.Repositories;
using CapstoneUWP.Model.Display_Model;
using CapstoneUWP.Model.Repo_Model;

namespace CapstoneUWP.Controller
{
    public class CourseController
    {
        private CourseGradeRepository repo;
        private GradeItemRepository gradeRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseController"/> class.
        /// </summary>
        public CourseController()
        {
            this.repo = new CourseGradeRepository();
            this.gradeRepo = new GradeItemRepository();
        }

        /// <summary>
        /// Gets the student grades by course identifier.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public List<StudentGradeDisplay> GetStudentGradesbyCourseId(int courseId)
        {
            return this.repo.GetStudentGradesbyCourseId(courseId);
        }

        /// <summary>
        /// Gets the student grade item grades.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public List<StudentGradeItemGrade> GetStudentGradeItemGrades(int courseId)
        {
            var gradeitems = (List<GradeItemGrade>)this.gradeRepo.GetAllGradedItemGrades(courseId);
            return (List<StudentGradeItemGrade>)this.gradeRepo.CreateAllStudentGradeItemGrades(gradeitems, courseId);
        }
    }

}
