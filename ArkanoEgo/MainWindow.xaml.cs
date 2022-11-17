using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArkanoEgo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            myCanvas.Focus();
        }

        private void myCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.A)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
            if (e.Key == Key.D)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }
        }
    }
}
