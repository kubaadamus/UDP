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
            this.Wbutton = new System.Windows.Forms.Button();
            this.Sbutton = new System.Windows.Forms.Button();
            this.Abutton = new System.Windows.Forms.Button();
            this.Dbutton = new System.Windows.Forms.Button();
            this.DebugConsole = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.button3 = new System.Windows.Forms.Button();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.PasswordButton = new System.Windows.Forms.Button();
            this.ArduinoTestButton = new System.Windows.Forms.Button();
            this.But8 = new System.Windows.Forms.Button();
            this.But5 = new System.Windows.Forms.Button();
            this.But4 = new System.Windows.Forms.Button();
            this.But6 = new System.Windows.Forms.Button();
            this.MachajButton = new System.Windows.Forms.Button();
            this.WitajButton = new System.Windows.Forms.Button();
            this.ŻegnajButton = new System.Windows.Forms.Button();
            this.OtwórzButton = new System.Windows.Forms.Button();
            this.ZamknijButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(592, 460);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(610, 476);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cam 1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(610, 504);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cam 2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Wbutton
            // 
            this.Wbutton.Location = new System.Drawing.Point(66, 478);
            this.Wbutton.Name = "Wbutton";
            this.Wbutton.Size = new System.Drawing.Size(45, 33);
            this.Wbutton.TabIndex = 4;
            this.Wbutton.Text = "W";
            this.Wbutton.UseVisualStyleBackColor = true;
            // 
            // Sbutton
            // 
            this.Sbutton.Location = new System.Drawing.Point(66, 520);
            this.Sbutton.Name = "Sbutton";
            this.Sbutton.Size = new System.Drawing.Size(45, 33);
            this.Sbutton.TabIndex = 5;
            this.Sbutton.Text = "S";
            this.Sbutton.UseVisualStyleBackColor = true;
            // 
            // Abutton
            // 
            this.Abutton.Location = new System.Drawing.Point(15, 520);
            this.Abutton.Name = "Abutton";
            this.Abutton.Size = new System.Drawing.Size(45, 33);
            this.Abutton.TabIndex = 6;
            this.Abutton.Text = "A";
            this.Abutton.UseVisualStyleBackColor = true;
            // 
            // Dbutton
            // 
            this.Dbutton.Location = new System.Drawing.Point(117, 520);
            this.Dbutton.Name = "Dbutton";
            this.Dbutton.Size = new System.Drawing.Size(45, 33);
            this.Dbutton.TabIndex = 7;
            this.Dbutton.Text = "D";
            this.Dbutton.UseVisualStyleBackColor = true;
            // 
            // DebugConsole
            // 
            this.DebugConsole.Location = new System.Drawing.Point(610, 25);
            this.DebugConsole.Multiline = true;
            this.DebugConsole.Name = "DebugConsole";
            this.DebugConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.DebugConsole.Size = new System.Drawing.Size(398, 447);
            this.DebugConsole.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(753, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Feedback From Arduino";
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.LargeChange = 1;
            this.hScrollBar1.Location = new System.Drawing.Point(610, 559);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(178, 17);
            this.hScrollBar1.TabIndex = 10;
            this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(610, 533);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 11;
            this.button3.Text = "Cam 3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(908, 491);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.Size = new System.Drawing.Size(100, 20);
            this.PasswordTextBox.TabIndex = 12;
            // 
            // PasswordButton
            // 
            this.PasswordButton.Location = new System.Drawing.Point(919, 517);
            this.PasswordButton.Name = "PasswordButton";
            this.PasswordButton.Size = new System.Drawing.Size(75, 23);
            this.PasswordButton.TabIndex = 13;
            this.PasswordButton.Text = "Autoryzuj";
            this.PasswordButton.UseVisualStyleBackColor = true;
            this.PasswordButton.Click += new System.EventHandler(this.PasswordButton_Click);
            // 
            // ArduinoTestButton
            // 
            this.ArduinoTestButton.Location = new System.Drawing.Point(691, 476);
            this.ArduinoTestButton.Name = "ArduinoTestButton";
            this.ArduinoTestButton.Size = new System.Drawing.Size(75, 23);
            this.ArduinoTestButton.TabIndex = 14;
            this.ArduinoTestButton.Text = "TestujArduino";
            this.ArduinoTestButton.UseVisualStyleBackColor = true;
            this.ArduinoTestButton.Click += new System.EventHandler(this.ArduinoTestButton_Click);
            // 
            // But8
            // 
            this.But8.Location = new System.Drawing.Point(469, 476);
            this.But8.Name = "But8";
            this.But8.Size = new System.Drawing.Size(43, 36);
            this.But8.TabIndex = 15;
            this.But8.Text = "8";
            this.But8.UseVisualStyleBackColor = true;
            // 
            // But5
            // 
            this.But5.Location = new System.Drawing.Point(469, 518);
            this.But5.Name = "But5";
            this.But5.Size = new System.Drawing.Size(43, 36);
            this.But5.TabIndex = 16;
            this.But5.Text = "5";
            this.But5.UseVisualStyleBackColor = true;
            // 
            // But4
            // 
            this.But4.Location = new System.Drawing.Point(420, 518);
            this.But4.Name = "But4";
            this.But4.Size = new System.Drawing.Size(43, 36);
            this.But4.TabIndex = 17;
            this.But4.Text = "4";
            this.But4.UseVisualStyleBackColor = true;
            // 
            // But6
            // 
            this.But6.Location = new System.Drawing.Point(518, 518);
            this.But6.Name = "But6";
            this.But6.Size = new System.Drawing.Size(43, 36);
            this.But6.TabIndex = 18;
            this.But6.Text = "6";
            this.But6.UseVisualStyleBackColor = true;
            // 
            // MachajButton
            // 
            this.MachajButton.Location = new System.Drawing.Point(12, 566);
            this.MachajButton.Name = "MachajButton";
            this.MachajButton.Size = new System.Drawing.Size(75, 23);
            this.MachajButton.TabIndex = 19;
            this.MachajButton.Text = "Machaj";
            this.MachajButton.UseVisualStyleBackColor = true;
            // 
            // WitajButton
            // 
            this.WitajButton.Location = new System.Drawing.Point(93, 566);
            this.WitajButton.Name = "WitajButton";
            this.WitajButton.Size = new System.Drawing.Size(75, 23);
            this.WitajButton.TabIndex = 20;
            this.WitajButton.Text = "Witaj";
            this.WitajButton.UseVisualStyleBackColor = true;
            // 
            // ŻegnajButton
            // 
            this.ŻegnajButton.Location = new System.Drawing.Point(174, 566);
            this.ŻegnajButton.Name = "ŻegnajButton";
            this.ŻegnajButton.Size = new System.Drawing.Size(75, 23);
            this.ŻegnajButton.TabIndex = 21;
            this.ŻegnajButton.Text = "Żegnaj";
            this.ŻegnajButton.UseVisualStyleBackColor = true;
            // 
            // OtwórzButton
            // 
            this.OtwórzButton.Location = new System.Drawing.Point(255, 566);
            this.OtwórzButton.Name = "OtwórzButton";
            this.OtwórzButton.Size = new System.Drawing.Size(75, 23);
            this.OtwórzButton.TabIndex = 22;
            this.OtwórzButton.Text = "Otwórz";
            this.OtwórzButton.UseVisualStyleBackColor = true;
            // 
            // ZamknijButton
            // 
            this.ZamknijButton.Location = new System.Drawing.Point(336, 566);
            this.ZamknijButton.Name = "ZamknijButton";
            this.ZamknijButton.Size = new System.Drawing.Size(75, 23);
            this.ZamknijButton.TabIndex = 23;
            this.ZamknijButton.Text = "Zamknij";
            this.ZamknijButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 601);
            this.Controls.Add(this.ZamknijButton);
            this.Controls.Add(this.OtwórzButton);
            this.Controls.Add(this.ŻegnajButton);
            this.Controls.Add(this.WitajButton);
            this.Controls.Add(this.MachajButton);
            this.Controls.Add(this.But6);
            this.Controls.Add(this.But4);
            this.Controls.Add(this.But5);
            this.Controls.Add(this.But8);
            this.Controls.Add(this.ArduinoTestButton);
            this.Controls.Add(this.PasswordButton);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DebugConsole);
            this.Controls.Add(this.Dbutton);
            this.Controls.Add(this.Abutton);
            this.Controls.Add(this.Sbutton);
            this.Controls.Add(this.Wbutton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Wbutton;
        private System.Windows.Forms.Button Sbutton;
        private System.Windows.Forms.Button Abutton;
        private System.Windows.Forms.Button Dbutton;
        private System.Windows.Forms.TextBox DebugConsole;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Button PasswordButton;
        private System.Windows.Forms.Button ArduinoTestButton;
        private System.Windows.Forms.Button But8;
        private System.Windows.Forms.Button But5;
        private System.Windows.Forms.Button But4;
        private System.Windows.Forms.Button But6;
        private System.Windows.Forms.Button MachajButton;
        private System.Windows.Forms.Button WitajButton;
        private System.Windows.Forms.Button ŻegnajButton;
        private System.Windows.Forms.Button OtwórzButton;
        private System.Windows.Forms.Button ZamknijButton;
    }
}

