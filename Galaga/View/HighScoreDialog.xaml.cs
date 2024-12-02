using Galaga.Model;
using Galaga.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Galaga.View
{
    public sealed partial class HighScoreDialog : ContentDialog
    {
        
        public HighScoreDialog()
        {
            this.InitializeComponent();
        }

        private void SortByScoreNameLevel_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (GameManagerViewModel)DataContext;
            viewModel.SortByScoreNameLevel();
            this.UpdateSortedHighScores(viewModel);
        }

        private void SortByNameScoreLevel_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (GameManagerViewModel)DataContext;
            viewModel.SortByNameScoreLevel();
            this.UpdateSortedHighScores(viewModel);
        }

        private void SortByLevelScoreName_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (GameManagerViewModel)DataContext;
            viewModel.SortByLevelScoreName();
            this.UpdateSortedHighScores(viewModel);
        }

        private void UpdateSortedHighScores(GameManagerViewModel viewModel)
        {
            var collectionViewSource = (CollectionViewSource)Resources["SortedHighScores"];
            collectionViewSource.Source = viewModel.HighScores;
        }

    }
}
