using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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
using System.Xml.Linq;
using static KPhotels.administration1;
using static KPhotels.clients1;
using static System.Net.Mime.MediaTypeNames;

namespace KPhotels
{
    public partial class clients1 : Page
    {

        public class Hotel
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Director { get; set; }
            public int Stars { get; set; }
            public int Numberz{ get; set; }
            public string Image{ get; set; }
        }
        //поля для записи количества гостиниц и номеров
        static int counthotels;
        static int countrooms;
        public List<Hotel> Hotels { get; set; }

        private string conString = @"Data Source = LAPTOP-TUEVAI0L\SQLEXPRESS; Initial Catalog = databasehotels; Integrated Security = true;";
        
        //получаем количество гостиниц в базе
        private void Number_of_hotels()
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string Name_of_the_procedure = "Number_of_hotels";
                SqlCommand cmd = new SqlCommand(Name_of_the_procedure, con);
                cmd.CommandType = CommandType.StoredProcedure;//команда представляет хранимую процедуру
                var reader = cmd.ExecuteScalar();
                counthotels= Convert.ToInt32(reader);
                con.Close();

            }

        }

        //получаем количество номеров в базе
        private void Number_of_rooms()
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string Name_of_the_procedure = "Number_of_rooms";
                SqlCommand cmd = new SqlCommand(Name_of_the_procedure, con);
                cmd.CommandType = CommandType.StoredProcedure;//команда представляет хранимую процедуру
                var reader = cmd.ExecuteScalar();
                countrooms = Convert.ToInt32(reader);
                con.Close();

            }

        }


        class hotels_and_rooms
        {
            public static string[] h = new string[counthotels];
            public static  int[] idh = new int[counthotels];
            public static int[] n = new int[countrooms];
        }

        public clients1()
        {
            InitializeComponent();
            Number_of_hotels();
            Number_of_rooms();
            Hotels = new List<Hotel>();
            FillListBox(Hotels);
           
        }

        //поле для результата проверки на sql-инъекцию
        public int result;
        //проверка на sql-инъекцию
        public void sqlinjection(TextBox text)
        {
            char[] symbols = { '"', ',', ':', ';', '/', '\\', '\'', '*', '+','.' };
            bool flag = true;
            foreach (char c in text.Text)
            {
                if (!flag) break;
                flag = false;
                for (int i = 0; i < symbols.Length; i++)
                {
                    if (c != symbols[i]) { flag = true; }
                    else { flag = false; break; }
                }
            }
            if (!flag)
            {
                text.Clear();
                result++;
            }

        }
        public List<Hotel> HotelsP { get; set; }
        //кнопка поиска гостиницы
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sqlinjection(TextBoxSearch);
            if (result > 0)
            {
                MessageBox.Show("Ошибка! Вы ввели служебный символ");
                result = 0;
            }
            else
            {
                HotelsP = new List<Hotel>();
                FillListBox(HotelsP);
            }
        }

        //вывод гостиниц в listbox
        public void FillListBox(List<Hotel> name)
        {
            string Name_of_the_procedure = "Fill_hotels";
            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = command.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    Hotel Hotels1 = new Hotel();
                    Hotels1.id = Convert.ToInt32(reader[0]);
                    Hotels1.Name = reader[1].ToString();
                        Hotels1.Address = reader[2].ToString();
                    //проверка поиска
                    if (Hotels1.Name.ToLower().Contains(TextBoxSearch.Text.ToLower()) || Hotels1.Address.ToLower().Contains(TextBoxSearch.Text.ToLower()) || TextBoxSearch.Text == "")
                    {
                        Hotels1.Director = reader[3].ToString();
                        Hotels1.Stars = Convert.ToInt32(reader[4]);
                        //проверка фильтрации
                        if (Hotels1.Stars == countstars || countstars==0)
                        {
                            Hotels1.Numberz = Convert.ToInt32(reader[5]);
                            //Hotels1.Numbers = Convert.ToInt32(reader[6]);
                            Hotels1.Image = reader[6].ToString();
                            name.Add(Hotels1);
                            hotels_and_rooms.h[i] = Hotels1.Name;
                            hotels_and_rooms.n[i] = Hotels1.Numberz;
                            hotels_and_rooms.idh[i] = Hotels1.id;
                            i++;
                        }
                    }
                };
                listbox.ItemsSource = name;
                reader.Close();
                connection.Close();
            }
        }

        //свойства для передачи данных на следующие страницы
        public static string namehotel { get; set; }
        public static int numberhotel { get; set; }
        public static int idhotel { get; set; }

        //при двойном нажатии на listbox выполняется "выбор" гостиницы и переход на следующую страницу
        private void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = listbox.SelectedIndex;//считывает индекс гостиницы 
            namehotel = hotels_and_rooms.h[index];
            numberhotel= hotels_and_rooms.n[index];
            idhotel = hotels_and_rooms.idh[index];
            NavigationService.Navigate(new clients2());
        }



        public List<Hotel> HotelsS { get; set; }
        //поле для результата фильтрации 
        public int countstars = 0;
        //Фильтрация по звездам
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            switch (filter.Text)
            {
                case "5 звезд":
                    countstars = 5;
                    break;
                case "4 звезды":
                    countstars = 4;
                    break;
                case "3 звезды":
                    countstars = 3;
                    break;
                case "2 звезды":
                    countstars = 2;
                    break;
                case "1 звезда":
                    countstars = 1;
                    break;
                case "Показать все":
                    countstars = 0;
                    break;
            }
            HotelsS = new List<Hotel>();
            FillListBox(HotelsS);
        }
    }
}
