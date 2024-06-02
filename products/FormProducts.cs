using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Pizzeria
{
    public partial class FormProducts : Form
    {
        public FormProducts()
        {
            InitializeComponent();
        }

        private void FormProducts_Load(object sender, EventArgs e)
        {
            LoadProductsToGridView();
        }

        public void LoadProductsToGridView()
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                string query = "SELECT * FROM product ORDER BY \"id\" ASC";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Связываем столбцы DataGridView с данными из DataTable
                        dataGridView1.DataSource = dataTable;

                        dataGridView1.Columns["id"].HeaderText = "ID";
                        dataGridView1.Columns["description"].HeaderText = "Название";
                        dataGridView1.Columns["price"].HeaderText = "Цена";
                        dataGridView1.Columns["cookingTime"].HeaderText = "Время приготовления";
                    }
                }
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormMainMenu f = new FormMainMenu();
            this.Hide();
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormAddProduct formAddProduct = new FormAddProduct();
            formAddProduct.Show();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            // Получаем выбранную строку в DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем значения из выбранной строки
                int productId = (int)selectedRow.Cells["id"].Value;
                string description = selectedRow.Cells["description"].Value.ToString();
                decimal price = decimal.Parse(selectedRow.Cells["price"].Value.ToString());
                string cookingTime = selectedRow.Cells["cookingTime"].Value.ToString();

                // Открываем форму FormAddProduct и заполняем ее данными
                FormAddProduct formAddProduct = new FormAddProduct();
                formAddProduct.ProductId = productId;
                formAddProduct.TextBoxDescription = description;
                formAddProduct.TextBoxPrice = price.ToString();
                formAddProduct.TextBoxCookingTime = cookingTime;

                // Показываем форму
                if (formAddProduct.ShowDialog() == DialogResult.OK)
                {
                    // Обновляем данные в DataGridView
                    LoadProductsToGridView();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для изменения.");
            }
        }



        private void button4_Click(object sender, EventArgs e)
        {
            // Получаем выбранную строку в DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем ID выбранного продукта
                int productId = (int)selectedRow.Cells["id"].Value;

                // Спрашиваем пользователя, действительно ли он хочет удалить продукт
                if (MessageBox.Show("Вы действительно хотите удалить этот продукт?", "Подтверждение удаления", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        // Создаем соединение с базой данных
                        using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                        {
                            if (connection.State == ConnectionState.Open)
                            {
                                // Создаем SQL-запрос для удаления продукта из таблицы Product
                                string query = "DELETE FROM product WHERE \"id\" = @id";
                                // Создаем команду для выполнения SQL-запроса
                                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                                {
                                    // Передаем параметры в SQL-запрос
                                    command.Parameters.AddWithValue("@id", productId);

                                    // Выполняем SQL-запрос
                                    command.ExecuteNonQuery();
                                }

                                // Обновляем данные в DataGridView
                                LoadProductsToGridView();
                            }
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine($"Ошибка при удалении данных из базы данных: {ex.Message}");
                        // Обработка ошибки при удалении данных
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для удаления.");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = "";
            LoadProductsToGridView();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void buttonNameSearch_Click(object sender, EventArgs e)
        {
            FilterProductsByName();
        }

        private void FilterProductsByName()
        {
            try
            {
                // Создаем соединение с базой данных
                using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        // Получаем текст из TextBox
                        string searchText = textBoxSearch.Text.Trim();

                        // Формируем запрос на выборку продуктов, отфильтрованных по названию
                        string query = "SELECT * FROM product WHERE LOWER(\"description\") LIKE @searchText ORDER BY \"id\" ASC";
                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@searchText", $"%{searchText.ToLower()}%");
                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                // Создаем DataTable для хранения данных
                                DataTable dataTable = new DataTable();

                                // Заполняем DataTable данными из базы данных
                                dataTable.Load(reader);

                                // Обновляем источник данных dataGridView1
                                dataGridView1.DataSource = dataTable;
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Ошибка при фильтрации данных: {ex.Message}");
                // Обработка ошибки при фильтрации данных
            }
        }
    }
}
