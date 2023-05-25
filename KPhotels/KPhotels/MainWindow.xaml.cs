using System;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Navigation;

 namespace KPhotels
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new administration1();
        }

        //по нажатию на кнопку возврат на главное меню
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            MainFrame.Navigate(new administration1());
            
        }
        //скрытие кнопки в зависимости от страницы
        private void Button_Exit(object sender, EventArgs e)
        {

            if (MainFrame.CanGoBack)
            {
                ButtonExit.Visibility = Visibility.Visible;
            }
            //проверка, открыта ли страница входа, если да, то кнопка Выход скрыта
            if (MainFrame.Content.ToString() == "KPhotels.administration1")
            {
            ButtonExit.Visibility = Visibility.Hidden;
            }
        }
      
    }
}
