namespace HolidayTest1
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.buttonBegin = new System.Windows.Forms.Button();
            this.buttonGetNext = new System.Windows.Forms.Button();
            this.buttonAllocate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelState = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSerial = new System.Windows.Forms.TextBox();
            this.buttonRePrint = new System.Windows.Forms.Button();
            this.listViewStatus = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listViewLog = new System.Windows.Forms.ListView();
            this.columnHeader0 = new System.Windows.Forms.ColumnHeader();
            this.panel4 = new System.Windows.Forms.Panel();
            this.comboBoxStationNum = new System.Windows.Forms.ComboBox();
            this.labelStationNum = new System.Windows.Forms.Label();
            this.buttonCopyText = new System.Windows.Forms.Button();
            this.buttonNewBatch = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(94, 103);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            this.splitContainer1.Panel1.Controls.Add(this.buttonGetNext);
            this.splitContainer1.Panel1.Controls.Add(this.buttonAllocate);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.Controls.Add(this.labelState);
            this.splitContainer1.Panel1MinSize = 120;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(1008, 662);
            this.splitContainer1.SplitterDistance = 120;
            this.splitContainer1.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labelVersion);
            this.panel3.Controls.Add(this.buttonBegin);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(825, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(183, 120);
            this.panel3.TabIndex = 5;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelVersion.Location = new System.Drawing.Point(0, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(183, 35);
            this.labelVersion.TabIndex = 6;
            this.labelVersion.Text = "Version: ...";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonBegin
            // 
            this.buttonBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBegin.ForeColor = System.Drawing.Color.DarkGreen;
            this.buttonBegin.Location = new System.Drawing.Point(18, 57);
            this.buttonBegin.Name = "buttonBegin";
            this.buttonBegin.Size = new System.Drawing.Size(153, 57);
            this.buttonBegin.TabIndex = 2;
            this.buttonBegin.Text = "Begin Test";
            this.buttonBegin.UseVisualStyleBackColor = true;
            this.buttonBegin.Click += new System.EventHandler(this.buttonBegin_Click);
            // 
            // buttonGetNext
            // 
            this.buttonGetNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGetNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGetNext.ForeColor = System.Drawing.Color.MediumBlue;
            this.buttonGetNext.Location = new System.Drawing.Point(641, 12);
            this.buttonGetNext.Name = "buttonGetNext";
            this.buttonGetNext.Size = new System.Drawing.Size(81, 30);
            this.buttonGetNext.TabIndex = 13;
            this.buttonGetNext.Text = "Get Next";
            this.buttonGetNext.UseVisualStyleBackColor = true;
            this.buttonGetNext.Visible = false;
            this.buttonGetNext.Click += new System.EventHandler(this.buttonGetNext_Click);
            // 
            // buttonAllocate
            // 
            this.buttonAllocate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAllocate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAllocate.ForeColor = System.Drawing.Color.MediumBlue;
            this.buttonAllocate.Location = new System.Drawing.Point(728, 12);
            this.buttonAllocate.Name = "buttonAllocate";
            this.buttonAllocate.Size = new System.Drawing.Size(77, 30);
            this.buttonAllocate.TabIndex = 14;
            this.buttonAllocate.Text = "Allocate";
            this.buttonAllocate.UseVisualStyleBackColor = true;
            this.buttonAllocate.Visible = false;
            this.buttonAllocate.Click += new System.EventHandler(this.buttonAllocate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Location = new System.Drawing.Point(110, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(447, 37);
            this.label1.TabIndex = 3;
            this.label1.Text = "MooresCloud Holiday Tester";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelState
            // 
            this.labelState.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelState.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelState.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelState.Location = new System.Drawing.Point(112, 50);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(707, 67);
            this.labelState.TabIndex = 4;
            this.labelState.Text = "Ready for testing";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 340F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.listViewStatus, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listViewLog, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1008, 538);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.comboBoxPort);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(811, 505);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(194, 30);
            this.panel2.TabIndex = 8;
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Location = new System.Drawing.Point(75, 6);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(110, 21);
            this.comboBoxPort.TabIndex = 2;
            this.comboBoxPort.DropDown += new System.EventHandler(this.comboBoxPort_DropDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "COM Port:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBoxSerial);
            this.panel1.Controls.Add(this.buttonRePrint);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 505);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(334, 30);
            this.panel1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Serial Number:";
            // 
            // textBoxSerial
            // 
            this.textBoxSerial.Enabled = false;
            this.textBoxSerial.Location = new System.Drawing.Point(88, 7);
            this.textBoxSerial.Name = "textBoxSerial";
            this.textBoxSerial.Size = new System.Drawing.Size(109, 20);
            this.textBoxSerial.TabIndex = 0;
            // 
            // buttonRePrint
            // 
            this.buttonRePrint.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonRePrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRePrint.ForeColor = System.Drawing.Color.DarkGreen;
            this.buttonRePrint.Location = new System.Drawing.Point(210, 0);
            this.buttonRePrint.Name = "buttonRePrint";
            this.buttonRePrint.Size = new System.Drawing.Size(124, 30);
            this.buttonRePrint.TabIndex = 5;
            this.buttonRePrint.Text = "Re-Print Label";
            this.buttonRePrint.UseVisualStyleBackColor = true;
            this.buttonRePrint.Click += new System.EventHandler(this.buttonRePrint_Click);
            // 
            // listViewStatus
            // 
            this.listViewStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewStatus.FullRowSelect = true;
            this.listViewStatus.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewStatus.Location = new System.Drawing.Point(3, 3);
            this.listViewStatus.Name = "listViewStatus";
            this.listViewStatus.Size = new System.Drawing.Size(334, 496);
            this.listViewStatus.StateImageList = this.imageList1;
            this.listViewStatus.TabIndex = 6;
            this.listViewStatus.UseCompatibleStateImageBehavior = false;
            this.listViewStatus.View = System.Windows.Forms.View.Details;
            this.listViewStatus.Resize += new System.EventHandler(this.listViewStatus_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 200;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "none.png");
            this.imageList1.Images.SetKeyName(1, "here.png");
            this.imageList1.Images.SetKeyName(2, "pass.png");
            this.imageList1.Images.SetKeyName(3, "fail.png");
            this.imageList1.Images.SetKeyName(4, "retry.png");
            this.imageList1.Images.SetKeyName(5, "info.png");
            this.imageList1.Images.SetKeyName(6, "star.png");
            this.imageList1.Images.SetKeyName(7, "warn.png");
            this.imageList1.Images.SetKeyName(8, "restart.png");
            this.imageList1.Images.SetKeyName(9, "power.png");
            this.imageList1.Images.SetKeyName(10, "plus.png");
            this.imageList1.Images.SetKeyName(11, "minus.png");
            // 
            // listViewLog
            // 
            this.listViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0});
            this.tableLayoutPanel1.SetColumnSpan(this.listViewLog, 2);
            this.listViewLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewLog.FullRowSelect = true;
            this.listViewLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewLog.Location = new System.Drawing.Point(343, 3);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(662, 496);
            this.listViewLog.TabIndex = 9;
            this.listViewLog.UseCompatibleStateImageBehavior = false;
            this.listViewLog.View = System.Windows.Forms.View.Details;
            this.listViewLog.Resize += new System.EventHandler(this.listViewLog_Resize);
            // 
            // columnHeader0
            // 
            this.columnHeader0.Width = 300;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.comboBoxStationNum);
            this.panel4.Controls.Add(this.labelStationNum);
            this.panel4.Controls.Add(this.buttonCopyText);
            this.panel4.Controls.Add(this.buttonNewBatch);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(343, 505);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(462, 30);
            this.panel4.TabIndex = 10;
            // 
            // comboBoxStationNum
            // 
            this.comboBoxStationNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStationNum.FormattingEnabled = true;
            this.comboBoxStationNum.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.comboBoxStationNum.Location = new System.Drawing.Point(263, 6);
            this.comboBoxStationNum.Name = "comboBoxStationNum";
            this.comboBoxStationNum.Size = new System.Drawing.Size(48, 21);
            this.comboBoxStationNum.Sorted = true;
            this.comboBoxStationNum.TabIndex = 7;
            this.comboBoxStationNum.SelectedIndexChanged += new System.EventHandler(this.comboBoxStationNum_SelectedIndexChanged);
            // 
            // labelStationNum
            // 
            this.labelStationNum.AutoSize = true;
            this.labelStationNum.Location = new System.Drawing.Point(180, 9);
            this.labelStationNum.Name = "labelStationNum";
            this.labelStationNum.Size = new System.Drawing.Size(77, 13);
            this.labelStationNum.TabIndex = 8;
            this.labelStationNum.Text = "Test Station #:";
            // 
            // buttonCopyText
            // 
            this.buttonCopyText.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonCopyText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCopyText.ForeColor = System.Drawing.Color.MediumBlue;
            this.buttonCopyText.Location = new System.Drawing.Point(0, 0);
            this.buttonCopyText.Name = "buttonCopyText";
            this.buttonCopyText.Size = new System.Drawing.Size(102, 30);
            this.buttonCopyText.TabIndex = 15;
            this.buttonCopyText.Text = "Copy Text";
            this.buttonCopyText.UseVisualStyleBackColor = true;
            this.buttonCopyText.Click += new System.EventHandler(this.buttonCopyText_Click);
            // 
            // buttonNewBatch
            // 
            this.buttonNewBatch.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonNewBatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNewBatch.ForeColor = System.Drawing.Color.MediumBlue;
            this.buttonNewBatch.Location = new System.Drawing.Point(317, 0);
            this.buttonNewBatch.Name = "buttonNewBatch";
            this.buttonNewBatch.Size = new System.Drawing.Size(145, 30);
            this.buttonNewBatch.TabIndex = 12;
            this.buttonNewBatch.Text = "Begin New Batch";
            this.buttonNewBatch.UseVisualStyleBackColor = true;
            this.buttonNewBatch.Click += new System.EventHandler(this.buttonNewBatch_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            this.serialPort1.PinChanged += new System.IO.Ports.SerialPinChangedEventHandler(this.serialPort1_PinChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 662);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MooresCloud Holiday Tester";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonBegin;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox comboBoxPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSerial;
        private System.Windows.Forms.Button buttonRePrint;
        private System.Windows.Forms.ListView listViewStatus;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView listViewLog;
        private System.Windows.Forms.ColumnHeader columnHeader0;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonAllocate;
        private System.Windows.Forms.Button buttonGetNext;
        private System.Windows.Forms.Button buttonNewBatch;
        private System.Windows.Forms.Button buttonCopyText;
        private System.Windows.Forms.Label labelStationNum;
        private System.Windows.Forms.ComboBox comboBoxStationNum;
    }
}

