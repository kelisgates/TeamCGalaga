using System;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Galaga.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Galaga.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameCanvas 
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCanvas"/> class.
        /// </summary>
        public GameCanvas()
        {
            
            this.InitializeComponent();            
            Width = this.canvas.Width;
            Height= this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));

            GameManagerViewModel viewModel = new GameManagerViewModel(this.canvas);
            DataContext = viewModel;
            _ = viewModel.DisplayStartScreen();
        }

        private void setWindowTitle(string title)
        {
            var appView = ApplicationView.GetForCurrentView();
            appView.Title = String.Empty;
            appView.Title = title;
        }


        #endregion

        
    }
}
