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
using lib;
using System.Diagnostics;

namespace GraphPlus
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Grid grid;

        public NativeWindow NW;
        public MainWindow()
        {
            InitializeComponent();
            
            //t = t;
            Brush brush = new SolidColorBrush((Color)FindResource("DefaultColor"));

            
            Background = brush;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            grid = (Grid)FindName("MainGrid");

            
            NW = (NativeWindow)FindName("renderWindow");
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
        private bool isMaximized = false;
        private void Maximize(object sender, RoutedEventArgs e)
        {
            renderWindow.inputController.StopMouseControl();
            if(WindowState == WindowState.Maximized)
            {
                SystemCommands.RestoreWindow(this);
                grid.Margin = new Thickness(0);
                //Margin = new Thickness(0);
            }
            else
            {
               
                SystemCommands.MaximizeWindow(this);
                grid.Margin = new Thickness(7);
                // Margin = new Thickness(50);
            }
                
            isMaximized =!isMaximized;
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(WindowState ==WindowState.Normal& isMaximized)
            {
                isMaximized = false;
                grid.Margin = new Thickness(0);
            } else if(WindowState == WindowState.Maximized & !isMaximized)
            {
                isMaximized = true;
                grid.Margin = new Thickness(7);
            }
        }



        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)FindName("funcBox");
                
                Function F = new Function(textBox.Text);
                F.StringFunc = "y=" + F.StringFunc;
                NW.inputController.AddFunction(F);
                var template = (ControlTemplate)FindResource("Function");
                
                ContentControl c = new ContentControl { Content = template.LoadContent() };
                
                c.DataContext = F;
               
                FunctionList.Children.Add(c);

                textBox.Text = "";


            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Function F = (sender as MenuItem).DataContext as Function;
            renderWindow.inputController.RemoveFunction(F);
            var b =(((sender as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Parent;
            FunctionList.Children.Remove(b as UIElement);

        }

        private void FunctionString_MouseEnter(object sender, MouseEventArgs e)
        {

            Function F = (sender as Button).DataContext as Function;
            F.SetThickness(2);
            renderWindow.inputController.Draw();
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Function F = (sender as Button).DataContext as Function;
            F.SetThickness(1);
            renderWindow.inputController.Draw();
        }
    }

}
