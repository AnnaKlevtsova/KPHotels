using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Xml.Linq;
using static KPhotels.administration1;
using static KPhotels.clients1;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics.Tracing;
using System.Threading;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using static DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Office2010.Word;

namespace KPhotels
{
    public partial class Numbers1 : Page
    {
        public static int registrationid { get; set; }
        public static int customerid { get; set; }
        public static string customername { get; set; }
        public static string customersurname { get; set; }
        public static string customerpatronymic { get; set; }
        public static int roomid { get; set; }
        public static string hotelsname { get; set; }
        public static int price { get; set; }
        public string room_type { get; set; }

        public static string start { get; set; }

        public static string finish { get; set; }
        public static int duration { get; set; }

        private string conString = @"Data Source = LAPTOP-TUEVAI0L\SQLEXPRESS; Initial Catalog = databasehotels; Integrated Security = true;";


        //поиск количества номеров в регистрациях
        public void gettingregistrationid()
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                string Name_of_the_procedure = "last_reservation_entry";
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteScalar();
                registrationid = Convert.ToInt32(reader);
            }
        }

        //резервирование номера в гостинице 
        public void reservation(Button button)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                string Name_of_the_procedure = "reservation";
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@idreservation", registrationid);
                command.Parameters.AddWithValue("@idhotel", clients1.idhotel);
                command.Parameters.AddWithValue("@roomID", button.Content);
                command.Parameters.AddWithValue("@customerID", administration1.clientID);
                command.Parameters.AddWithValue("@start", clients2.start);
                command.Parameters.AddWithValue("@finish", clients2.finish);
                connection.Open();
                var writer = command.ExecuteNonQuery();//выполнение добавления записи 
                connection.Close();

            }
        }

        //данные о номере
        public void Data_Room(Button button)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                string Name_of_the_procedure = "data_room";
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@roomid", button.Content);
                command.Parameters.AddWithValue("@hotelid", clients1.idhotel);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    roomid = Convert.ToInt32(reader[0]);
                    room_type = Convert.ToString(reader[1]);
                    price = Convert.ToInt32(reader[2]);
                };
                reader.Close();
                connection.Close();

            }
        }

        public bool reserve=false;
        public Button buttonsender;

        //если нажали на кнопку номера
        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            
            //получаем кнопку, на которую нажали
            var button = (Button)sender;
            if (!reserve)
            {
                buttonsender = button;
                Data_Room(button);
                datanumber.Content = ($"Номер: {roomid}\nТип комнаты: {room_type}\nЦена за сутки: {price}\nИтоговая цена: {clients2.duration*price}");
                reservebutton.Visibility = Visibility.Visible;
            }
           if (reserve)
                {
                button = buttonsender;
                    //получаем сколько уже было записей резервирования
                    gettingregistrationid();
                    //увеличиваем на одну
                    registrationid++;
                    //убирем точки из дат, чтобы проверить какая из них больше
                    string starts = clients2.start.Replace(".", "");
                    string finish = clients2.finish.Replace(".", "");
                    string newstart = "";
                    //делаем даты от меньшего к большему
                    if (Convert.ToInt32(starts) > Convert.ToInt32(finish))
                    {
                        newstart = clients2.start;
                        clients2.start = clients2.finish;
                        clients2.finish = newstart;
                    }
                    MessageBox.Show("Вы зарезервировали номер");
                    //резервирование номера
                    reservation(button);
                    //кнопка на которую нажали становится зеленой
                    button.Background = Brushes.Green;

                    file.Visibility = Visibility.Visible;
                    reserve = false;
                    reservebutton.Visibility = Visibility.Hidden;
               }

        }

        //получаем данные для записи для дальнейшего отчета
        public void report()
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                string Name_of_the_procedure = "report";
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@idreservation", registrationid);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    customerid = Convert.ToInt32(reader[0]);
                    customername = Convert.ToString(reader[1]);
                    customersurname = Convert.ToString(reader[2]);
                    customerpatronymic = Convert.ToString(reader[3]);
                    roomid = Convert.ToInt32(reader[4]);
                    hotelsname = Convert.ToString(reader[5]);
                    price = Convert.ToInt32(reader[6]);
                };
                reader.Close();
                connection.Close();

            }
        }

        //отображение номеров, создание кнопок
        public void Room_buttons(int[] mas2, int col, int n2, int[] mas1)
        {
            Button[,] buttons = new Button[col, col];
            for (var i = 0; i < col; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    if (n2 < col)
                    {
                        buttons[i, j] = new Button();
                        //если номер занят
                        if (mas1[n2] != mas2[n2] || mas2[n2] == 0)
                            buttons[i, j].Background = Brushes.Red;
                        else
                        {
                            //если номер свободен
                            buttons[i, j].Background = Brushes.LightCyan;
                            buttons[i, j].Click += ButtonOnClick;
                        }
                        buttons[i, j].FontSize = 20;
                        buttons[i, j].Height = 60;
                        buttons[i, j].Width = 60;
                        buttons[i, j].Content = mas1[n2];

                        n2++;
                        buttons[i, j].BorderThickness = new Thickness(2);
                        Grid.SetColumn(buttons[i, j], j);
                        Grid.SetRow(buttons[i, j], i);
                        numbers.Children.Add(buttons[i, j]);
                    }
                }
            }
        }


        public Numbers1()
        {
            InitializeComponent();
            file.Visibility = Visibility.Hidden;
            reservebutton.Visibility = Visibility.Hidden;
            //создание строк и столбцов для grid в котором потом будут отображаться номера 
            for (var i = 0; i < clients1.numberhotel; i++)
            {
                numbers.Width = 400;
                RowDefinition newRow = new RowDefinition();
                newRow.Height= new GridLength(80);
                numbers.RowDefinitions.Add(newRow);
                ColumnDefinition newColumn = new ColumnDefinition();
                newColumn.Width = new GridLength(80);
                numbers.ColumnDefinitions.Add(newColumn);
            }

            //два массива, которые нужны для проверки свободных и занятых номеров
            int[] mas1 = new int[clients1.numberhotel];
            int[] mas2 = new int[clients1.numberhotel];
            using (SqlConnection connection = new SqlConnection(conString))
            {
                //процедура, которая выводит номер по дате резервирования
                string Name_of_the_procedure = "output_of_numbers_by_date";
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@finish", clients2.finish);
                command.Parameters.AddWithValue("@start", clients2.start);
                command.Parameters.AddWithValue("@idhotel", clients1.idhotel);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int n = 0;
                while (reader.Read())
                {
                    //запись всех номеров
                    mas1[n] = Convert.ToInt32(reader[0]);
                    //запись свободных номеров
                    mas2[n] = Convert.ToInt32(reader[1]);
                    int col;
                    n++;
                    col = clients1.numberhotel;
                    int n2 = 0;
                    Room_buttons(mas2, col, n2, mas1);
                };
                //если нет номеров
                if (n == 0) {int col = clients1.numberhotel; int n2 = 0; Room_buttons(mas2, col, n2, mas1); }
                reader.Close();
                connection.Close();
            }

            }

         
        //создание файла отчета
        private void file_Click(object sender, RoutedEventArgs e)
        {
            //получаем данные для отчета
            report();
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Сохранить файл";
            // saveFileDialog1.Filter = "text files(*.txt)|*.txt";
            saveFileDialog1.Filter = "text files(*.docx)|*.docx";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                start=clients2.start;
                finish = clients2.finish;
                duration = clients2.duration * price;

                //в случае сохранения файла txt
                /*  FileStream file = (FileStream)saveFileDialog1.OpenFile();
                  using (StreamWriter sw = new StreamWriter(file))
                  {
                      if (clients2.start!= clients2.finish)
                      sw.WriteLine($"Номер клиента: {customerid}\nФИО клиента: {customername} { customersurname} {customerpatronymic}\nЗарезервирован номер {roomid} в гостинице {hotelsname}\nЦена за сутки: { price}\nЦена к оплате: {clients2.duration * price}\nДаты с {clients2.start} по {clients2.finish}");
                    else
                          sw.WriteLine($"Номер клиента: {customerid}\nФИО клиента: {customername} {customersurname} {customerpatronymic}\nЗарезервирован номер {roomid} в гостинице {hotelsname}\nЦена за сутки: {price}\nЦена к оплате: {clients2.duration * price}\nДата: {clients2.start}");

                      //sw.WriteLine($"Клиент с номером {customerid}, {customername} {customersurname} {customerpatronymic} Зарезервировал/а номер {roomid} в гостинице {hotelsname} ценой {price} на {clients2.start}");

                      sw.Close();
                  }

                  file.Close();*/


                if (start != finish)
                    start = $"Даты с {start} по";
                else
                {
                    start = $"Дата: {start}";
                    finish = "";
                }

                //создание файла ворд
                File file = new File(saveFileDialog1.FileName);
                file.CreateFile();
            }

        }


        //кнопка назад
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new clients2());
        }

        //кнопка Зарезервировать
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            reserve = true;
            ButtonOnClick(sender, e);
        }
    }
}
