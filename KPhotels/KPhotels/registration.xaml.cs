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
using System.Security.Cryptography.X509Certificates;
using System.IO.Packaging;

namespace KPhotels
{
    public partial class registration : Page
    {
        string conString = @"Data Source=LAPTOP-TUEVAI0L\SQLEXPRESS; Initial Catalog=databasehotels; Integrated Security=true;";
        public int countcustomers;
        public string first_letter=String.Empty;
        public string numberpassport;
        public registration()
        {
            InitializeComponent();
            
        }

        //получение количества клиентов
        public void Countcustomer()
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                string Name_of_the_procedure = "count_customer";
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader = command.ExecuteScalar();
                countcustomers = Convert.ToInt32(reader);
            }
        }

        //добавление клиента в базу данных
        public void Registration()
        {
            if (TextBoxPassword.Text != TextBoxPasswordCheck.Text)
            {
                MessageBox.Show("Проверьте пароль");
            }
            else
            {
                try
                {
                    Countcustomer();
                    countcustomers++;
                    administration1.clientID = countcustomers;
                    string Name_of_the_procedure = "insert_customers";
                    SqlConnection connection = new SqlConnection(conString);
                    SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@customerIDText", countcustomers);
                    command.Parameters.AddWithValue("@nameText", TextBoxName.Text);
                    command.Parameters.AddWithValue("@surnameText", TextBoxSurname.Text);
                    command.Parameters.AddWithValue("@patronymicText", TextBoxPatronymic.Text);
                    command.Parameters.AddWithValue("@genderText", first_letter[0]);
                    command.Parameters.AddWithValue("@passport_numberText", numberpassport);
                    command.Parameters.AddWithValue("@loginCText", TextBoxLogin.Text);
                    command.Parameters.AddWithValue("@passwordCText", PasswordBoxPassword.Password);
                    connection.Open();
                    var writer = command.ExecuteNonQuery();//выполнение добавления записи 
                    MessageBox.Show("Вы зарегистрировались!");
                    connection.Close();
                    NavigationService.Navigate(new clients1());
                }
                catch (Exception error)
                {
                    MessageBox.Show("Ошибка! Проверьте данные!");
                }
            }
        }

        //нажатие на radiobutton (выбор мужского пола)
        private void radiobuttonman_Checked(object sender, RoutedEventArgs e)
        {
            first_letter = (string)radiobuttonman.Content;
        }

        //нажатие на radiobutton (выбор женского пола)
        private void radiobuttonwoman_Checked(object sender, RoutedEventArgs e)
        {
            first_letter =(string)radiobuttonwoman.Content;
        }

        //проверка на наличие цифр-если цифры есть, то ошибка
        public void checking_for_numbers(TextBox textbox)
        {
            bool flag = true;
            foreach (char c in textbox.Text)
            {
                flag = false;
                if (c >= '0' && c <= '9') {  flag = false; break; }
                else
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                MessageBox.Show("Ввод цифр в данное поле запрещен");
                textbox.Clear();
            }
        }

        //поле для результата проверки на sql-инъекцию
        public int result;
        //проверка на sql-инъекцию
        public void sqlinjection(TextBox text)
        {
            char[] symbols = { '"', '.', ',', ':', ';', '/', '\\', '\'', '*', '+' };
            if (text == TextBoxName || text == TextBoxSurname || text == TextBoxPatronymic)
            {
                checking_for_numbers(text);
            }
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

        //поле для результата проверки на пробел
        public int spaces;
        //проверка на пробелы
        public void Checking_for_spaces(TextBox textbox)
        {
            foreach (char c in textbox.Text)
            {
                if (c==' ')
                {
                    spaces++;
                }
            }
        }
        //кнопка Зарегистрироваться
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //присваивание значений PasswordBoxов TextBox
            TextBoxPassword.Text = PasswordBoxPassword.Password;
            TextBoxPasswordCheck.Text = PasswordBoxPasswordCheck.Password;
            //проверки на sql-инъекции и на пробелы
            sqlinjection(TextBoxName);
            sqlinjection(TextBoxSurname);
            sqlinjection(TextBoxPatronymic);
            sqlinjection(TextBoxNumberPassport);
            sqlinjection(TextBoxLogin);
            sqlinjection(TextBoxPassword);
            sqlinjection(TextBoxPasswordCheck);
            Checking_for_spaces(TextBoxLogin);
            Checking_for_spaces(TextBoxPassword);
            Checking_for_spaces(TextBoxPasswordCheck);
            if (result >0 || spaces > 0)
            {
                if (result > 0)
                {
                    MessageBox.Show("Ошибка! Вы ввели служебный символ");
                    result = 0;
                }
                if (spaces > 0)
                {
                    MessageBox.Show("В данное поле запрещено вводить пробел");
                    spaces = 0;
                }
            }
            else
            {
                bool flag = true;
                numberpassport = TextBoxNumberPassport.Text.Replace(" ", "");
                foreach (char c in numberpassport)
                {
                    flag = false;
                    if (c >= '0' && c <= '9') { flag = true; }
                    else 
                    {
                        flag = false; 
                        break;
                    }
                }
                if (!flag)
                {
                    MessageBox.Show("Вы ввели символ! Пожалуйста,введите цифрy");
                    TextBoxNumberPassport.Clear();
                }
                else
                {
                    if (first_letter!= String.Empty && TextBoxName.Text!=String.Empty && TextBoxSurname.Text != String.Empty && TextBoxPatronymic.Text != String.Empty && TextBoxNumberPassport.Text != String.Empty && PasswordBoxPassword.Password != String.Empty && TextBoxLogin.Text != String.Empty && PasswordBoxPasswordCheck.Password != String.Empty)
                    Registration();
                    else MessageBox.Show("Пустое поле!");
                }
            }
        }

    }
}
