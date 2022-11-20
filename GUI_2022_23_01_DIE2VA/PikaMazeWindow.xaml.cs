using GUI_2022_23_01_DIE2VA.Controller;
using GUI_2022_23_01_DIE2VA.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace gameszko
{
    /// <summary>
    /// Interaction logic for PikaMazeWindow.xaml
    /// </summary>
    public partial class PikaMazeWindow : Window
    {
        GameController controller;
        GameLogic logic;
        public PikaMazeWindow()
        {
            InitializeComponent();
            logic = new GameLogic();
            display.SetupModel(logic);
            controller = new GameController(logic);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            display.Resize(new Size(grid.ActualWidth, grid.ActualHeight));
            display.InvalidateVisual();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            display.Resize(new Size(grid.ActualWidth, grid.ActualHeight));
            display.InvalidateVisual();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var mbox = MessageBox.Show("Are you sure you want to quit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (mbox == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
            controller.KeyPressed(e.Key);
            logic.ActivateGravity();
            display.InvalidateVisual();
        }
    }
}
