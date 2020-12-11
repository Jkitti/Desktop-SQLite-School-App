using System;
using System.Collections.Generic;
using CapstoneUWP.DAL.Repositories;
using CapstoneUWP.Model;
using CapstoneUWP.Model.Display_Model;
using CapstoneUWP.Model.Repo_Model;

namespace CapstoneUWP.Controller
{
    class GradeItemController
    {
        private RubricRepository rubricRepo;
        private GradeItemRepository repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeItemController"/> class.
        /// </summary>
        public GradeItemController()
        {
            this.rubricRepo = new RubricRepository();
            this.repo = new GradeItemRepository();
        }

        /// <summary>
        /// Gets all rubric items.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public List<RubricItem> GetAllRubricItems(int courseId)
        {
            return this.rubricRepo.GetAllRubricItems(courseId);
        }

        /// <summary>
        /// Gets the rubric types.
        /// </summary>
        /// <param name="rubricItems">The rubric items.</param>
        /// <returns></returns>
        public List<String> GetTypes(List<RubricItem> rubricItems)
        {
            return this.rubricRepo.GetTypes(rubricItems);
        }

        /// <summary>
        /// Gets the student grade item grades.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public IList<StudentGradeItemGrade> GetStudentGradeItemGrades(int courseId)
        {
            return this.repo.GetStudentGradeItemGrades(courseId);
        }

        /// <summary>
        /// Gets all graded item grades by identifier.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <returns></returns>
        public IList<GradeItemGrade> GetAllGradedItemGradesById(int assignmentId)
        {
           return this.repo.GetAllGradedItemGradesById(assignmentId);
        }

        /// <summary>
        /// Creates the student grade item grade.
        /// </summary>
        /// <param name="grade">The grade.</param>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public StudentGradeItemGrade CreateStudentGradeItemGrade(GradeItemGrade grade, int courseId)
        {
            return this.repo.CreateStudentGradeItemGrade(grade, courseId);
        }

        /// <summary>
        /// Updates the grade item grade.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="grade">The grade.</param>
        public void UpdateGradeItemGrade(int assignmentId, int studentId, int grade)
        {
            this.repo.UpdateGradeItemGrade(assignmentId, studentId, grade);
        }

        /// <summary>
        /// Gets all grade items by course.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public IList<GradeItem> GetAllGradeItemsByCourse(int courseId)
        {
           return this.repo.GetAllGradeItemsByCourse(courseId);
        }

        /// <summary>
        /// Deletes the grade item from local database.
        /// </summary>
        /// <param name="gradeItem">The grade item.</param>
        public void DeleteGradeItemFromLocalDb(GradeItem gradeItem)
        {
            this.repo.DeleteGradeItemFromLocalDb(gradeItem);
        }

        /// <summary>
        /// Edits the grade item in local database.
        /// </summary>
        /// <param name="gradeItem">The grade item.</param>
        public void EditGradeItemInLocalDb(GradeItem gradeItem)
        {
            this.repo.EditGradeItemInLocalDb(gradeItem);
        }

        /// <summary>
        /// Writes the new grade item to local database.
        /// </summary>
        /// <param name="gradeItem">The grade item.</param>
        public void WriteNewGradeItemsToLocalDb(GradeItem gradeItem)
        {
            this.repo.WriteNewGradeItemsToLocalDb(gradeItem);
        }

        /// <summary>
        /// Creates all student grade item grades.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public IList<StudentGradeItemGrade> CreateAllStudentGradeItemGrades(List<GradeItemGrade> items, int courseId)
        {
            return this.repo.CreateAllStudentGradeItemGrades(items, courseId);
        }
    }
}
