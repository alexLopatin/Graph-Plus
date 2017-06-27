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

namespace GraphPlus
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NativeWindow RenderWindow;
        public MainWindow()
        {
            InitializeComponent();
            
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RenderWindow = (NativeWindow)FindName("render");
            //RenderWindow = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();

            float r = rand.Next(101)/100f;
            float g = rand.Next(101)/100f;
            float b = rand.Next(101) / 100f;
            RenderWindow.ChangeColor(r, g, b);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RenderWindow.MoveCamera(0, 5);
        }

        private void render_MouseEnter(object sender, MouseEventArgs e)
        {
            if(e.LeftButton==MouseButtonState.Pressed)
            {
                RenderWindow.MoveCamera(5, 0);
            }
        }
    }
}
