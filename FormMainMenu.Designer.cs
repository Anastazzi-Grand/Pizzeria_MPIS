namespace Pizzeria
{
    partial class FormMainMenu
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainMenu));
            this.buttonProducts = new System.Windows.Forms.Button();
            this.buttonOrders = new System.Windows.Forms.Button();
            this.buttonEmployees = new System.Windows.Forms.Button();
            this.buttonClients = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonProducts
            // 
            this.buttonProducts.BackColor = System.Drawing.SystemColors.Info;
            this.buttonProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonProducts.Location = new System.Drawing.Point(397, 65);
            this.buttonProducts.Margin = new System.Windows.Forms.Padding(4);
            this.buttonProducts.Name = "buttonProducts";
            this.buttonProducts.Size = new System.Drawing.Size(459, 94);
            this.buttonProducts.TabIndex = 0;
            this.buttonProducts.Text = "Меню";
            this.buttonProducts.UseVisualStyleBackColor = false;
            this.buttonProducts.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonOrders
            // 
            this.buttonOrders.BackColor = System.Drawing.SystemColors.Info;
            this.buttonOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonOrders.Location = new System.Drawing.Point(397, 218);
            this.buttonOrders.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOrders.Name = "buttonOrders";
            this.buttonOrders.Size = new System.Drawing.Size(459, 94);
            this.buttonOrders.TabIndex = 1;
            this.buttonOrders.Text = "Заказ";
            this.buttonOrders.UseVisualStyleBackColor = false;
            this.buttonOrders.Click += new System.EventHandler(this.buttonOrders_Click);
            // 
            // buttonEmployees
            // 
            this.buttonEmployees.BackColor = System.Drawing.SystemColors.Info;
            this.buttonEmployees.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonEmployees.Location = new System.Drawing.Point(397, 370);
            this.buttonEmployees.Margin = new System.Windows.Forms.Padding(4);
            this.buttonEmployees.Name = "buttonEmployees";
            this.buttonEmployees.Size = new System.Drawing.Size(459, 94);
            this.buttonEmployees.TabIndex = 2;
            this.buttonEmployees.Text = "Сотрудники";
            this.buttonEmployees.UseVisualStyleBackColor = false;
            this.buttonEmployees.Click += new System.EventHandler(this.buttonEmployees_Click);
            // 
            // buttonClients
            // 
            this.buttonClients.BackColor = System.Drawing.SystemColors.Info;
            this.buttonClients.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonClients.Location = new System.Drawing.Point(397, 523);
            this.buttonClients.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClients.Name = "buttonClients";
            this.buttonClients.Size = new System.Drawing.Size(459, 94);
            this.buttonClients.TabIndex = 3;
            this.buttonClients.Text = "Клиенты";
            this.buttonClients.UseVisualStyleBackColor = false;
            this.buttonClients.Click += new System.EventHandler(this.button4_Click);
            // 
            // FormMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1327, 699);
            this.Controls.Add(this.buttonClients);
            this.Controls.Add(this.buttonEmployees);
            this.Controls.Add(this.buttonOrders);
            this.Controls.Add(this.buttonProducts);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormMainMenu";
            this.Text = "Пиццерия";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonProducts;
        private System.Windows.Forms.Button buttonOrders;
        private System.Windows.Forms.Button buttonEmployees;
        private System.Windows.Forms.Button buttonClients;
    }
}

