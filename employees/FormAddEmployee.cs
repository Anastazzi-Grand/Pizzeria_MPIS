using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pizzeria
{
    public partial class FormAddEmployee : Form
    {
        public FormAddEmployee()
        {
            InitializeComponent();
        }

        public string TextBoxFullName
        {
            get { return textBoxFullName.Text; }
            set { textBoxFullName.Text = value; }
        }

        public string TextBoxPhoneNumber
        {
            get { return textBoxPhoneNumber.Text; }
            set { textBoxPhoneNumber.Text = value; }
        }

        public string TextBoxPosition
        {
            get { return textBoxPosition.Text; }
            set { textBoxPosition.Text = value; }
        }

        private int? _employeeId;

        public int? EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                // Получаем данные из TextBox'ов
                string fullname = textBoxFullName.Text;
                string phonenumber = textBoxPhoneNumber.Text;
                string position = textBoxPosition.Text;

                try
                {
                    // Создаем соединение с базой данных
                    using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            // Проверяем, является ли это обновлением существующего продукта
                            if (_employeeId.HasValue)
                            {
                                // Обновляем существующий продукт
                                string query = "UPDATE employee SET \"fullname\" = @fullname, \"phonenumber\" = @phonenumber, \"position\" = @position WHERE \"id\" = @id";
                                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@fullname", fullname);
                                    command.Parameters.AddWithValue("@phonenumber", phonenumber);
                                    command.Parameters.AddWithValue("@position", position);
                                    command.Parameters.AddWithValue("@id", _employeeId.Value);

                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Добавляем новый продукт
                                string query = "INSERT INTO employee (\"fullname\", \"phonenumber\", \"position\") VALUES (@fullname, @phonenumber, @position)";
                                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@fullname", fullname);
                                    command.Parameters.AddWithValue("@phonenumber", phonenumber);
                                    command.Parameters.AddWithValue("@position", position);

                                    command.ExecuteNonQuery();
                                }
                            }

                            // После сохранения данных, закрываем форму
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                }
                catch (NpgsqlException ex)
                {
                    // Проверяем, является ли исключение вызванным триггером
                    if (ex.Message.Contains("Неверный формат номера телефона"))
                    {
                        // Выводим сообщение об ошибке
                        MessageBox.Show($"Ошибка базы данных: {ex.Message}\nТранзакция завершилась в триггере. Выполнение пакета было прервано.", "Ошибка");
                    }
                    else
                    {
                        // Обрабатываем другие ошибки
                        Console.WriteLine($"Ошибка при сохранении данных в базу данных: {ex.Message}");
                    }
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
