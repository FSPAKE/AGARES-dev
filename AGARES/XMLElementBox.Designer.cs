namespace Utils
{
    partial class XMLElementBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ElementBox = new System.Windows.Forms.ListBox();
            this.Add = new System.Windows.Forms.Label();
            this.ItemforAdd = new System.Windows.Forms.TextBox();
            this.Delete = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ElementBox
            // 
            this.ElementBox.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ElementBox.FormattingEnabled = true;
            this.ElementBox.ItemHeight = 48;
            this.ElementBox.Location = new System.Drawing.Point(0, 2);
            this.ElementBox.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.ElementBox.Name = "ElementBox";
            this.ElementBox.Size = new System.Drawing.Size(799, 388);
            this.ElementBox.TabIndex = 0;
            this.ElementBox.SelectedIndexChanged += new System.EventHandler(this.ElementBox_SelectedIndexChanged);
            // 
            // Add
            // 
            this.Add.BackColor = System.Drawing.Color.Azure;
            this.Add.Location = new System.Drawing.Point(589, 404);
            this.Add.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(98, 36);
            this.Add.TabIndex = 1;
            this.Add.Text = "追加";
            this.Add.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // ItemforAdd
            // 
            this.ItemforAdd.Location = new System.Drawing.Point(37, 404);
            this.ItemforAdd.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.ItemforAdd.Multiline = true;
            this.ItemforAdd.Name = "ItemforAdd";
            this.ItemforAdd.Size = new System.Drawing.Size(541, 36);
            this.ItemforAdd.TabIndex = 2;
            // 
            // Delete
            // 
            this.Delete.BackColor = System.Drawing.Color.Azure;
            this.Delete.Location = new System.Drawing.Point(693, 404);
            this.Delete.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(98, 36);
            this.Delete.TabIndex = 3;
            this.Delete.Text = "削除";
            this.Delete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // XMLElementBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.ItemforAdd);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.ElementBox);
            this.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.Name = "XMLElementBox";
            this.Text = "単語一覧";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XMLElementBox_FormClosing);
            this.Load += new System.EventHandler(this.XMLElementBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ElementBox;
        private System.Windows.Forms.Label Add;
        private System.Windows.Forms.TextBox ItemforAdd;
        private System.Windows.Forms.Label Delete;
    }
}