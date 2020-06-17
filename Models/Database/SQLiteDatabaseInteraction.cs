using PFSoftware.Extensions;
using PFSoftware.Extensions.DatabaseHelp;
using PFSoftware.Extensions.DataTypeHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using PersonalTracker.Media.Models.MediaTypes;
using PersonalTracker.Media.Models.Enums;

namespace PFSoftware.MediaTracker.Models.Database
{
    public class SQLiteDatabaseInteraction
    {
        private const string _DATABASENAME = "MediaTracker.sqlite";
        private readonly string _con = $"Data Source={DatabaseLocation}; foreign keys = TRUE; Version = 3;";
        private static readonly string DatabaseLocation = Path.Combine(AppData.Location, _DATABASENAME);

        #region Database Interaction

        /// <summary>Verifies that the requested database exists and that its file size is greater than zero. If not, it extracts the embedded database file to the local output folder.</summary>
        public void VerifyDatabaseIntegrity() => Functions.VerifyFileIntegrity(
            Assembly.GetExecutingAssembly().GetManifestResourceStream($"PFSoftware.MediaTracker.{_DATABASENAME}"), _DATABASENAME, AppData.Location);

        #endregion Database Interaction

        #region Books

        /// <summary>Deletes a <see cref="Book"/> from the database.</summary>
        /// <param name="deleteBook"><see cref="Book"/> to be deleted</param>
        /// <returns>True if successful</returns>
        public Task<bool> DeleteBook(Book deleteBook)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "DELETE FROM Books WHERE [Name] = @name AND [Author] = @author" };
            cmd.Parameters.AddWithValue("@name", deleteBook.Name);
            cmd.Parameters.AddWithValue("@author", deleteBook.Author);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        /// <summary>Loads all <see cref="Book"/>s from the database.</summary>
        /// <returns>All <see cref="Book"/>s</returns>
        public async Task<List<Book>> LoadBooks()
        {
            List<Book> allBooks = new List<Book>();
            DataSet ds = await SQLiteHelper.FillDataSet(_con, "SELECT * FROM Books");

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                    allBooks.Add(new Book(dr["Name"].ToString(), dr["Author"].ToString(), dr["Series"].ToString(), DecimalHelper.Parse(dr["Number"]), DecimalHelper.Parse(dr["Rating"]), Int32Helper.Parse(dr["Year"])));
            }

            return allBooks;
        }

        /// <summary>Modifies a <see cref="Book"/> in the database.</summary>
        /// <param name="oldBook">Original <see cref="Book"/></param>
        /// <param name="newBook"><see cref="Book"/> to replace original</param>
        /// <returns>True if successful</returns>
        public Task<bool> ModifyBook(Book oldBook, Book newBook)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "UPDATE Books SET [Name] = @newName, [Author] = @newAuthor, [Series] = @newSeries, [Rating] = @newRating, [Year] = @newYear WHERE [Name] = @oldName AND [Author] = @oldAuthor" };
            cmd.Parameters.AddWithValue("@newName", newBook.Name);
            cmd.Parameters.AddWithValue("@newAuthor", newBook.Author);
            cmd.Parameters.AddWithValue("@newSeries", newBook.Series);
            cmd.Parameters.AddWithValue("@newRating", newBook.Rating);
            cmd.Parameters.AddWithValue("@newYear", newBook.Year);
            cmd.Parameters.AddWithValue("@oldName", oldBook.Name);
            cmd.Parameters.AddWithValue("@oldAuthor", oldBook.Author);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        /// <summary>Saves a new <see cref="Book"/> to the database.</summary>
        /// <param name="newBook"><see cref="Book"/> to be saved</param>
        public Task<bool> NewBook(Book newBook)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "INSERT INTO Books([Name], [Author], [Series], [Rating], [Year]) VALUES(@name, @author, @series, @rating, @year)" };
            cmd.Parameters.AddWithValue("@name", newBook.Name);
            cmd.Parameters.AddWithValue("@author", newBook.Author);
            cmd.Parameters.AddWithValue("@series", newBook.Series);
            cmd.Parameters.AddWithValue("@rating", newBook.Rating);
            cmd.Parameters.AddWithValue("@year", newBook.Year);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        #endregion Books

        #region Television

        #region Delete

        /// <summary>Deletes a <see cref="Television"/> from the database.</summary>
        /// <param name="deleteSeries"><see cref="Television"/> to be deleted</param>
        /// <returns>True if successful</returns>
        public Task<bool> DeleteSeries(Series deleteSeries)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "DELETE FROM Television WHERE [Name] = @name AND [PremiereDate] = @date" };
            cmd.Parameters.AddWithValue("@name", deleteSeries.Name);
            cmd.Parameters.AddWithValue("@date", deleteSeries.PremiereDateToString);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        #endregion Delete

        #region Load

        /// <summary>Loads all <see cref="Television"/> from the database.</summary>
        /// <returns>All <see cref="Television"/></returns>
        public async Task<List<Series>> LoadSeries()
        {
            List<Series> allTelevision = new List<Series>();
            DataSet ds = await SQLiteHelper.FillDataSet(_con, "SELECT * FROM Television");

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                    allTelevision.Add(new Series(dr["Name"].ToString(), DateTimeHelper.Parse(dr["PremiereDate"]), DecimalHelper.Parse(dr["Rating"]), Int32Helper.Parse(dr["Seasons"]), Int32Helper.Parse(dr["Episodes"]), (SeriesStatus)Int32Helper.Parse(dr["Status"]), dr["Channel"].ToString(), DateTimeHelper.Parse(dr["FinaleDate"]), (DayOfWeek)Int32Helper.Parse(dr["Day"]), DateTimeHelper.Parse(dr["Time"]), dr["ReturnDate"].ToString()));
            }

            return allTelevision;
        }

        #endregion Load

        #region Save

        /// <summary>Modifies a <see cref="Television"/> in the database.</summary>
        /// <param name="oldSeries">Original <see cref="Television"/></param>
        /// <param name="newSeries"><see cref="Television"/> to replace original</param>
        /// <returns>True if successful</returns>
        public Task<bool> ModifySeries(Series oldSeries, Series newSeries)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "UPDATE Television SET [Name] = @name, [PremiereDate] = @premiereDate, [Rating] = @rating, [Seasons] = @seasons, [Episodes] = @episodes, [Status] = @status, [Channel] = @channel, [FinaleDate] = @finaleDate, [Day] = @day, [Time] = @time, [ReturnDate] = @returnDate WHERE [Name] = @oldName" };
            cmd.Parameters.AddWithValue("@name", newSeries.Name);
            cmd.Parameters.AddWithValue("@premiereDate", newSeries.PremiereDateToString);
            cmd.Parameters.AddWithValue("@rating", newSeries.Rating);
            cmd.Parameters.AddWithValue("@seasons", newSeries.Seasons);
            cmd.Parameters.AddWithValue("@episodes", newSeries.Episodes);
            cmd.Parameters.AddWithValue("@status", Int32Helper.Parse(newSeries.Status));
            cmd.Parameters.AddWithValue("@channel", newSeries.Channel);
            cmd.Parameters.AddWithValue("@finaleDate", newSeries.FinaleDateToString);
            cmd.Parameters.AddWithValue("@day", Int32Helper.Parse(newSeries.Day));
            cmd.Parameters.AddWithValue("@time", newSeries.TimeToString);
            cmd.Parameters.AddWithValue("@returnDate", newSeries.ReturnDate);
            cmd.Parameters.AddWithValue("@oldName", oldSeries.Name);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        /// <summary>Saves a new <see cref="Television"/> to the database.</summary>
        /// <param name="newSeries"><see cref="Television"/> to be saved</param>
        /// <returns>True if successful</returns>
        public Task<bool> NewSeries(Series newSeries)
        {
            SQLiteCommand cmd = new SQLiteCommand { CommandText = "INSERT INTO Television([Name], [PremiereDate], [Rating], [Seasons], [Episodes], [Status], [Channel], [FinaleDate], [Day], [Time], [ReturnDate]) VALUES(@name, @premiereDate, @rating, @seasons, @episodes, @status, @channel, @finaleDate, @day, @time, @returnDate)" };
            cmd.Parameters.AddWithValue("@name", newSeries.Name);
            cmd.Parameters.AddWithValue("@premiereDate", newSeries.PremiereDateToString);
            cmd.Parameters.AddWithValue("@rating", newSeries.Rating);
            cmd.Parameters.AddWithValue("@seasons", newSeries.Seasons);
            cmd.Parameters.AddWithValue("@episodes", newSeries.Episodes);
            cmd.Parameters.AddWithValue("@status", Int32Helper.Parse(newSeries.Status));
            cmd.Parameters.AddWithValue("@channel", newSeries.Channel);
            cmd.Parameters.AddWithValue("@finaleDate", newSeries.FinaleDateToString);
            cmd.Parameters.AddWithValue("@day", Int32Helper.Parse(newSeries.Day));
            cmd.Parameters.AddWithValue("@time", newSeries.TimeToString);
            cmd.Parameters.AddWithValue("@returnDate", newSeries.ReturnDate);

            return SQLiteHelper.ExecuteCommand(_con, cmd);
        }

        #endregion Save

        #endregion Television
    }
}