using PersonalTracker.Media.Views.MediaSeries;
using PFSoftware.MediaTracker.Models;
using System.Windows;

namespace PersonalTracker.Media.Views
{
    /// <summary> Interaction logic for MediaPage.xaml </summary>
    public partial class MediaPage
    {
        //TODO Implement other types of Media: film, books, and music.

        #region Click

        private void BtnBooks_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnFilms_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnMusic_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnTelevision_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new TelevisionPage());

        #endregion Click

        #region Page Manipulation

        public MediaPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppState.Loaded)
                await AppState.FileManagement();
        }

        #endregion Page Manipulation
    }
}