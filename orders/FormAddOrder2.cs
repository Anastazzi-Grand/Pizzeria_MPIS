using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pizzeria.orders
{
    public partial class FormAddOrder2 : Form
    {
        public FormAddOrder2()
        {
            InitializeComponent();
            FormAddOrder2_Load();
        }

        private dynamic _selectedProduct;
        public dynamic SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                comboBoxProduct.SelectedItem = value;
            }
        }

        private int? _selectedOrderId;
        public int? SelectedOrderId
        {
            get { return _selectedOrderId; }
            set
            {
                _selectedOrderId = value;
                comboBoxOrderId.SelectedItem = value;
            }
        }

        private int? _quantity;
        public int? Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                textBoxQuantity.Text = value.ToString();
            }
        }
        private bool _isNew;

        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
            }
        }

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private void FormAddOrder2_Load()
        {
            // Заполняем комбобокс OrderId данными из базы данных
            using (var connection = DatabaseConnection.GetConnection())
            {
                string ordersQuery = "SELECT id FROM orders";
                using (var command = new NpgsqlCommand(ordersQuery, connection))
                { 
                    using (var reader = command.ExecuteReader())
                    {
                        comboBoxOrderId.Items.Clear();
                        while (reader.Read())
                        {
                            comboBoxOrderId.Items.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            // Заполняем комбобокс Product данными из базы данных
            using (var connection = DatabaseConnection.GetConnection())
            {
                string productsQuery = "SELECT id, description FROM product";
                using (var command = new NpgsqlCommand(productsQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        comboBoxProduct.Items.Clear();
                        while (reader.Read())
                        {
                            comboBoxProduct.Items.Add(new { Id = reader.GetInt32(0), Description = reader.GetString(1) });
                        }
                    }
                }
            }

            // Установка отображаемого поля и значения для комбобоксов
            comboBoxProduct.DisplayMember = "Description";
            comboBoxProduct.ValueMember = "Id";
        }

        public void LoadOrderItemsUpdate(int id)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                string selectQuery = "SELECT orderid, productid, quantity FROM orderitems WHERE id = @id";
                using (var command = new NpgsqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int selectedOrderId = reader.GetInt32(0);
                            comboBoxOrderId.SelectedItem = selectedOrderId;

                            //SelectedOrderId = reader.GetInt32(0);

                            int selectedProductId = reader.GetInt32(1);
                            var selectedProduct = comboBoxProduct.Items.Cast<dynamic>()
                                .FirstOrDefault(c => (int)c.Id == selectedProductId);

                            if (selectedProduct != null)
                            {
                                comboBoxProduct.SelectedItem = selectedProduct;
                            }

                            Quantity = reader.GetInt32(2);

                            Id = id;

                            IsNew = false;
                        }
                    }
                }
            }
        }
        public void LoadOrderItemsAdd()
        {
            SelectedProduct = (dynamic)comboBoxProduct.Items[comboBoxProduct.SelectedIndex];
            SelectedOrderId = comboBoxOrderId.SelectedItem as int?;

            // Получаем значение из textBoxQuantity
            if (int.TryParse(textBoxQuantity.Text, out int quantity))
            {
                Quantity = quantity;
            }
            else
            {
                // Если количество не является числом, сохраняем значение textBoxQuantity.Text
                // и показываем сообщение об ошибке
                Quantity = null;
                MessageBox.Show("Количество должно быть числом", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        if (int.TryParse(textBoxQuantity.Text, out int quantity))
                        {
                            Quantity = quantity;
                            SaveOrderItem(connection);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Количество должно быть числом", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Quantity = null;
                            return; // Возвращаемся, не закрываем форму
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                // Обработка ошибок
                HandleDatabaseError(ex);
            }
        }

        private void SaveOrderItem(NpgsqlConnection connection)
        {
            if (!IsNew)
            {
                // Обновление существующего заказа
                string updateQuery = "UPDATE orderitems SET orderid = @orderid, productid = @productid, quantity = @quantity WHERE id = @id";
                using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", Id);
                    command.Parameters.AddWithValue("@orderid", (dynamic)comboBoxOrderId.SelectedItem);
                    command.Parameters.AddWithValue("@productid", ((dynamic)comboBoxProduct.SelectedItem).Id);
                    command.Parameters.AddWithValue("@quantity", Quantity);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                LoadOrderItemsAdd();
                if (Quantity.HasValue)
                {
                    string insertQuery = "INSERT INTO orderitems (orderid, productid, quantity) VALUES (@orderid, @productid, @quantity)";
                    using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@orderid", (dynamic)SelectedOrderId);
                        insertCommand.Parameters.AddWithValue("@productid", ((dynamic)SelectedProduct).Id);
                        insertCommand.Parameters.AddWithValue("@quantity", Quantity.Value);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private void HandleDatabaseError(NpgsqlException ex)
        {
            if (ex.Message.Contains("P0001"))
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}\nТранзакция завершилась в триггере. Выполнение пакета было прервано.", "Ошибка");
            }
            else
            {
                Console.WriteLine($"Ошибка при сохранении данных в базу данных: {ex.Message}");
            }
        }


        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxOrderId_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedOrderId = (dynamic)comboBoxOrderId.SelectedItem;
        }

        private void comboBoxProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedProduct = (dynamic)comboBoxProduct.SelectedItem;
            int selectedProductId = selectedProduct.Id;
        }
    }
}
