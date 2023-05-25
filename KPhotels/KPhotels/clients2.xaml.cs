using System;
using System.Collections;
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
namespace KPhotels
{
    public partial class clients2 : Page
    {
        public clients2()
        {
            InitializeComponent();
            //скрываем кнопку
            buttonchooseanumber.Visibility = Visibility.Hidden;
            l.Content += " "+clients1.namehotel;
            //помечаем, что даты, которые уже прошли, выбрать нельзя
            date.BlackoutDates.AddDatesInPast();
        }


        public static string start { get; set; }
        public static string finish { get; set; }
        public static int duration { get; set; }
    //выбор даты/дат
    private void date_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
           //получаем первую выбранную дату
            start = Convert.ToString(date.SelectedDates.First());
            //обрезаем дату
            start = start.Remove(start.Length - 8);
            //получаем последнюю выбранную дату
            finish = Convert.ToString(date.SelectedDates.Last());
            finish = finish.Remove(finish.Length - 8);
            duration= date.SelectedDates.Count();
            //после выбора даты можно перейти к выбору номера, поэтому кнопка отображается
            buttonchooseanumber.Visibility = Visibility.Visible;
        }

        //переход на новую страницу
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Numbers1());
        }

        //переход на предыдущую страницу
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new clients1());
        }
    }
}
