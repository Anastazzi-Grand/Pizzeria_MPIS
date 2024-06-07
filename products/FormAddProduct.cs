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
    public partial class FormAddProduct : Form
    {
        public FormAddProduct()
        {
            InitializeComponent();
        }

        public string TextBoxDescription
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string TextBoxPrice
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        public string TextBoxCookingTime
        {
            get { return textBox3.Text; }
            set { textBox3.Text = value; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormAddProduct_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private int? _productId;

        public int? ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        private void buttonAddProduct_Click(object sender, EventArgs e)
        {
            // Получаем данные из TextBox'ов
            string productName = textBox1.Text;
            decimal productPrice = Convert.ToDecimal(textBox2.Text);
            string cookingTime = textBox3.Text;

            try
            {
                // Создаем соединение с базой данных
                using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        // Проверяем, является ли это обновлением существующего продукта
                        if (_productId.HasValue)
                        {
                            // Изменяем заголовок формы
                            this.Text = "Изменение продукта";
                            // Обновляем существующий продукт
                            string query = "UPDATE product SET \"description\" = @description, \"price\" = @price, \"cookingTime\" = @cookingTime WHERE \"id\" = @id";
                            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@description", productName);
                                command.Parameters.AddWithValue("@price", productPrice);
                                command.Parameters.AddWithValue("@cookingTime", cookingTime);
                                command.Parameters.AddWithValue("@id", _productId.Value);
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Изменяем заголовок формы
                            this.Text = "Добавление продукта";
                            // Добавляем новый продукт
                            string query = "INSERT INTO product (\"description\", \"price\", \"cookingTime\") VALUES (@description, @price, @cookingTime)";
                            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@description", productName);
                                command.Parameters.AddWithValue("@price", productPrice);
                                command.Parameters.AddWithValue("@cookingTime", cookingTime);
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
                if (ex.Message.Contains("Цена не может быть отрицательной"))
                {
                    // Обработка ошибки, если цена отрицательная
                    MessageBox.Show($"Ошибка базы данных: {ex.Message}\nТранзакция завершилась в триггере. Выполнение пакета было прервано.", "Ошибка");
                }
                else
                {
                    // Обработка других ошибок
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
