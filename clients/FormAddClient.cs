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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Pizzeria
{
    public partial class FormAddClient : Form
    {
        public FormAddClient()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

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

        private int? _clientId;

        public int? ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Получаем данные из TextBox'ов
            string fullname = textBoxFullName.Text;
            string phonenumber = textBoxPhoneNumber.Text;

            try
            {
                // Создаем соединение с базой данных
                using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        // Проверяем, является ли это обновлением существующего продукта
                        if (_clientId.HasValue)
                        {
                            // Обновляем существующий продукт
                            string query = "UPDATE client SET \"fullname\" = @fullname, \"phonenumber\" = @phonenumber WHERE \"id\" = @id";
                            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@fullname", fullname);
                                command.Parameters.AddWithValue("@phonenumber", phonenumber);
                                command.Parameters.AddWithValue("@id", _clientId.Value);

                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Добавляем новый продукт
                            string query = "INSERT INTO client (\"fullname\", \"phonenumber\") VALUES (@fullname, @phonenumber)";
                            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@fullname", fullname);
                                command.Parameters.AddWithValue("@phonenumber", phonenumber);

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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
