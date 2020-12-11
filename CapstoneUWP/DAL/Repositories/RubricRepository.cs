using System;
using System.Collections.Generic;
using System.Linq;
using CapstoneUWP.Model;
using CapstoneUWP.Model.Repo_Model;
using DataAccessLibrary;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace CapstoneUWP.DAL.Repositories
{
    public class RubricRepository
    {
        private readonly MySqlConnection dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RubricRepository"/> class.
        /// </summary>
        public RubricRepository()
        {
            this.dbConnection = DbConnection.getConnection();
        }

        /// <summary>
        /// Gets all rubric items.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public List<RubricItem> GetAllRubricItems(int courseId)
        {
            List<RubricItem> courseItems = new List<RubricItem>();

            const string sqlStatementGetAll =
                "SELECT rid, cid, assignment_type, grade_percentage FROM Course_rubric WHERE cid = @id";

            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                db.Open();
                var query = db.CreateCommand();
                query.CommandText = sqlStatementGetAll;
                query.Parameters.Add(new SqliteParameter("@id", courseId));
                using (var reader = query.ExecuteReader())
                    while (reader.Read())
                        courseItems.Add(new RubricItem
                        {
                            RubricId = reader.GetInt32(0),
                            CourseId = reader.GetInt32(1),
                            AssignmentType = reader.GetString(2),
                            GradeWeight = reader.GetDouble(3)
                        });
            }
            return courseItems;
        }

        /// <summary>
        /// Adds the rubric item.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="grade">The grade.</param>
        public async void AddRubricItem(int courseId, string type, double grade)
        {
            const string sqlStatementAddRubricItem =
                "INSERT INTO `Course_rubric` (`cid`, `assignment_type`, `grade_percentage`) VALUES (@cid, @type, @grade);";
            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                db.Open();
                var transaction = db.BeginTransaction();
                var addRubricCommand = db.CreateCommand();
                addRubricCommand.Connection = db;
                addRubricCommand.Transaction = transaction;
                try
                {
                    addRubricCommand.CommandText = sqlStatementAddRubricItem;
                    addRubricCommand.Parameters.Add("@cid", SqliteType.Integer, 11);
                    addRubricCommand.Parameters["@cid"].Value = courseId;
                    addRubricCommand.Parameters.Add("@type", SqliteType.Text, 60);
                    addRubricCommand.Parameters["@type"].Value = type;
                    addRubricCommand.Parameters.Add("@grade", SqliteType.Real);
                    addRubricCommand.Parameters["@grade"].Value = grade;
                    addRubricCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }

            var sqlStatementAdd = "INSERT INTO `Course_rubric` (`rid`, `cid`, `assignment_type`, `grade_percentage`) VALUES" +
                                  " (" + 0 + "," + courseId + ",'" + type + "'," + grade + ");";
            await ChangelogModifier.AppendChangeLog(sqlStatementAdd);

        }

        /// <summary>
        /// Edits the rubric item.
        /// </summary>
        /// <param name="rubricId">The rubric identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="grade">The grade.</param>
        public async void EditRubricItem(int rubricId, string type, double grade)
        {
            const string sqlStatementUpdateById =
                "UPDATE `Course_rubric` SET `assignment_type`= @type, `grade_percentage` = @grade WHERE rid = @id";
            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                db.Open();
                var transaction = db.BeginTransaction();
                var editRubricCommand = db.CreateCommand();
                editRubricCommand.Connection = db;
                editRubricCommand.Transaction = transaction;
                try
                {
                    editRubricCommand.CommandText = sqlStatementUpdateById;
                    editRubricCommand.Parameters.Add("@id", SqliteType.Integer, 11);
                    editRubricCommand.Parameters["@id"].Value = rubricId;
                    editRubricCommand.Parameters.Add("@type", SqliteType.Text, 60);
                    editRubricCommand.Parameters["@type"].Value = type;
                    editRubricCommand.Parameters.Add("@grade", SqliteType.Real);
                    editRubricCommand.Parameters["@grade"].Value = grade;
                    editRubricCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            var sqlStatementUpdate = "UPDATE `Course_rubric` SET `assignment_type`= '" + type +
                                     "',`grade_percentage` = " + grade + " WHERE rid = " + rubricId;
            await ChangelogModifier.AppendChangeLog(sqlStatementUpdate);
        }

        /// <summary>
        /// Deletes the rubric item.
        /// </summary>
        /// <param name="rubricId">The rubric identifier.</param>
        public async void DeleteRubricItem(int rubricId)
        {
            const string sqlStatementDeleteById =
                "DELETE FROM `Course_rubric` WHERE rid = @id";
            using (SqliteConnection db = DataAccess.LocalConnection)
            {
                db.Open();
                var query = db.CreateCommand();
                query.CommandText = sqlStatementDeleteById;
                query.Parameters.Add("@id", SqliteType.Integer, 11);
                query.Parameters["@id"].Value = rubricId;
                query.ExecuteNonQuery();
                this.dbConnection.Close();
            }

            var dbStatementDeleteById =
                "DELETE FROM `Course_rubric` WHERE rid = " + rubricId + " ;";
            await ChangelogModifier.AppendChangeLog(dbStatementDeleteById);
        }

        /// <summary>
        /// Gets the rubric item types.
        /// </summary>
        /// <param name="rubricItems">The rubric items.</param>
        /// <returns></returns>
        public List<String> GetTypes(List<RubricItem> rubricItems)
        {
            List<String> types = new List<string>();
            foreach (var item in rubricItems)
            {
                types.Add(item.AssignmentType);
            }
            var uniquetypes = types.Distinct().ToList();
            return uniquetypes;
        }

        /// <summary>
        /// Gets the percentage total from all rubric item grade percentages.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        public double GetRubricPercentageTotal(int courseId)
        {
            double total = 0;
            var items = this.GetAllRubricItems(courseId);
            foreach (var item in items)
            {
                total += item.GradeWeight;
            }

            return total;
        }

    }
}
