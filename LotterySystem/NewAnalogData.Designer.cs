namespace LotterySystem
{
    partial class NewAnalogData
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.list = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.刷新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox11 = new System.Windows.Forms.ComboBox();
            this.button7 = new System.Windows.Forms.Button();
            this.comboBox10 = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.end = new System.Windows.Forms.DateTimePicker();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.begin = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.赔率设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加规则ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.list)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.list);
            this.groupBox1.Location = new System.Drawing.Point(12, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(672, 360);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "自动投注规则";
            // 
            // list
            // 
            this.list.AllowUserToAddRows = false;
            this.list.AllowUserToDeleteRows = false;
            this.list.AllowUserToResizeColumns = false;
            this.list.AllowUserToResizeRows = false;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.list.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.list.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.list.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.list.DefaultCellStyle = dataGridViewCellStyle14;
            this.list.Location = new System.Drawing.Point(6, 20);
            this.list.Name = "list";
            this.list.ReadOnly = true;
            this.list.RowHeadersVisible = false;
            this.list.RowHeadersWidth = 20;
            this.list.RowTemplate.Height = 23;
            this.list.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.list.Size = new System.Drawing.Size(660, 334);
            this.list.TabIndex = 15;
            this.list.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.list_CellMouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.刷新ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // 刷新ToolStripMenuItem
            // 
            this.刷新ToolStripMenuItem.Name = "刷新ToolStripMenuItem";
            this.刷新ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.刷新ToolStripMenuItem.Text = "刷新";
            this.刷新ToolStripMenuItem.Click += new System.EventHandler(this.刷新ToolStripMenuItem_Click);
            // 
            // textBox9
            // 
            this.textBox9.BackColor = System.Drawing.Color.Black;
            this.textBox9.ForeColor = System.Drawing.Color.Green;
            this.textBox9.Location = new System.Drawing.Point(12, 394);
            this.textBox9.Multiline = true;
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox9.ShortcutsEnabled = false;
            this.textBox9.Size = new System.Drawing.Size(672, 280);
            this.textBox9.TabIndex = 42;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.comboBox11);
            this.groupBox2.Controls.Add(this.button7);
            this.groupBox2.Controls.Add(this.comboBox10);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Controls.Add(this.end);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.begin);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(695, 28);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(929, 646);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "模拟运算投注";
            // 
            // comboBox11
            // 
            this.comboBox11.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox11.FormattingEnabled = true;
            this.comboBox11.Items.AddRange(new object[] {
            "南宫"});
            this.comboBox11.Location = new System.Drawing.Point(645, 620);
            this.comboBox11.Name = "comboBox11";
            this.comboBox11.Size = new System.Drawing.Size(98, 20);
            this.comboBox11.TabIndex = 51;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(749, 617);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(86, 23);
            this.button7.TabIndex = 50;
            this.button7.Text = "查询最新余额";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // comboBox10
            // 
            this.comboBox10.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox10.FormattingEnabled = true;
            this.comboBox10.Items.AddRange(new object[] {
            "模拟投注",
            "真实投注"});
            this.comboBox10.Location = new System.Drawing.Point(719, 21);
            this.comboBox10.Name = "comboBox10";
            this.comboBox10.Size = new System.Drawing.Size(80, 20);
            this.comboBox10.TabIndex = 48;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(886, 21);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(38, 23);
            this.button4.TabIndex = 19;
            this.button4.Text = "跳转";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(6, 619);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(633, 21);
            this.textBox3.TabIndex = 17;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridView1.Location = new System.Drawing.Point(8, 49);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 20;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(916, 562);
            this.dataGridView1.TabIndex = 17;
            // 
            // end
            // 
            this.end.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.end.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.end.Location = new System.Drawing.Point(292, 20);
            this.end.Name = "end";
            this.end.Size = new System.Drawing.Size(197, 21);
            this.end.TabIndex = 18;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(805, 20);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "开始";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(269, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "至";
            // 
            // begin
            // 
            this.begin.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.begin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.begin.Location = new System.Drawing.Point(66, 19);
            this.begin.Name = "begin";
            this.begin.Size = new System.Drawing.Size(197, 21);
            this.begin.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "查询时间";
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.赔率设置ToolStripMenuItem,
            this.添加规则ToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(1636, 25);
            this.menuStrip2.TabIndex = 44;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // 赔率设置ToolStripMenuItem
            // 
            this.赔率设置ToolStripMenuItem.Name = "赔率设置ToolStripMenuItem";
            this.赔率设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.赔率设置ToolStripMenuItem.Text = "赔率设置";
            this.赔率设置ToolStripMenuItem.Click += new System.EventHandler(this.赔率设置ToolStripMenuItem_Click_1);
            // 
            // 添加规则ToolStripMenuItem
            // 
            this.添加规则ToolStripMenuItem.Name = "添加规则ToolStripMenuItem";
            this.添加规则ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.添加规则ToolStripMenuItem.Text = "添加规则";
            this.添加规则ToolStripMenuItem.Click += new System.EventHandler(this.添加规则ToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(496, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(49, 23);
            this.button1.TabIndex = 52;
            this.button1.Text = "上一天";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(606, 21);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(49, 23);
            this.button3.TabIndex = 53;
            this.button3.Text = "上一月";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(551, 21);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(49, 23);
            this.button5.TabIndex = 54;
            this.button5.Text = "下一天";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(661, 20);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(49, 23);
            this.button6.TabIndex = 55;
            this.button6.Text = "下一月";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // NewAnalogData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1636, 686);
            this.Controls.Add(this.menuStrip2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.textBox9);
            this.Controls.Add(this.groupBox1);
            this.Name = "NewAnalogData";
            this.Text = "NewAnalogData";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewAnalogData_FormClosing);
            this.Load += new System.EventHandler(this.NewAnalogData_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.list)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView list;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBox11;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox comboBox10;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DateTimePicker end;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker begin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 赔率设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加规则ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 刷新ToolStripMenuItem;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}