namespace FSRAudioSwitcher
{
    partial class ThresholdPicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThresholdPicker));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.thresholdNumber = new System.Windows.Forms.NumericUpDown();
            this.PickButton = new System.Windows.Forms.Button();
            this.CurrentMeasurementBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(305, 11);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.Button1_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(224, 11);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // thresholdNumber
            // 
            this.thresholdNumber.Location = new System.Drawing.Point(12, 12);
            this.thresholdNumber.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.thresholdNumber.Name = "thresholdNumber";
            this.thresholdNumber.Size = new System.Drawing.Size(125, 20);
            this.thresholdNumber.TabIndex = 3;
            // 
            // PickButton
            // 
            this.PickButton.Location = new System.Drawing.Point(143, 11);
            this.PickButton.Name = "PickButton";
            this.PickButton.Size = new System.Drawing.Size(75, 23);
            this.PickButton.TabIndex = 4;
            this.PickButton.Text = "Pick +20%";
            this.PickButton.UseVisualStyleBackColor = true;
            this.PickButton.Click += new System.EventHandler(this.PickButton_Click);
            // 
            // CurrentMeasurementBox
            // 
            this.CurrentMeasurementBox.Enabled = false;
            this.CurrentMeasurementBox.Location = new System.Drawing.Point(12, 40);
            this.CurrentMeasurementBox.Name = "CurrentMeasurementBox";
            this.CurrentMeasurementBox.Size = new System.Drawing.Size(368, 20);
            this.CurrentMeasurementBox.TabIndex = 5;
            this.CurrentMeasurementBox.TextChanged += new System.EventHandler(this.CurrentMeasurementBox_Click);
            // 
            // ThresholdPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 72);
            this.Controls.Add(this.CurrentMeasurementBox);
            this.Controls.Add(this.PickButton);
            this.Controls.Add(this.thresholdNumber);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ThresholdPicker";
            this.Text = "Pick measurement threshold";
            this.Load += new System.EventHandler(this.ComPortPicker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.thresholdNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.NumericUpDown thresholdNumber;
        private System.Windows.Forms.Button PickButton;
        private System.Windows.Forms.TextBox CurrentMeasurementBox;
    }
}