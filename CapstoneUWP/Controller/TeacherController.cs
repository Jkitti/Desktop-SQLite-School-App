using System;
using CapstoneUWP.DAL.Repositories;

namespace CapstoneUWP.Controller
{
    public class TeacherController
    {

        private TeacherRepository repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeacherController"/> class.
        /// </summary>
        public TeacherController()
        {
            this.repo = new TeacherRepository();
        }

        /// <summary>
        /// Writes the credentials.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pass">The pass.</param>
        public void WriteCredentials(int id, string pass)
        {
            this.repo.WriteCredentialsToLocalDb(id, pass);
        }

        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pass">The pass.</param>
        /// <returns></returns>
        public bool ValidateCredentials(int id, string pass)
        {
            return this.repo.ValidateCredentials(id, pass);
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public String GetUserName(int id)
        {
            return this.repo.GetUserName(id);
        }

        /// <summary>
        /// Checks for local data.
        /// </summary>
        /// <returns></returns>
        public bool CheckForLocalData()
        {
            return this.repo.CheckForLocalData();
        }
    }
}
