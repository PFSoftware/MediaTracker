using PersonalTracker.Media.Models.MediaTypes;
using PFSoftware.Extensions;
using PFSoftware.Extensions.Enums;
using PFSoftware.MediaTracker.Models.Database;
using PFSoftware.MediaTracker.Views;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PFSoftware.MediaTracker.Models
{
    public static class AppState
    {
        public static List<Series> AllSeries = new List<Series>();
        public static bool Loaded = false;

        #region Database Interaction

        private static readonly SQLiteDatabaseInteraction DatabaseInteraction = new SQLiteDatabaseInteraction();

        /// <summary>Handles verification of required files.</summary>
        internal static async Task FileManagement()
        {
            if (!Directory.Exists(AppData.Location))
                Directory.CreateDirectory(AppData.Location);
            DatabaseInteraction.VerifyDatabaseIntegrity();
            await LoadAll();
        }

        #endregion Database Interaction

        #region Navigation

        /// <summary>Instance of MainWindow currently loaded</summary>
        public static MainWindow MainWindow { get; set; }

        /// <summary>Navigates to selected Page.</summary>
        /// <param name="newPage">Page to navigate to.</param>
        public static void Navigate(Page newPage) => MainWindow.MainFrame.Navigate(newPage);

        /// <summary>Navigates to the previous Page.</summary>
        public static void GoBack()
        {
            if (MainWindow.MainFrame.CanGoBack)
                MainWindow.MainFrame.GoBack();
        }

        #endregion Navigation

        #region Television

        #region Delete

        /// <summary>Deletes a <see cref="Series"/> from the database.</summary>
        /// <param name="deleteSeries"><see cref="Series"/> to be deleted</param>
        /// <returns>True if successful</returns>
        public static async Task<bool> DeleteSeries(Series deleteSeries)
        {
            if (YesNoNotification($"Are you sure you want to delete {deleteSeries.Name}? This action cannot be undone.",
              "Personal Tracker"))
            {
                if (await DatabaseInteraction.DeleteSeries(deleteSeries))
                {
                    AllSeries.Remove(deleteSeries);
                    return true;
                }
                DisplayNotification($"Unable to delete {deleteSeries.Name}.", "Personal Tracker");
            }
            return false;
        }

        #endregion Delete

        #region Load

        public static async Task LoadAll() => await LoadSeries();

        /// <summary>Loads all <see cref="Series"/> from the database.</summary>
        /// <returns>All Series</returns>
        public static async Task LoadSeries()
        {
            AllSeries = await DatabaseInteraction.LoadSeries();
        }

        #endregion Load

        #region Save

        /// <summary>Modifies a <see cref="Series"/> in the database.</summary>
        /// <param name="oldSeries">Original <see cref="Series"/></param>
        /// <param name="newSeries"><see cref="Series"/> to replace original</param>
        /// <returns>True if successful</returns>
        public static async Task<bool> ModifySeries(Series oldSeries, Series newSeries)
        {
            if (await DatabaseInteraction.ModifySeries(oldSeries, newSeries))
            {
                AllSeries.Replace(oldSeries, newSeries);
                return true;
            }
            DisplayNotification("Unable to modify television series.", "Personal Tracker");
            return false;
        }

        /// <summary>Saves a new <see cref="Series"/> to the database.</summary>
        /// <param name="newSeries"><see cref="Series"/> to be saved</param>
        /// <returns>True if successful</returns>
        public static async Task<bool> NewSeries(Series newSeries)
        {
            if (await DatabaseInteraction.NewSeries(newSeries))
            {
                AllSeries.Add(newSeries);
                return true;
            }
            DisplayNotification("Unable to add new television series.", "Personal Tracker");
            return false;
        }

        #endregion Save

        #endregion Television

        #region Notification Management

        /// <summary>Displays a new Notification in a thread-safe way.</summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Title of the Notification window</param>
        public static void DisplayNotification(string message, string title) => Application.Current.Dispatcher.Invoke(
            () => new Notification(message, title, NotificationButton.OK, MainWindow).ShowDialog());

        /// <summary>Displays a new Notification in a thread-safe way and retrieves a boolean result upon its closing.</summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="title">Title of the Notification window</param>
        /// <returns>Returns value of clicked button on Notification.</returns>
        public static bool YesNoNotification(string message, string title) => Application.Current.Dispatcher.Invoke(() => (new Notification(message, title, NotificationButton.YesNo, MainWindow).ShowDialog() == true));

        #endregion Notification Management
    }
}