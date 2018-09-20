namespace ClientWinForms_Simple
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Wbutton = new System.Windows.Forms.Button();
            this.Sbutton = new System.Windows.Forms.Button();
            this.Abutton = new System.Windows.Forms.Button();
            this.Dbutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(558, 397);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(658, 357);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "a";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(658, 386);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "b";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(668, 109);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(25, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            this.textBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyUp);
            // 
            // Wbutton
            // 
            this.Wbutton.Location = new System.Drawing.Point(658, 70);
            this.Wbutton.Name = "Wbutton";
            this.Wbutton.Size = new System.Drawing.Size(45, 33);
            this.Wbutton.TabIndex = 4;
            this.Wbutton.Text = "W";
            this.Wbutton.UseVisualStyleBackColor = true;
            // 
            // Sbutton
            // 
            this.Sbutton.Location = new System.Drawing.Point(658, 135);
            this.Sbutton.Name = "Sbutton";
            this.Sbutton.Size = new System.Drawing.Size(45, 33);
            this.Sbutton.TabIndex = 5;
            this.Sbutton.Text = "S";
            this.Sbutton.UseVisualStyleBackColor = true;
            // 
            // Abutton
            // 
            this.Abutton.Location = new System.Drawing.Point(607, 135);
            this.Abutton.Name = "Abutton";
            this.Abutton.Size = new System.Drawing.Size(45, 33);
            this.Abutton.TabIndex = 6;
            this.Abutton.Text = "A";
            this.Abutton.UseVisualStyleBackColor = true;
            // 
            // Dbutton
            // 
            this.Dbutton.Location = new System.Drawing.Point(709, 135);
            this.Dbutton.Name = "Dbutton";
            this.Dbutton.Size = new System.Drawing.Size(45, 33);
            this.Dbutton.TabIndex = 7;
            this.Dbutton.Text = "D";
            this.Dbutton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Dbutton);
            this.Controls.Add(this.Abutton);
            this.Controls.Add(this.Sbutton);
            this.Controls.Add(this.Wbutton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Wbutton;
        private System.Windows.Forms.Button Sbutton;
        private System.Windows.Forms.Button Abutton;
        private System.Windows.Forms.Button Dbutton;
    }
}

