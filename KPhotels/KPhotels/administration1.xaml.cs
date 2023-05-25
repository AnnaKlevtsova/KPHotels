using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls.Primitives;

namespace KPhotels
{
    public partial class administration1 : Page
    {
        public static int clientID { get; set; }
        public administration1()
        {
            InitializeComponent();

        }
        string conString = @"Data Source=LAPTOP-TUEVAI0L\SQLEXPRESS; Initial Catalog=databasehotels; Integrated Security=true;";
        
        bool f = false;
        int input = 0;
        public class Student
        {
            public string log { get; set; }
            public string par { get; set; }
            public int id { get; set; }
        }

        //чтение данных и проверка логина и пароля с данными из базы данных
        public void FillDataGrid(String Name_of_the_procedure)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@login ", login.Text);
                command.Parameters.AddWithValue("@password ", passwordbox.Password);
                try
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Student st = new Student();
                        st.log = reader[0].ToString();
                        st.par = reader[1].ToString();
                        st.id = Convert.ToInt32(reader[2]);
                        clientID = st.id;
                        if (st.log == login.Text && st.par == passwordbox.Password)
                        {
                            MessageBox.Show("Успешно");
                            f = true;
                            input = 1;
                        }
                    }

                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //поле для результата проверки на sql-инъекцию
        public int result;
        //проверка на sql-инъекцию
        public void sqlinjection(TextBox text)
        {
            char[] symbols = { '"', ',', ':', ';', '/', '\\', '\'', '*', '+', '-' };
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

        //вход
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //проверка для того, чтобы все изменения в passwordbox и textbox отражались
            if (checkBox.IsChecked == true)
            {
                passwordbox.Password = parol.Text;
            }
            if (checkBox.IsChecked == false)
            {
                parol.Text = passwordbox.Password;
            }

            sqlinjection(login);
            sqlinjection(parol);
            if (result > 0)
            {
                MessageBox.Show("Ошибка! Вы ввели служебный символ");
                result = 0;
            }
            else
            {
                if (parol.Text != String.Empty && login.Text != String.Empty)
                {
                    string Name_of_the_procedure = "administrator_login ";

                    FillDataGrid(Name_of_the_procedure);

                    if (input == 1)
                    {
                        MessageBox.Show("Вы вошли как администратор");
                        NavigationService.Navigate(new administration2());
                    }
                    else
                    {
                        Name_of_the_procedure = "clients_login";
                        FillDataGrid(Name_of_the_procedure);

                        if (input == 1)
                        {
                            MessageBox.Show("Вы вошли как клиент");
                            NavigationService.Navigate(new clients1());
                        }
                    }
                    if (f == false) MessageBox.Show("Неверные данные");
                    input = 0;
                    f = false;
                } else MessageBox.Show("Пустое поле!");
            }
        }

        //Показать пароль, скрытие элементов
        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox.IsChecked == true)
            {
                passwordbox.Visibility = Visibility.Hidden;
                parol.Text = passwordbox.Password;
                parol.Visibility = Visibility.Visible;
            }
            if (checkBox.IsChecked == false)
            {
                parol.Visibility = Visibility.Hidden;
                passwordbox.Password = parol.Text;
                passwordbox.Visibility = Visibility.Visible;
            }
        }


        //нажатие на кнопку зарегистрироваться, переход на страницу регистрации
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new registration());
        }
    }
}
