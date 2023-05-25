using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static KPhotels.administration1;
using static System.Net.Mime.MediaTypeNames;

namespace KPhotels
{
    public partial class administration2 : Page
    {
        //строка подключения
        private string conString = @"Data Source = LAPTOP-TUEVAI0L\SQLEXPRESS; Initial Catalog = databasehotels; Integrated Security = true;";

        public administration2()
        {
            InitializeComponent();
            //Данные для ComboBox, в котором будут отображаться таблицы базы данных
            using (SqlConnection connection = new SqlConnection(conString))
            {
                string Name_of_the_procedure = "database_tables";
                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                List<string> list = new List<string>();
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader[0].ToString());
                };
                TableName.ItemsSource = list;
                reader.Close();
                connection.Close();
            }
        }
        //поле для проверки в каких случаях будет отображаться вся таблица, а в каких нет
        bool pr = false;

        //массив для хранения текстбоксов
        TextBox[] textboxs = new TextBox[8];

        //отображение textbox для добавленияя записи 
        private void _Visibility(int count, List<string> list, string TableName)
        {
            Label[] labels = new Label[8];
            int j = 0;
            int i1 = 1;
            GridTextBox.Children.Clear();
            GridTextBox.RowDefinitions.Clear();//очистка сетки
            GridTextBox.Height = 150;

            for (int i = 0; i < count; i++)
            {
                GridTextBox.Height += 35;
                textboxs[i] = new TextBox();
                labels[i] = new Label();
                labels[i].Content = list[i];
                labels[i].FontSize = 15;
                textboxs[i].FontSize = 15;
                RowDefinition rowDeflabel = new RowDefinition();
                GridTextBox.RowDefinitions.Add(rowDeflabel);
                RowDefinition rowDeftextbox = new RowDefinition();
                GridTextBox.RowDefinitions.Add(rowDeftextbox);
                GridTextBox.Children.Add(labels[i]);
                GridTextBox.Children.Add(textboxs[i]);
                Grid.SetRow(labels[i], j);
                Grid.SetRow(textboxs[i], i1);
                j = j + 2;
                i1 = i1 + 2;
            }
            if (TableName == "hotels")
            {
                Button buttons = new Button();
                GridTextBox.Height += 35;
                buttons.FontSize = 15;
                GridTextBox.Children.Add(buttons);
                Grid.SetRow(buttons, 18);
                buttons.Content = "Добавить фото";
                buttons.Click += ButtonOnClickImage;
            }
        }

        //кнопка для добавления картинки в таблицу hotels
        private void ButtonOnClickImage(object sender, EventArgs eventArgs)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Images files(*.JPG)|*.JPG";
            openFile.ShowDialog();
            string FileName = openFile.FileName;
            textboxs[6].Text = openFile.FileName;
        }


        //отображение таблицы выбранной в ComboBox
        public void AllTable()
        {
            SqlConnection connection = new SqlConnection(conString);
            string Name_of_the_procedure = "table_selection";
            SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@nametable", TableName.Text);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable(TableName.Text);
            sda.Fill(dt);
            DG.ItemsSource = dt.DefaultView;
        }

        //отображение таблицы в зависимости от запроса
        public void FillDataGrid(SqlCommand command)
        {
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(command);
                DataTable dt = new DataTable(TableName.Text);
                sda.Fill(dt);
                DG.ItemsSource = dt.DefaultView;
                if (pr != true)
                    AllTable();
                pr = false;
            }
            catch (Exception error)
            {
                MessageBox.Show("Ошибка! Проверьте данные!");
            }
        }

        public int count = 0;
        //кнопка "открыть таблицу"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TableName.Text != String.Empty)
            {
                AddDataButton.Visibility = Visibility.Visible;
                TableColumn.Visibility = Visibility.Visible;
                data.Visibility = Visibility.Visible;
                newdata.Visibility = Visibility.Visible;
                FullSearchButton.Visibility = Visibility.Visible;
                SearchbyPartButton.Visibility = Visibility.Visible;
                Update_Click.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
                ColumnLabel.Visibility = Visibility.Visible;
                labelinsertdata.Visibility = Visibility.Visible;
                labeldata.Visibility = Visibility.Visible;
                labelnewdata.Visibility= Visibility.Visible;
                count = 0;//переменная для подсчета количества столбцов
                AllTable();
                SqlConnection connection = new SqlConnection(conString);
                List<string> list = new List<string>();
                //добавление столбцов таблицы в ComboBox
                string Name_of_the_procedure = "column_search";
                SqlCommand command_ = new SqlCommand(Name_of_the_procedure, connection);
                command_.CommandType = CommandType.StoredProcedure;
                command_.Parameters.AddWithValue("@tablename", TableName.Text);
                connection.Open();
                SqlDataReader dr = command_.ExecuteReader();
                while (dr.Read())
                {
                    string fieldName = dr.GetString(0);//возвращает значение указанного столбца в виде строки
                    list.Add(fieldName);
                    count++;
                }
                TableColumn.ItemsSource = list;
                dr.Close();
                connection.Close();

                //отображение textbox
                _Visibility(count, list, TableName.Text);
            }
            else
            {
                MessageBox.Show("Выберите таблицу");
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
        //кнопка "Добавить данные"
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            for (int i=0; i < count; i++)
            {
                if (TableName.Text != "hotels" && i!=7)
                {
                    sqlinjection(textboxs[i]);
                }
            }
            if (result > 0)
            {
                MessageBox.Show("Ошибка! Вы ввели служебный символ");
                result = 0;
            }
            else
            {
                switch (TableName.Text)
                {
                    case "customers":
                        if (textboxs[0].Text != String.Empty && textboxs[1].Text != String.Empty && textboxs[2].Text != String.Empty && textboxs[3].Text != String.Empty && textboxs[4].Text != String.Empty && textboxs[5].Text != String.Empty && textboxs[6].Text != String.Empty && textboxs[7].Text != String.Empty)
                        {
                            string Name_of_the_procedure = "insert_customers";
                            SqlConnection connection = new SqlConnection(conString);
                            SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@customerIDText", textboxs[0].Text);
                            command.Parameters.AddWithValue("@nameText", textboxs[1].Text);
                            command.Parameters.AddWithValue("@surnameText", textboxs[2].Text);
                            command.Parameters.AddWithValue("@patronymicText", textboxs[3].Text);
                            command.Parameters.AddWithValue("@genderText", textboxs[4].Text);
                            command.Parameters.AddWithValue("@passport_numberText", textboxs[5].Text);
                            command.Parameters.AddWithValue("@loginCText", textboxs[6].Text.Replace(" ", ""));
                            command.Parameters.AddWithValue("@passwordCText", textboxs[7].Text.Replace(" ", ""));
                            FillDataGrid(command);
                        }
                        else MessageBox.Show("Пустое поле");
                        break;
                    case "hotels":
                        if (textboxs[0].Text != String.Empty && textboxs[1].Text != String.Empty && textboxs[2].Text != String.Empty && textboxs[3].Text != String.Empty && textboxs[4].Text != String.Empty && textboxs[5].Text != String.Empty && textboxs[6].Text != String.Empty)
                        {
                            if (Convert.ToInt32(textboxs[5].Text) < 26 && Convert.ToInt32(textboxs[5].Text) > 0)
                            {
                                string Name_of_the_procedure = "insert_hotels";
                                SqlConnection connection = new SqlConnection(conString);
                                SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@hotelID", textboxs[0].Text);
                                command.Parameters.AddWithValue("@Name", textboxs[1].Text);
                                command.Parameters.AddWithValue("@address", textboxs[2].Text);
                                command.Parameters.AddWithValue("@director", textboxs[3].Text);
                                command.Parameters.AddWithValue("@number_of_stars", textboxs[4].Text);
                                command.Parameters.AddWithValue("@number_of_rooms", textboxs[5].Text);
                                //command.Parameters.AddWithValue("@number_of_available_rooms", textboxs[6].Text);
                                command.Parameters.AddWithValue("@Image", textboxs[6].Text);
                                FillDataGrid(command);
                            }
                            else MessageBox.Show("Количество номеров должно быть не больше 25");
                        }
                        else MessageBox.Show("Пустое поле");
                        break;
                    case "passwords_for_administration":
                        if (textboxs[0].Text != String.Empty && textboxs[1].Text != String.Empty && textboxs[2].Text != String.Empty)
                        {
                            string Name_of_the_procedure = "insert_passwords_for_administration";
                            SqlConnection connection = new SqlConnection(conString);
                            SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Number", textboxs[0].Text);
                            command.Parameters.AddWithValue("@login", textboxs[1].Text.Replace(" ", ""));
                            command.Parameters.AddWithValue("@password", textboxs[2].Text.Replace(" ", ""));
                            FillDataGrid(command);
                        }
                        else MessageBox.Show("Пустое поле");
                        break;
                    case "reservation_log":
                        if (textboxs[0].Text != String.Empty && textboxs[1].Text != String.Empty && textboxs[2].Text != String.Empty && textboxs[3].Text != String.Empty && textboxs[4].Text != String.Empty && textboxs[5].Text != String.Empty)
                        {
                            string Name_of_the_procedure = "reservation";
                            SqlConnection connection = new SqlConnection(conString);
                            SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@idreservation", textboxs[0].Text);
                            command.Parameters.AddWithValue("@idhotel", textboxs[1].Text);
                            command.Parameters.AddWithValue("@roomID", textboxs[2].Text);
                            command.Parameters.AddWithValue("@customerID", textboxs[3].Text);
                            command.Parameters.AddWithValue("@start", textboxs[4].Text.Replace(" ", ""));
                            command.Parameters.AddWithValue("@finish", textboxs[5].Text.Replace(" ", ""));
                            FillDataGrid(command);
                        }
                        else MessageBox.Show("Пустое поле");
                        break;
                    case "rooms":
                        if (textboxs[0].Text != String.Empty && textboxs[1].Text != String.Empty && textboxs[2].Text != String.Empty && textboxs[3].Text != String.Empty)
                        {
                            string Name_of_the_procedure = "insert_rooms";
                            SqlConnection connection = new SqlConnection(conString);
                            SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@roomID", textboxs[0].Text);
                            command.Parameters.AddWithValue("@hotelID ", textboxs[1].Text);
                            command.Parameters.AddWithValue("@room_type", textboxs[2].Text);
                            command.Parameters.AddWithValue("@price", textboxs[3].Text);
                            FillDataGrid(command);
                        }
                        else MessageBox.Show("Пустое поле");
                        break;
                }
            }
        }


        //метод для вызова хранимой процедуры и записи параметров
        private void search(string Name_of_the_procedure)
        {
            SqlConnection connection = new SqlConnection(conString);
            SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@tablename", TableName.Text);
            command.Parameters.AddWithValue("@tableColumn", TableColumn.Text);
            command.Parameters.AddWithValue("@data", data.Text);
            pr = true;
            FillDataGrid(command);
        }



        //кнопка "Полный поиск"
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (TableColumn.Text != String.Empty)
            {
                sqlinjection(data);
                if (result > 0)
                {
                    MessageBox.Show("Ошибка! Вы ввели служебный символ");
                    result = 0;
                }
                else
                {
                    string Name_of_the_procedure = "full_search";
                    search(Name_of_the_procedure);
                }
            }
            else
            {
                MessageBox.Show("Выберите столбец для поиска");
            }
        }

        //кнопка "Поиск по части"
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (TableColumn.Text != String.Empty)
            {
                sqlinjection(data);
            if (result > 0)
            {
                MessageBox.Show("Ошибка! Вы столбец служебный символ");
                result = 0;
            }
            else
            {
                string Name_of_the_procedure = "search_by_part";
                search(Name_of_the_procedure);
            }
            }
            else
            {
                MessageBox.Show("Выберите столбец для поиска");
            }
        }

        //кнопка "Изменить"
        private void Update_Click_1(object sender, RoutedEventArgs e)
        {
            if (TableColumn.Text != String.Empty)
            {
            sqlinjection(data);
            sqlinjection(newdata);
            if (result > 0)
            {
                MessageBox.Show("Ошибка! Вы ввели служебный символ");
                result = 0;
            }
            else
            {
                if (data.Text != String.Empty && newdata.Text != String.Empty)
                {
                    SqlConnection connection = new SqlConnection(conString);
                    string Name_of_the_procedure = "update_data";
                    SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tablename", TableName.Text);
                    command.Parameters.AddWithValue("@tableColumn", TableColumn.Text);
                    command.Parameters.AddWithValue("@data", data.Text);
                    command.Parameters.AddWithValue("@newdata", newdata.Text);
                    FillDataGrid(command);
                }
                else
                {
                    MessageBox.Show("Пустое поле");
                }
            }
            }
            else
            {
                MessageBox.Show("Выберите столбец для изменения");
            }
        }

        //кнопка "Удалить"
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (TableColumn.Text != String.Empty)
            {
                sqlinjection(data);
            if (result > 0)
            {
                MessageBox.Show("Ошибка! Вы ввели служебный символ");
                result = 0;
            }
            else
            {
                if (data.Text != String.Empty)
                {
                    SqlConnection connection = new SqlConnection(conString);
                    string Name_of_the_procedure = "delete_data";
                    SqlCommand command = new SqlCommand(Name_of_the_procedure, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tablename", TableName.Text);
                    command.Parameters.AddWithValue("@tableColumn", TableColumn.Text);
                    command.Parameters.AddWithValue("@data", data.Text);
                    FillDataGrid(command);
                }
                else
                {
                    MessageBox.Show("Пустое поле");
                }
            }
            }
            else
            {
                MessageBox.Show("Выберите столбец для удаления");
            }
        }
    }
}
