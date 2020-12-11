
using MySql.Data.MySqlClient;

namespace CapstoneUWP.DAL
{
    /// <summary>
    /// Class for the database connection
    /// </summary>
    public class DbConnection
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns>The database connection</returns>
        public static MySqlConnection getConnection()
        {
            MySqlConnection connection = null;

            var conStr = "server=160.10.25.16; port=3306; uid=cs4982s19c;" +
                         "pwd=sBoMPq5yiZ2DB9nv;database=cs4982s19c; convert zero datetime=True";

            using (var conn = new MySqlConnection(conStr))
            {
                connection = conn;
            }

            return connection;
        }
    }
}
