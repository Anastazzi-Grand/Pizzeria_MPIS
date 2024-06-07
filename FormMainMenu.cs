
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
    public partial class FormMainMenu : Form
    {
        public FormMainMenu()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormProducts form2 = new FormProducts();
            this.Hide();
            form2.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormClients formClients = new FormClients();
            this.Hide();
            formClients.Show();

        }

        private void buttonEmployees_Click(object sender, EventArgs e)
        {
            FormEmployees formEmployees = new FormEmployees();
            this.Hide();
            formEmployees.Show();
        }

        private void buttonOrders_Click(object sender, EventArgs e)
        {
           FormOrders formOrders = new FormOrders();
            //FormOrder2 formOrder2 = new FormOrder2();
            this.Hide();
            formOrders.Show();
           //formOrder2.Show();
        }
    }
}
