using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneUWP.Model.Display_Model;
using CapstoneUWP.Model.Repo_Model;
using DataAccessLibrary;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace CapstoneUWP.DAL.Repositories
{
    public class GradeItemRepository
    {
        private readonly MySqlConnection dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeItemRepository"/> class.
        /// </summary>
        public GradeItemRepository()
        {
            this.dbConnection = DbConnection.getConnection();
        }

        /// <summary>
        /// Gets the grade item.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <returns></returns>
        public GradeItem GetGradeItem(int assignmentId)
        {
            var allItems = this.GetAllGradeItems();
            var selectedItems = allItems.First(i => i.AssignmentId == assignmentId);
            var gradeItem = (GradeItem) selectedItems;
            return gradeItem;
        }

        /// <summary>
        /// Gets all grade items by course.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public IList<GradeItem> GetAllGradeItemsByCourse(int courseId)
        {
            List<GradeItem> courseItems = new List<GradeItem>();

            var allItems = this.GetAllGradeItems();

            var selectedItems = from gi in allItems
                                where gi.CourseId == courseId
                                select gi;

            courseItems = selectedItems.ToList<GradeItem>();
            return courseItems;
        }

        /// <summary>
        /// Gets all grade items.
        /// </summary>
        /// <returns></returns>
        public IList<GradeItem> GetAllGradeItems()
        {
            List<GradeItem> allItems = new List<GradeItem>();
            const string sqlStatementGetById =
                "SELECT aid, cid, name, description, grade_type, max_grade from Course_grade_item";

            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                db.Open();
                var query = db.CreateCommand();
                query.CommandText = sqlStatementGetById;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        allItems.Add(new GradeItem(
                            Convert.ToInt32(reader.GetString(0)),
                            Convert.ToInt32(reader.GetString(1)),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            Convert.ToInt32(reader.GetString(5))));
            }
            return allItems;
        }

        /// <summary>
        /// Gets all grade item grades.
        /// </summary>
        /// <returns></returns>
        public IList<GradeItemGrade> GetAllGradeItemGrades()
        {
            List<GradeItemGrade> allItems = new List<GradeItemGrade>();
            const string sqlStatementGetById =
                "SELECT sid, aid, grade, graded from Course_student_grade_item_grade ";

            using (SqliteConnection db = DataAccess.LocalConnection)
                {
                db.Open();
                var query = db.CreateCommand();
                query.CommandText = sqlStatementGetById;
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        allItems.Add(new GradeItemGrade()
                        {
                            Grade = Convert.ToInt32(reader.GetString(2)),
                            assignmentID = reader.GetInt32(1),
                            studentID = reader.GetInt32(0),
                            Graded = reader.GetBoolean(3)
                        });
                }
                return allItems;
        }

        /// <summary>
        /// Updates the grade item grade.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="grade">The grade.</param>
        public async void UpdateGradeItemGrade(int assignmentId, int studentId, int grade)
        {
            const string sqlStatementUpdateByIds =
                "UPDATE `Course_student_grade_item_grade` SET `grade`= @grade, `graded` = @graded WHERE aid = @aid AND sid = @sid";
            using (SqliteConnection db = DataAccess.LocalConnection)
                {
                db.Open();
                var transaction = db.BeginTransaction();
                var editGradeItemGradeCommand = db.CreateCommand();
                editGradeItemGradeCommand.Connection = db;
                editGradeItemGradeCommand.Transaction = transaction;
                try
                {
                    editGradeItemGradeCommand.CommandText = sqlStatementUpdateByIds;
                    editGradeItemGradeCommand.Parameters.Add("@grade", SqliteType.Integer, 5);
                    editGradeItemGradeCommand.Parameters["@grade"].Value = grade;
                    editGradeItemGradeCommand.Parameters.Add("@aid", SqliteType.Integer, 11);
                    editGradeItemGradeCommand.Parameters["@aid"].Value = assignmentId;
                    editGradeItemGradeCommand.Parameters.Add("@sid", SqliteType.Integer, 11);
                    editGradeItemGradeCommand.Parameters["@sid"].Value = studentId;
                    editGradeItemGradeCommand.Parameters.Add("@graded", SqliteType.Integer, 1);
                    editGradeItemGradeCommand.Parameters["@graded"].Value = 1;
                    editGradeItemGradeCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }

            var sqlUpdateStatement = "UPDATE `Course_student_grade_item_grade` SET " +
                                     "`grade`= " + grade + ", `graded` = 1 WHERE aid = " + assignmentId + " AND sid = " + studentId;
            await ChangelogModifier.AppendChangeLog(sqlUpdateStatement);

            var course = this.GetGradeItem(assignmentId).CourseId;
            this.UpdateStudentCourseGrade(studentId,course);
        }

        /// <summary>
        /// Gets all grade item grade by course.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public IList<GradeItemGrade> GetAllGradeItemGradeByCourse(int courseId)
        {
            List<GradeItemGrade> grades = new List<GradeItemGrade>();
            List<GradeItem> gradeItems = (List<GradeItem>)this.GetAllGradeItemsByCourse(courseId);

            var allItems = this.GetAllGradeItemGrades();
            foreach (var gradeItem in gradeItems)
            {
                var selectedItems = from gi in allItems
                                    where gi.assignmentID == gradeItem.AssignmentId
                                    select gi;

                var courseItems = selectedItems.ToList<GradeItemGrade>();
                grades = grades.Concat(courseItems).ToList();
            }

            return grades;
        }

        /// <summary>
        /// Gets the student grade item grades.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public IList<StudentGradeItemGrade> GetStudentGradeItemGrades(int courseId)
        {
            var courseRepo = new CourseRepository();
            List<StudentGradeItemGrade> studentGrades = new List<StudentGradeItemGrade>();
            List<GradeItemGrade> grades = (List<GradeItemGrade>)this.GetAllGradeItemGradeByCourse(courseId);
            foreach (var grade in grades)
            {
                var gradeItem = this.GetGradeItem(grade.assignmentID);
                var student = courseRepo.LocalGetStudentById(grade.studentID);
                var name = student.FirstName + " " + student.LastName;
                var gradeItemname = gradeItem.Name;
                var course = courseId;
                var oldgrade = grade.Grade;
                var studentGrade = new StudentGradeItemGrade(name,course.ToString(),gradeItemname,grade.assignmentID,grade.studentID, oldgrade);
                studentGrades.Add(studentGrade);
            }

            return studentGrades;
        }

        /// <summary>
        /// Gets the grade display items.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public IList<GradeItemGradeDisplay> GetGradeDisplayItems(int courseId)
        {
            var courseRepo = new CourseRepository();
            List<GradeItemGradeDisplay> grades = new List<GradeItemGradeDisplay>();
            List<GradeItem> gradeItems = (List<GradeItem>)this.GetAllGradeItemsByCourse(courseId);
            foreach (var item in gradeItems)
            {
                var ungraded = (List<GradeItemGrade>) this.GetAllUnGradedItemGradesById(item.AssignmentId);
                var graded = (List<GradeItemGrade>) this.GetAllGradedItemGradesById(item.AssignmentId);
                grades.Add(new GradeItemGradeDisplay(item, ungraded, graded));
            }

            return grades;

        }

        /// <summary>
        /// Gets all graded item grades.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        public IList<GradeItemGrade> GetAllGradedItemGrades(int course)
        {
            var allItems = this.GetAllGradeItemGradeByCourse(course);
            var selectedItems = from gi in allItems
                                where gi.Graded == true
                                select gi;

            var ungradedItems = selectedItems.ToList<GradeItemGrade>();
            return ungradedItems;
        }

        /// <summary>
        /// Gets all un graded item grades by identifier.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <returns></returns>
        public IList<GradeItemGrade> GetAllUnGradedItemGradesById(int assignmentId)
        {
            var allItems = this.GetAllGradeItemGrades();
            var selectedItems = from gi in allItems
                                where gi.assignmentID == assignmentId
                                      && gi.Graded == false 
                                    select gi;

            var ungradedItems = selectedItems.ToList<GradeItemGrade>();
            return ungradedItems;
        }

        /// <summary>
        /// Gets all graded item grades by identifier.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <returns></returns>
        public IList<GradeItemGrade> GetAllGradedItemGradesById(int assignmentId)
        {
            var allItems = this.GetAllGradeItemGrades();
            var selectedItems = from gi in allItems
                                where gi.assignmentID == assignmentId
                                      && gi.Graded == true
                                select gi;

            var ungradedItems = selectedItems.ToList<GradeItemGrade>();
            return ungradedItems;
        }

        /// <summary>
        /// Creates all student grade item grades display models.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public IList<StudentGradeItemGrade> CreateAllStudentGradeItemGrades(List<GradeItemGrade> items, int courseId)
        {
            List<StudentGradeItemGrade> displayItems = new List<StudentGradeItemGrade>();
            foreach (var item in items)
            {
                displayItems.Add(this.CreateStudentGradeItemGrade(item, courseId));
            }

            return displayItems;
        }

        /// <summary>
        /// Creates the student grade item grade display model.
        /// </summary>
        /// <param name="grade">The grade.</param>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public StudentGradeItemGrade CreateStudentGradeItemGrade(GradeItemGrade grade, int courseId)
        {
            var courseRepo = new CourseRepository();
            var gradeItem = this.GetGradeItem(grade.assignmentID);
            var student = courseRepo.LocalGetStudentById(grade.studentID);
            var name = student.FirstName + " " + student.LastName;
            var gradeItemname = gradeItem.Name;
            var course = courseId;
            var oldgrade = grade.Grade;
            var studentGrade = new StudentGradeItemGrade(name, course.ToString(), gradeItemname, grade.assignmentID, grade.studentID, oldgrade);
            return studentGrade;
        }

        /// <summary>
        /// Writes the new grade item to local database.
        /// </summary>
        /// <param name="gradeItem">The grade item.</param>
        public async void WriteNewGradeItemsToLocalDb(GradeItem gradeItem)
        {
            CourseRepository courseRepo = new CourseRepository();
            
            var sqlInsert = "INSERT INTO `Course_grade_item`(`cid`, `name`, `description`, `grade_type`, `max_grade`)"
                            + " VALUES (@cid, @name, @description, @grade_type, @max_grade);";
            int rowId = 0;
            using (SqliteConnection db = DataAccess.LocalConnection)
                {
                db.Open();
                var transaction = db.BeginTransaction();
                var createGradeItemCommand = db.CreateCommand();
                createGradeItemCommand.Connection = db;
                createGradeItemCommand.Transaction = transaction;
                try
                {
                    createGradeItemCommand.CommandText = sqlInsert;
                    createGradeItemCommand.Parameters.Add("@cid", SqliteType.Integer, 11);
                    createGradeItemCommand.Parameters["@cid"].Value = gradeItem.CourseId;
                    createGradeItemCommand.Parameters.Add("@name", SqliteType.Text, 60);
                    createGradeItemCommand.Parameters["@name"].Value = gradeItem.Name;
                    createGradeItemCommand.Parameters.Add("@description", SqliteType.Text, 600);
                    createGradeItemCommand.Parameters["@description"].Value = gradeItem.Description;
                    createGradeItemCommand.Parameters.Add("@grade_type", SqliteType.Text, 60);
                    createGradeItemCommand.Parameters["@grade_type"].Value = gradeItem.Type;
                    createGradeItemCommand.Parameters.Add("@max_grade", SqliteType.Integer, 11);
                    createGradeItemCommand.Parameters["@max_grade"].Value = gradeItem.MaxGrade;
                    createGradeItemCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
                var cmd = db.CreateCommand();
                cmd.CommandText = "SELECT last_insert_rowid()";
                object i = cmd.ExecuteScalar();

                cmd.CommandText = "SELECT `aid` FROM `Course_grade_item` WHERE rowid=" + i.ToString();
                i = cmd.ExecuteScalar();
                rowId = (int)(long)i;
                }

            var sqlchange = "INSERT INTO `Course_grade_item`(`cid`, `name`, `description`, `grade_type`, `max_grade`)"
                            + " VALUES (" + gradeItem.CourseId + ",'" + gradeItem.Name +"','"+ gradeItem.Description
                            +"','" + gradeItem.Type + "'," + gradeItem.MaxGrade +");";
            await ChangelogModifier.AppendChangeLog(sqlchange);

            var students = courseRepo.GetAllStudentsByCourseLocalDb(gradeItem.CourseId);
            foreach (var student in students)
            {
                await this.AddGradeItemGrade(rowId, student.Id);
            }
        }

        /// <summary>
        /// Edits the grade item in local database.
        /// </summary>
        /// <param name="gradeItem">The grade item.</param>
        public async void EditGradeItemInLocalDb(GradeItem gradeItem)
        {
            const string sqlStatementUpdateById =
                "UPDATE `Course_grade_item` SET `name`= @name, `description` = @description, `grade_type` = @type, `max_grade` = @grade WHERE aid = @aid";
            using (SqliteConnection db = DataAccess.LocalConnection)
                {
                db.Open();
                var transaction = db.BeginTransaction();
                var editGradeItemCommand = db.CreateCommand();
                editGradeItemCommand.Connection = db;
                editGradeItemCommand.Transaction = transaction;
                try
                {
                    editGradeItemCommand.CommandText = sqlStatementUpdateById;
                    editGradeItemCommand.Parameters.Add("@aid", SqliteType.Integer, 11);
                    editGradeItemCommand.Parameters["@aid"].Value = gradeItem.AssignmentId;
                    editGradeItemCommand.Parameters.Add("@name", SqliteType.Text, 60);
                    editGradeItemCommand.Parameters["@name"].Value = gradeItem.Name;
                    editGradeItemCommand.Parameters.Add("@description", SqliteType.Text, 600);
                    editGradeItemCommand.Parameters["@description"].Value = gradeItem.Description;
                    editGradeItemCommand.Parameters.Add("@type", SqliteType.Text, 60);
                    editGradeItemCommand.Parameters["@type"].Value = gradeItem.Type;
                    editGradeItemCommand.Parameters.Add("@grade", SqliteType.Integer, 11);
                    editGradeItemCommand.Parameters["@grade"].Value = gradeItem.MaxGrade;
                    editGradeItemCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            var sqlStatementUpdate = "UPDATE `Course_grade_item` SET `name`= '" + gradeItem.Name +
                                     "', `description` = '" + gradeItem.Description + "', `grade_type` = '" + gradeItem.Type + "', " +
                                     "`max_grade` = " + gradeItem.MaxGrade + " WHERE aid = " + gradeItem.AssignmentId + ";";
            await ChangelogModifier.AppendChangeLog(sqlStatementUpdate);
        }

        /// <summary>
        /// Deletes the grade item from local database.
        /// </summary>
        /// <param name="gradeItem">The grade item.</param>
        public async void DeleteGradeItemFromLocalDb(GradeItem gradeItem)
        {
            const string sqlStatementDeleteById =
                "DELETE FROM `Course_grade_item` WHERE aid = @id";
            using (SqliteConnection db = DataAccess.LocalConnection)
                {
                db.Open();
                var query = db.CreateCommand();
                query.CommandText = sqlStatementDeleteById;
                query.Parameters.Add("@id", SqliteType.Integer, 11);
                query.Parameters["@id"].Value = gradeItem.AssignmentId;
                query.ExecuteNonQuery();
                db.Close();
                }

            var dbStatementDeleteById =
                "DELETE FROM `Course_grade_item` WHERE aid = " + gradeItem.AssignmentId + " ;";
            await ChangelogModifier.AppendChangeLog(dbStatementDeleteById);
            
        }

        /// <summary>
        /// Adds the grade item grade.
        /// </summary>
        /// <param name="assignmentId">The assignment identifier.</param>
        /// <param name="studentId">The student identifier.</param>
        public async Task AddGradeItemGrade(int assignmentId, int studentId)
        {
            const string sqlStatementGradeItemGrade =
                "INSERT INTO Course_student_grade_item_grade(`aid`, `sid`, `grade`, `graded`) VALUES(@aid, @sid, @grade, @graded);";
            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                db.Open();
                var transaction = db.BeginTransaction();
                var createStudentGradeItemCommand = db.CreateCommand();
                createStudentGradeItemCommand.Connection = db;
                createStudentGradeItemCommand.Transaction = transaction;
                createStudentGradeItemCommand.CommandText = sqlStatementGradeItemGrade;
                createStudentGradeItemCommand.Parameters.Add("@aid", SqliteType.Integer, 11);
                createStudentGradeItemCommand.Parameters["@aid"].Value = assignmentId;
                createStudentGradeItemCommand.Parameters.Add("@sid", SqliteType.Integer, 11);
                createStudentGradeItemCommand.Parameters["@sid"].Value = studentId;
                createStudentGradeItemCommand.Parameters.Add("@grade", SqliteType.Integer, 4);
                createStudentGradeItemCommand.Parameters["@grade"].Value = 0;
                createStudentGradeItemCommand.Parameters.Add("@graded", SqliteType.Integer, 1);
                createStudentGradeItemCommand.Parameters["@graded"].Value = 0;
                createStudentGradeItemCommand.ExecuteNonQuery();
                transaction.Commit();
            }
            var dbQuery = "INSERT INTO `Course_student_grade_item_grade`(`aid`, `sid`, `grade`, `graded`) VALUES(" + assignmentId +"," + studentId +", 0, 0);";
            await ChangelogModifier.AppendChangeLog(dbQuery);
        }

        /// <summary>
        /// Updates the student course grade.
        /// </summary>
        /// <param name="studentId">The student identifier.</param>
        /// <param name="courseId">The course identifier.</param>
        public async void UpdateStudentCourseGrade(int studentId, int courseId)
        {
           List<GradeItemGrade> allGrades = (List<GradeItemGrade>)this.GetAllGradeItemGradeByCourse(courseId);
           var studentGrades = from gi in allGrades
                                where gi.studentID == studentId
                                select gi;
            double totalstudentgrade = 0;
            var rubricRepo = new RubricRepository();
            var types = rubricRepo.GetAllRubricItems(courseId);
            List<GradeItemGrade> gradeList = studentGrades.ToList<GradeItemGrade>();
            foreach (var type in types)
            {
                totalstudentgrade += this.CalculateTypeGrade(gradeList, type);
            }

            const string sqlStatementUpdateByIds =
                "UPDATE `Course_student_participant_grade` SET `cid`= @cid, `sid` = @sid, `grade` = @grade WHERE cid = @cid AND sid = @sid ";
            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                db.Open();
                var transaction = db.BeginTransaction();
                var editGradeItemGradeCommand = db.CreateCommand();
                editGradeItemGradeCommand.Connection = db;
                editGradeItemGradeCommand.Transaction = transaction;
                try
                {
                    editGradeItemGradeCommand.CommandText = sqlStatementUpdateByIds;
                    editGradeItemGradeCommand.Parameters.Add("@grade", SqliteType.Real);
                    editGradeItemGradeCommand.Parameters["@grade"].Value = totalstudentgrade;
                    editGradeItemGradeCommand.Parameters.Add("@cid", SqliteType.Integer, 11);
                    editGradeItemGradeCommand.Parameters["@cid"].Value = courseId;
                    editGradeItemGradeCommand.Parameters.Add("@sid", SqliteType.Integer, 11);
                    editGradeItemGradeCommand.Parameters["@sid"].Value = studentId;
                    editGradeItemGradeCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            var sqlStatementUpdate =
                "UPDATE `Course_student_participant_grade` SET `cid`=" + courseId + ", `sid` = " +
                + studentId + ", `grade` = " + totalstudentgrade + " WHERE cid = " + courseId + " AND sid = " + studentId + " ";
           await ChangelogModifier.AppendChangeLog(sqlStatementUpdate);

        }

        /// <summary>
        /// Calculates the grade for the rubric type.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="rItem">The rubric item.</param>
        /// <returns></returns>
        public double CalculateTypeGrade(List<GradeItemGrade> items, RubricItem rItem)
        {
            int student = 0;
            int max = 0;

            foreach (var grade in items)
            {
                var gItem = this.GetGradeItem(grade.assignmentID);
                if (string.Equals(gItem.Type, rItem.AssignmentType, StringComparison.OrdinalIgnoreCase))
                {
                    if (grade.Graded == true)
                    {
                        student += grade.Grade;
                        max += this.GetGradeItem(grade.assignmentID).MaxGrade;
                    }
                }
            }

            double total = 0;
            if (max != 0)
            {
                total = (double)student / (double)max;
                total = total * 100.0;
                total = total * (double)rItem.GradeWeight;
            }
            return total;
        }

    }
}
