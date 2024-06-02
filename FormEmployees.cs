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
    public partial class FormEmployees : Form
    {
        public FormEmployees()
        {
            InitializeComponent();
        }

        private void FormEmployees_Load(object sender, EventArgs e)
        {
            LoadEmployeesToGridView();
        }

        public void LoadEmployeesToGridView()
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                string query = "SELECT * FROM employee ORDER BY \"id\" ASC";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Связываем столбцы DataGridView с данными из DataTable
                        dataGridView1.DataSource = dataTable;

                        dataGridView1.Columns["id"].HeaderText = "ID";
                        dataGridView1.Columns["fullname"].HeaderText = "ФИО";
                        dataGridView1.Columns["phonenumber"].HeaderText = "Номер телефона";
                        dataGridView1.Columns["position"].HeaderText = "Должность";
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = "";
            LoadEmployeesToGridView();
        }

        private void buttonAddEmployee_Click(object sender, EventArgs e)
        {
            FormAddEmployee formAddEmployee = new FormAddEmployee();
            formAddEmployee.Show();
        }

        private void buttonChangeEmployee_Click(object sender, EventArgs e)
        {
            // Получаем выбранную строку в DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем значения из выбранной строки
                int clientId = (int)selectedRow.Cells["id"].Value;
                string fullName = selectedRow.Cells["fullname"].Value.ToString();
                string phoneNumber = selectedRow.Cells["phonenumber"].Value.ToString();
                string position = selectedRow.Cells["position"].Value.ToString();

                // Открываем форму formAddEmployee и заполняем ее данными
                FormAddEmployee formAddEmployee = new FormAddEmployee();
                formAddEmployee.EmployeeId = clientId;
                formAddEmployee.TextBoxFullName = fullName;
                formAddEmployee.TextBoxPhoneNumber = phoneNumber;
                formAddEmployee.TextBoxPosition = position;

                // Показываем форму
                if (formAddEmployee.ShowDialog() == DialogResult.OK)
                {
                    // Обновляем данные в DataGridView
                    LoadEmployeesToGridView();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для изменения.");
            }
        }

        private void buttonDeleteEmployee_Click(object sender, EventArgs e)
        {
            // Получаем выбранную строку в DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                int clientId = (int)selectedRow.Cells["id"].Value;

                if (MessageBox.Show("Вы действительно хотите удалить этого сотрудника?", "Подтверждение удаления", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        // Создаем соединение с базой данных
                        using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                        {
                            if (connection.State == ConnectionState.Open)
                            {
                                // Создаем SQL-запрос для удаления продукта из таблицы Product
                                string query = "DELETE FROM employee WHERE \"id\" = @id";
                                // Создаем команду для выполнения SQL-запроса
                                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                                {
                                    // Передаем параметры в SQL-запрос
                                    command.Parameters.AddWithValue("@id", clientId);

                                    // Выполняем SQL-запрос
                                    command.ExecuteNonQuery();
                                }

                                // Обновляем данные в DataGridView
                                LoadEmployeesToGridView();
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
                MessageBox.Show("Пожалуйста, выберите клиента для удаления.");
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            FormMainMenu formMainMenu = new FormMainMenu();
            this.Hide();
            formMainMenu.Show();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            FilterEmployeesByName();
        }

        private void FilterEmployeesByName()
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
                        string query = "SELECT * FROM employee WHERE LOWER(\"fullname\") LIKE @searchText ORDER BY \"id\" ASC";
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
    }
}
