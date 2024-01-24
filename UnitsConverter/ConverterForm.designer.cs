namespace UnitsConverter
{
    partial class ConverterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConverterForm));
            this.btnSource = new System.Windows.Forms.Button();
            this.txtSrcPath = new System.Windows.Forms.TextBox();
            this.grpSrc = new System.Windows.Forms.GroupBox();
            this.grpDestination = new System.Windows.Forms.GroupBox();
            this.btnDestination = new System.Windows.Forms.Button();
            this.txtDestPath = new System.Windows.Forms.TextBox();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lstBxUpdates = new System.Windows.Forms.ListBox();
            this.bar = new System.Windows.Forms.ProgressBar();
            this.grpFileType = new System.Windows.Forms.GroupBox();
            this.radioButtonImperial = new System.Windows.Forms.RadioButton();
            this.radioButtonMetric = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.grpSrc.SuspendLayout();
            this.grpDestination.SuspendLayout();
            this.grpFileType.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSource
            // 
            this.btnSource.Location = new System.Drawing.Point(331, 17);
            this.btnSource.Name = "btnSource";
            this.btnSource.Size = new System.Drawing.Size(75, 24);
            this.btnSource.TabIndex = 0;
            this.btnSource.Text = "Browse";
            this.btnSource.UseVisualStyleBackColor = true;
            this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
            // 
            // txtSrcPath
            // 
            this.txtSrcPath.Location = new System.Drawing.Point(6, 19);
            this.txtSrcPath.Name = "txtSrcPath";
            this.txtSrcPath.Size = new System.Drawing.Size(320, 20);
            this.txtSrcPath.TabIndex = 1;
            // 
            // grpSrc
            // 
            this.grpSrc.Controls.Add(this.txtSrcPath);
            this.grpSrc.Controls.Add(this.btnSource);
            this.grpSrc.Location = new System.Drawing.Point(10, 11);
            this.grpSrc.Name = "grpSrc";
            this.grpSrc.Size = new System.Drawing.Size(415, 53);
            this.grpSrc.TabIndex = 2;
            this.grpSrc.TabStop = false;
            this.grpSrc.Text = "Source";
            // 
            // grpDestination
            // 
            this.grpDestination.Controls.Add(this.btnDestination);
            this.grpDestination.Controls.Add(this.txtDestPath);
            this.grpDestination.Location = new System.Drawing.Point(10, 70);
            this.grpDestination.Name = "grpDestination";
            this.grpDestination.Size = new System.Drawing.Size(415, 58);
            this.grpDestination.TabIndex = 3;
            this.grpDestination.TabStop = false;
            this.grpDestination.Text = "Destination";
            // 
            // btnDestination
            // 
            this.btnDestination.Location = new System.Drawing.Point(332, 23);
            this.btnDestination.Name = "btnDestination";
            this.btnDestination.Size = new System.Drawing.Size(75, 24);
            this.btnDestination.TabIndex = 1;
            this.btnDestination.Text = "Browse";
            this.btnDestination.UseVisualStyleBackColor = true;
            this.btnDestination.Click += new System.EventHandler(this.btnDestination_Click);
            // 
            // txtDestPath
            // 
            this.txtDestPath.Location = new System.Drawing.Point(6, 24);
            this.txtDestPath.Name = "txtDestPath";
            this.txtDestPath.Size = new System.Drawing.Size(320, 20);
            this.txtDestPath.TabIndex = 0;
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(261, 149);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(75, 24);
            this.btnUpgrade.TabIndex = 4;
            this.btnUpgrade.Text = "Convert";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(342, 149);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lstBxUpdates
            // 
            this.lstBxUpdates.FormattingEnabled = true;
            this.lstBxUpdates.HorizontalScrollbar = true;
            this.lstBxUpdates.Location = new System.Drawing.Point(10, 188);
            this.lstBxUpdates.Name = "lstBxUpdates";
            this.lstBxUpdates.ScrollAlwaysVisible = true;
            this.lstBxUpdates.Size = new System.Drawing.Size(416, 147);
            this.lstBxUpdates.TabIndex = 6;
            // 
            // bar
            // 
            this.bar.Location = new System.Drawing.Point(10, 341);
            this.bar.Name = "bar";
            this.bar.Size = new System.Drawing.Size(415, 23);
            this.bar.TabIndex = 7;
            // 
            // grpFileType
            // 
            this.grpFileType.Controls.Add(this.radioButtonImperial);
            this.grpFileType.Controls.Add(this.radioButtonMetric);
            this.grpFileType.Location = new System.Drawing.Point(10, 134);
            this.grpFileType.Name = "grpFileType";
            this.grpFileType.Size = new System.Drawing.Size(235, 48);
            this.grpFileType.TabIndex = 8;
            this.grpFileType.TabStop = false;
            this.grpFileType.Text = "Convert to";
            // 
            // radioButtonImperial
            // 
            this.radioButtonImperial.AutoSize = true;
            this.radioButtonImperial.Location = new System.Drawing.Point(111, 19);
            this.radioButtonImperial.Name = "radioButtonImperial";
            this.radioButtonImperial.Size = new System.Drawing.Size(88, 17);
            this.radioButtonImperial.TabIndex = 1;
            this.radioButtonImperial.TabStop = true;
            this.radioButtonImperial.Text = "Imperial Units";
            this.radioButtonImperial.UseVisualStyleBackColor = true;
            // 
            // radioButtonMetric
            // 
            this.radioButtonMetric.AutoSize = true;
            this.radioButtonMetric.Location = new System.Drawing.Point(6, 18);
            this.radioButtonMetric.Name = "radioButtonMetric";
            this.radioButtonMetric.Size = new System.Drawing.Size(81, 17);
            this.radioButtonMetric.TabIndex = 0;
            this.radioButtonMetric.TabStop = true;
            this.radioButtonMetric.Text = "Metric Units";
            this.radioButtonMetric.UseVisualStyleBackColor = true;
            this.radioButtonMetric.CheckedChanged += new System.EventHandler(this.radioButtonMetric_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(105, 374);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "v1.210206";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(7, 374);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(92, 13);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "www.revit.com.au";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // ConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(436, 396);
            this.ControlBox = false;
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grpFileType);
            this.Controls.Add(this.bar);
            this.Controls.Add(this.lstBxUpdates);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.grpDestination);
            this.Controls.Add(this.grpSrc);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConverterForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Revit Units Converter";
            this.Load += new System.EventHandler(this.UpgraderForm_Load);
            this.grpSrc.ResumeLayout(false);
            this.grpSrc.PerformLayout();
            this.grpDestination.ResumeLayout(false);
            this.grpDestination.PerformLayout();
            this.grpFileType.ResumeLayout(false);
            this.grpFileType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSource;
        private System.Windows.Forms.TextBox txtSrcPath;
        private System.Windows.Forms.GroupBox grpSrc;
        private System.Windows.Forms.GroupBox grpDestination;
        private System.Windows.Forms.Button btnDestination;
        private System.Windows.Forms.TextBox txtDestPath;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lstBxUpdates;
        private System.Windows.Forms.ProgressBar bar;
        private System.Windows.Forms.GroupBox grpFileType;
        private System.Windows.Forms.RadioButton radioButtonImperial;
        private System.Windows.Forms.RadioButton radioButtonMetric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}