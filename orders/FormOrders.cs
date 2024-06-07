using Npgsql;
using Pizzeria.orders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pizzeria
{
    public partial class FormOrders : Form
    {
        public FormOrders()
        {
            InitializeComponent();
        }

        private void FormOrders_Load(object sender, EventArgs e)
        {
            LoadOrdersToGridView();
        }

        public void LoadOrdersToGridView()
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                //для вкладки Заказы
                string query = "SELECT " +
                       "o.id, " +
                       "o.orderdate, " +
                       "o.totalprice, " +
                       "c.fullname AS client, " +
                       "e.fullname AS employee, " +
                       "o.status " +
                       "FROM orders o " +
                       "JOIN client c ON o.clientid = c.id " +
                       "JOIN employee e ON o.employeeid = e.id " +
                       "ORDER BY o.id ASC";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Связываем столбцы DataGridView с данными из DataTable
                        dataGridView1.DataSource = dataTable;

                        dataGridView1.Columns["id"].HeaderText = "ID";
                        dataGridView1.Columns["orderdate"].HeaderText = "Дата заказа";
                        dataGridView1.Columns["totalprice"].HeaderText = "Итоговая сумма";
                        dataGridView1.Columns["client"].HeaderText = "Клиент";
                        dataGridView1.Columns["employee"].HeaderText = "Сотрудник";
                        dataGridView1.Columns["status"].HeaderText = "Статус заказа";
                    }
                }
                //для вкладки ЗаказыПродукты
                string query2 = "SELECT " +
               "oi.id, " +
               "oi.orderid, " +
               "p.description, " +
               "oi.quantity, " +
               "(p.price * oi.quantity) AS total_price " +
               "FROM orderitems oi " +
               "JOIN product p ON oi.productid = p.id " +
               "ORDER BY oi.orderid ASC";

                using (var command = new NpgsqlCommand(query2, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        // Связываем столбцы DataGridView с данными из DataTable
                        dataGridView2.DataSource = dataTable;

                        dataGridView2.Columns["id"].HeaderText = "ID записи";
                        dataGridView2.Columns["orderid"].HeaderText = "ID заказа";
                        dataGridView2.Columns["description"].HeaderText = "Название продукта";
                        dataGridView2.Columns["quantity"].HeaderText = "Количество";
                        dataGridView2.Columns["total_price"].HeaderText = "Стоимость";
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormAddOrder formAddOrder = new FormAddOrder();
            formAddOrder.IsNew = true;
            formAddOrder.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormMainMenu formMainMenu = new FormMainMenu();
            this.Close();
            formMainMenu.Show();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                // Проверяем, что searchText содержит только буквы, пробелы и точки
                if (IsValidFullName(searchText))
                {
                    using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            string query = @"
                        SELECT o.id, o.orderdate, c.fullname, e.fullname, o.status
                        FROM orders o
                        JOIN client c ON o.clientid = c.id
                        JOIN employee e ON o.employeeid = e.id
                        WHERE c.fullname ILIKE @searchText
                    "; 

                            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@searchText", $"%{searchText}%");

                                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                                {
                                    DataTable dataTable = new DataTable();
                                    adapter.Fill(dataTable);

                                    // Обновление источника данных для dataGridView1
                                    dataGridView1.DataSource = dataTable;

                                    dataGridView1.Columns["fullname"].HeaderText = "Клиент";
                                    dataGridView1.Columns["fullname1"].HeaderText = "Сотрудник";
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректное ФИО (можно использовать точки для сокращения инициалов).", "Ошибка ввода");
                    textBox1.Clear();
                }
            }
            else
            {
                // Очистка dataGridView1, если поле поиска пустое
                dataGridView1.DataSource = null;
            }
        }

        private bool IsValidFullName(string fullName)
        {
            string pattern = @"^[a-zA-Zа-яА-Я\s\.]+$";
            return Regex.IsMatch(fullName, pattern);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            LoadOrdersToGridView();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получение идентификатора выбранного заказа
                int selectedOrderId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                // Создание экземпляра формы для редактирования заказа
                FormAddOrder formAddOrder = new FormAddOrder();

                // Загрузка данных выбранного заказа
                formAddOrder.LoadOrderUpdate(selectedOrderId);

                // Установка флага, что это не новый заказ
                formAddOrder.IsNew = false;

                // Отображение формы редактирования заказа
                if (formAddOrder.ShowDialog() == DialogResult.OK)
                {
                    LoadOrdersToGridView();
                }
            }
            else
            {
                // Если ни один заказ не выбран, выводим сообщение
                MessageBox.Show("Выберите заказ для редактирования.");
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Получаем выбранную строку в DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                int clientId = (int)selectedRow.Cells["id"].Value;

                if (MessageBox.Show("Вы действительно хотите удалить этот заказ?", "Подтверждение удаления", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        // Создаем соединение с базой данных
                        using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                        {
                            if (connection.State == ConnectionState.Open)
                            {
                                // Создаем SQL-запрос для удаления продукта из таблицы Product
                                string query = "DELETE FROM orders WHERE \"id\" = @id";
                                // Создаем команду для выполнения SQL-запроса
                                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                                {
                                    // Передаем параметры в SQL-запрос
                                    command.Parameters.AddWithValue("@id", clientId);

                                    // Выполняем SQL-запрос
                                    command.ExecuteNonQuery();
                                }

                                // Обновляем данные в DataGridView
                                LoadOrdersToGridView();
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

        private void buttonProductsList_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // Проверяем, что textBoxOrderId содержит только числовое значение
            if (!int.TryParse(textBox2.Text, out int orderId))
            {
                MessageBox.Show("Пожалуйста, введите число в поле ID заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var connection = DatabaseConnection.GetConnection())
            {
                string query = "SELECT " +
                               "oi.id, " +
                               "oi.orderid, " +
                               "p.description, " +
                               "oi.quantity, " +
                               "(p.price * oi.quantity) AS total_price " +
                               "FROM orderitems oi " +
                               "JOIN product p ON oi.productid = p.id " +
                               "WHERE oi.orderid = @orderId " +
                               "ORDER BY oi.orderid ASC";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderId", orderId);

                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        dataGridView2.DataSource = dataTable;

                        dataGridView2.Columns["id"].HeaderText = "ID записи";
                        dataGridView2.Columns["orderid"].HeaderText = "ID заказа";
                        dataGridView2.Columns["description"].HeaderText = "Название продукта";
                        dataGridView2.Columns["quantity"].HeaderText = "Количество";
                        dataGridView2.Columns["total_price"].HeaderText = "Стоимость";
                    }
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonBack2_Click(object sender, EventArgs e)
        {
            FormMainMenu formMainMenu = new FormMainMenu();
            this.Close();
            formMainMenu.Show();
        }

        private void buttonRefresh2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            LoadOrdersToGridView();
        }

        private void buttonAdd2_Click(object sender, EventArgs e)
        {
            FormAddOrder2 formAddOrder2 = new FormAddOrder2();
            formAddOrder2.IsNew = true;
            formAddOrder2.Show();
        }

        private void buttonChange2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                // Получение идентификатора выбранного заказа
                int selectedOrderItemsId = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["id"].Value);

                // Создание экземпляра формы для редактирования заказа
                FormAddOrder2 formAddOrder2 = new FormAddOrder2();

                // Загрузка данных выбранного заказа
                formAddOrder2.LoadOrderItemsUpdate(selectedOrderItemsId);

                // Установка флага, что это не новый заказ
                formAddOrder2.IsNew = false;

                // Отображение формы редактирования заказа
                if (formAddOrder2.ShowDialog() == DialogResult.OK)
                {
                    LoadOrdersToGridView();
                }
            }
            else
            {
                // Если ни один заказ не выбран, выводим сообщение
                MessageBox.Show("Выберите заказ для редактирования.");
            }
        }

        private void buttonDelete2_Click(object sender, EventArgs e)
        {
            // Получаем выбранную строку в dataGridView2
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                // Получаем идентификатор записи в orderitems
                int orderItemId = Convert.ToInt32(selectedRow.Cells["id"].Value);

                // Показываем диалоговое окно с запросом подтверждения
                DialogResult result = MessageBox.Show($"Вы точно хотите удалить запись с ID {orderItemId}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Удаляем запись из orderitems
                        using (var connection = DatabaseConnection.GetConnection())
                        {
                            string deleteQuery = "DELETE FROM orderitems WHERE id = @id";
                            using (var command = new NpgsqlCommand(deleteQuery, connection))
                            {
                                command.Parameters.AddWithValue("@id", orderItemId);
                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    // Обновляем данные в dataGridView2
                                    LoadOrdersToGridView();
                                }
                                else
                                {
                                    MessageBox.Show("Не удалось удалить запись.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Equals("P0001"))
                        {
                            MessageBox.Show($"Ошибка триггера: {ex.Message}");
                        }
                        else
                        {
                            // Обрабатываем другие ошибки PostgreSQL
                            MessageBox.Show($"Произошла ошибка при удалении записи: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку, которую хотите удалить.");
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
