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

namespace Pizzeria.orders
{
    public partial class FormAddOrder : Form
    {
        public FormAddOrder()
        {
            InitializeComponent();
            LoadClientsAndEmployees();
        }

        private dynamic _selectedClient;
        private dynamic _selectedEmployee;

        public dynamic SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                comboBoxClient.SelectedItem = value;
            }
        }

        public dynamic SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                comboBoxEmployee.SelectedItem = value;
            }
        }

        public DateTime OrderDate
        {
            get { return dateTimePicker1.Value; }
            set { dateTimePicker1.Value = value; }
        }

        public int? OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

        private int? _orderId;

        private bool _isNew;

        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
            }
        }

        public string TextBoxStatus
        {
            get { return textBoxStatus.Text; }
            set { textBoxStatus.Text = value; }
        }
        public void LoadClientsAndEmployees()
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                // Загрузка клиентов
                string clientQuery = "SELECT id, fullname FROM client";
                using (var clientCommand = new NpgsqlCommand(clientQuery, connection))
                {
                    using (var clientReader = clientCommand.ExecuteReader())
                    {
                        comboBoxClient.Items.Clear();
                        while (clientReader.Read())
                        {
                            comboBoxClient.Items.Add(new { Id = clientReader.GetInt32(0), FullName = clientReader.GetString(1) });
                        }
                    }
                }

                // Загрузка сотрудников
                string employeeQuery = "SELECT id, fullname FROM employee";
                using (var employeeCommand = new NpgsqlCommand(employeeQuery, connection))
                {
                    using (var employeeReader = employeeCommand.ExecuteReader())
                    {
                        comboBoxEmployee.Items.Clear();
                        while (employeeReader.Read())
                        {
                            comboBoxEmployee.Items.Add(new { Id = employeeReader.GetInt32(0), FullName = employeeReader.GetString(1) });
                        }
                    }
                }
            }

            // Установка отображаемого поля и значения для комбобоксов
            comboBoxClient.DisplayMember = "FullName";
            comboBoxClient.ValueMember = "Id";

            comboBoxEmployee.DisplayMember = "FullName";
            comboBoxEmployee.ValueMember = "Id";
        }


        private void FormAddOrder_Load(object sender, EventArgs e)
        {

        }

        private void comboBoxClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем выбранного клиента
            var selectedClient = (dynamic)comboBoxClient.SelectedItem;
            int selectedClientId = selectedClient.Id;
        }

        private void comboBoxEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем выбранного сотрудника
            var selectedEmployee = (dynamic)comboBoxEmployee.SelectedItem;
            int selectedEmployeeId = selectedEmployee.Id;
        }

        public void LoadOrderUpdate(int orderId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                string selectQuery = "SELECT clientid, employeeid, orderdate, status FROM orders WHERE id = @id";
                using (var command = new NpgsqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", orderId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Загрузка данных клиента
                            int clientId = reader.GetInt32(0);
                            var selectedClient = comboBoxClient.Items.Cast<dynamic>()
                                .FirstOrDefault(c => (int)c.Id == clientId);
                            if (selectedClient != null)
                            {
                                comboBoxClient.SelectedItem = selectedClient;
                            }

                            // Загрузка данных сотрудника
                            int employeeId = reader.GetInt32(1);
                            var selectedEmployee = comboBoxEmployee.Items.Cast<dynamic>()
                                .FirstOrDefault(e => (int)e.Id == employeeId);
                            if (selectedEmployee != null)
                            {
                                comboBoxEmployee.SelectedItem = selectedEmployee;
                            }

                            // Загрузка даты заказа
                            OrderDate = reader.GetDateTime(2);

                            // Установка идентификатора заказа
                            OrderId = orderId;

                            TextBoxStatus = reader.GetString(3);

                            // Установка флага, что это не новый заказ
                            IsNew = false;
                        }
                    }
                }
            }
        }

        public void LoadOrderAdd()
        {
            SelectedClient = (dynamic)comboBoxClient.Items[comboBoxClient.SelectedIndex];
            SelectedEmployee = (dynamic)comboBoxEmployee.Items[comboBoxEmployee.SelectedIndex];
            OrderDate = dateTimePicker1.Value;
            TextBoxStatus = textBoxStatus.Text;
        }


        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        if (!IsNew)
                        {
                            // Обновление существующего заказа
                            string updateQuery = "UPDATE orders SET orderdate = @orderdate, clientid = @clientid, employeeid = @employeeid, status = @status WHERE id = @id";
                            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@id", OrderId);
                                command.Parameters.AddWithValue("@clientid", ((dynamic)comboBoxClient.SelectedItem).Id);
                                command.Parameters.AddWithValue("@employeeid", ((dynamic)comboBoxEmployee.SelectedItem).Id);
                                command.Parameters.AddWithValue("@orderdate", OrderDate);
                                command.Parameters.AddWithValue("@status", TextBoxStatus);

                                Console.WriteLine($"Updating order with status: {TextBoxStatus}");
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Добавление нового заказа
                            LoadOrderAdd();;
                            string query = "INSERT INTO orders (orderdate, clientid, employeeid, status) VALUES (@orderdate, @clientid, @employeeid, @status)";
                            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@orderdate", OrderDate);
                                command.Parameters.AddWithValue("@clientid", ((dynamic)SelectedClient).Id);
                                command.Parameters.AddWithValue("@employeeid", ((dynamic)SelectedEmployee).Id);
                                command.Parameters.AddWithValue("@status", TextBoxStatus);

                                command.ExecuteNonQuery();
                            }
                        }

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                // Обработка ошибок
                if (ex.Message.Contains("P0001"))
                {
                    MessageBox.Show($"Ошибка базы данных: {ex.Message}\nТранзакция завершилась в триггере. Выполнение пакета было прервано.", "Ошибка");
                }
                else
                {
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

