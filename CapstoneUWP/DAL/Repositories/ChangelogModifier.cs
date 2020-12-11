using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using DataAccessLibrary;
using MySql.Data.MySqlClient;

namespace CapstoneUWP.DAL.Repositories
{
    public static class ChangelogModifier
    {
        /// <summary>
        /// Creates the change log.
        /// </summary>
        public static async void CreateChangeLog()
        {
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            await storageFolder.CreateFileAsync("Changelog.txt");
        }

        /// <summary>
        /// Deletes the changelog.
        /// </summary>
        public static async void DeleteChangelog()
        {
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file =
                await storageFolder.GetFileAsync("Changelog.txt");
            await file.DeleteAsync(StorageDeleteOption.Default);
        }

        /// <summary>
        /// Appends the change log with a new query.
        /// </summary>
        /// <param name="query">The query.</param>
        public static async Task AppendChangeLog(String query)
        {
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.GetFileAsync("Changelog.txt");
            await FileIO.AppendTextAsync(sampleFile,
                query + Environment.NewLine);
        }

        /// <summary>
        /// Outputs the changes to online db.
        /// </summary>
        /// <returns></returns>
        public static async Task OutputChanges()
        {
                MySqlConnection dbConnection = DbConnection.getConnection();
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file =
                    await storageFolder.GetFileAsync("Changelog.txt");
                IList<string> data = await FileIO.ReadLinesAsync(file);
                List<string> queries = data.ToList();
                foreach (var queryItem in queries)
                {
                    using (dbConnection)
                    {

                        try
                        {
                            dbConnection.Open();
                            var query = dbConnection.CreateCommand();
                            query.CommandText = queryItem;
                            query.ExecuteReader();
                            dbConnection.Close();
                        }
                        catch (MySqlException)
                        {
                            break;
                        }
                    }
                }

                using (dbConnection)
                {
                    try
                    {
                        dbConnection.Open();
                        await file.DeleteAsync(StorageDeleteOption.Default);
                        DataAccess.InitializeDatabase();
                        dbConnection.Close();
                    }
                    catch (Exception)
                    {

                    }
                }
        }
    }
}
