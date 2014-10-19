using Game2048.Game.ViewModels.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace Game2048.Game.Views
{
    /// <summary>
    /// Interaction logic for GameShell.xaml
    /// </summary>
    public partial class GameShell : Window
    {
        public GameShell()
        {
            InitializeComponent();
        }

        public GameShell(IGameViewModel gameViewModel, WebBrowser browserManager)
        {
            InitializeComponent();
            DataContext = gameViewModel;
            host.Content = browserManager;
        }
    }
}
