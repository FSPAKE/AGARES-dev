namespace FSPAKE.AGARES.Unit
{
    partial class CommentDisplayForm
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
            this.UserIconPicture = new System.Windows.Forms.PictureBox();
            this.DisplayMessageLabel = new System.Windows.Forms.Label();
            this.Front = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FontSize = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.UserIconPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Front)).BeginInit();
            this.SuspendLayout();
            // 
            // UserIconPicture
            // 
            this.UserIconPicture.Location = new System.Drawing.Point(26, 24);
            this.UserIconPicture.Margin = new System.Windows.Forms.Padding(0);
            this.UserIconPicture.Name = "UserIconPicture";
            this.UserIconPicture.Size = new System.Drawing.Size(270, 270);
            this.UserIconPicture.TabIndex = 0;
            this.UserIconPicture.TabStop = false;
            // 
            // DisplayMessageLabel
            // 
            this.DisplayMessageLabel.Font = new System.Drawing.Font("ＭＳ ゴシック", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DisplayMessageLabel.Location = new System.Drawing.Point(336, 24);
            this.DisplayMessageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.DisplayMessageLabel.Name = "DisplayMessageLabel";
            this.DisplayMessageLabel.Size = new System.Drawing.Size(663, 340);
            this.DisplayMessageLabel.TabIndex = 1;
            this.DisplayMessageLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.DisplayMessageLabel_Paint);
            // 
            // Front
            // 
            this.Front.BackColor = System.Drawing.Color.Transparent;
            this.Front.Location = new System.Drawing.Point(0, 0);
            this.Front.Margin = new System.Windows.Forms.Padding(0);
            this.Front.Name = "Front";
            this.Front.Size = new System.Drawing.Size(1021, 414);
            this.Front.TabIndex = 2;
            this.Front.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 311);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 24);
            this.label1.TabIndex = 4;
            this.label1.Text = "フォントサイズ";
            // 
            // FontSize
            // 
            this.FontSize.Font = new System.Drawing.Font("MS UI Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FontSize.FormattingEnabled = true;
            this.FontSize.Items.AddRange(new object[] {
            "12",
            "13",
            "15",
            "16",
            "18",
            "19",
            "20",
            "21"});
            this.FontSize.Location = new System.Drawing.Point(66, 341);
            this.FontSize.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.FontSize.Name = "FontSize";
            this.FontSize.Size = new System.Drawing.Size(186, 50);
            this.FontSize.TabIndex = 5;
            this.FontSize.Text = "16";
            this.FontSize.SelectedIndexChanged += new System.EventHandler(this.fontSize_SelectedIndexChanged);
            // 
            // CommentDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 414);
            this.Controls.Add(this.FontSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DisplayMessageLabel);
            this.Controls.Add(this.UserIconPicture);
            this.Controls.Add(this.Front);
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "CommentDisplayForm";
            this.Text = "CommentDIsplayForm";
            ((System.ComponentModel.ISupportInitialize)(this.UserIconPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Front)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox UserIconPicture;
        private System.Windows.Forms.Label DisplayMessageLabel;
        private System.Windows.Forms.PictureBox Front;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox FontSize;
    }
}