namespace Wall_E
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ActionButton = new System.Windows.Forms.Button();
            this.lab1 = new System.Windows.Forms.Label();
            this.Commands = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.Sequence = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ActionButton
            // 
            this.ActionButton.BackColor = System.Drawing.Color.IndianRed;
            this.ActionButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ActionButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.ActionButton.Location = new System.Drawing.Point(67, 499);
            this.ActionButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ActionButton.Name = "ActionButton";
            this.ActionButton.Size = new System.Drawing.Size(259, 52);
            this.ActionButton.TabIndex = 0;
            this.ActionButton.Text = "Process Commands";
            this.ActionButton.UseVisualStyleBackColor = false;
            this.ActionButton.Click += new System.EventHandler(this.ActionButton_Click);
            // 
            // lab1
            // 
            this.lab1.AutoSize = true;
            this.lab1.Font = new System.Drawing.Font("Arial Narrow", 11F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.lab1.Location = new System.Drawing.Point(54, 88);
            this.lab1.Name = "lab1";
            this.lab1.Size = new System.Drawing.Size(290, 26);
            this.lab1.TabIndex = 1;
            this.lab1.Text = "Introduce the commands in here";
            // 
            // Commands
            // 
            this.Commands.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Commands.Location = new System.Drawing.Point(45, 118);
            this.Commands.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Commands.Multiline = true;
            this.Commands.Name = "Commands";
            this.Commands.Size = new System.Drawing.Size(321, 363);
            this.Commands.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Silver;
            this.pictureBox1.Location = new System.Drawing.Point(474, 62);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(404, 456);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // ClearButton
            // 
            this.ClearButton.BackColor = System.Drawing.Color.Brown;
            this.ClearButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ClearButton.Font = new System.Drawing.Font("Times New Roman", 16F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.ClearButton.Location = new System.Drawing.Point(746, 588);
            this.ClearButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(181, 67);
            this.ClearButton.TabIndex = 4;
            this.ClearButton.Text = "Clean";
            this.ClearButton.UseVisualStyleBackColor = false;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // Sequence
            // 
            this.Sequence.BackColor = System.Drawing.Color.IndianRed;
            this.Sequence.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Sequence.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.Sequence.Location = new System.Drawing.Point(607, 525);
            this.Sequence.Name = "Sequence";
            this.Sequence.Size = new System.Drawing.Size(159, 49);
            this.Sequence.TabIndex = 5;
            this.Sequence.Text = "Jump seq";
            this.Sequence.UseVisualStyleBackColor = false;
            this.Sequence.Click += new System.EventHandler(this.Sequence_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(950, 668);
            this.Controls.Add(this.Sequence);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Commands);
            this.Controls.Add(this.lab1);
            this.Controls.Add(this.ActionButton);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "GeoWall-E";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ActionButton;
        private System.Windows.Forms.Label lab1;
        private System.Windows.Forms.TextBox Commands;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button ClearButton;
        private Button Sequence;
    }
}