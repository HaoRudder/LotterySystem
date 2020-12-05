namespace GetTheLatestData
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.testDBName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.testDbPwd = new System.Windows.Forms.TextBox();
            this.testDbAcc = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.update = new System.Windows.Forms.Button();
            this.updateInterval = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.updateBytime = new System.Windows.Forms.Button();
            this.end = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.begin = new System.Windows.Forms.DateTimePicker();
            this.tableList = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dbPwd = new System.Windows.Forms.TextBox();
            this.dbAcc = new System.Windows.Forms.TextBox();
            this.dbName = new System.Windows.Forms.TextBox();
            this.dbAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Black;
            this.textBox2.ForeColor = System.Drawing.Color.Green;
            this.textBox2.Location = new System.Drawing.Point(-2, -2);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox2.ShortcutsEnabled = false;
            this.textBox2.Size = new System.Drawing.Size(839, 514);
            this.textBox2.TabIndex = 7;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.testDBName);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.testDbPwd);
            this.groupBox1.Controls.Add(this.testDbAcc);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.update);
            this.groupBox1.Controls.Add(this.updateInterval);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.updateBytime);
            this.groupBox1.Controls.Add(this.end);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.begin);
            this.groupBox1.Controls.Add(this.tableList);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dbPwd);
            this.groupBox1.Controls.Add(this.dbAcc);
            this.groupBox1.Controls.Add(this.dbName);
            this.groupBox1.Controls.Add(this.dbAddress);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(843, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 507);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "系统配置";
            // 
            // testDBName
            // 
            this.testDBName.Location = new System.Drawing.Point(93, 137);
            this.testDBName.Name = "testDBName";
            this.testDBName.Size = new System.Drawing.Size(162, 21);
            this.testDBName.TabIndex = 35;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 140);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 12);
            this.label14.TabIndex = 34;
            this.label14.Text = "本地数据库名";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "加拿大",
            "北京"});
            this.comboBox1.Location = new System.Drawing.Point(193, 404);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(63, 20);
            this.comboBox1.TabIndex = 33;
            this.comboBox1.MouseEnter += new System.EventHandler(this.comboBox1_MouseEnter);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(102, 402);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(89, 23);
            this.button3.TabIndex = 32;
            this.button3.Text = "查询截断期号";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 426);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(245, 12);
            this.label13.TabIndex = 31;
            this.label13.Text = "————————————————————";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 402);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 23);
            this.button2.TabIndex = 30;
            this.button2.Text = "查询最新期号";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 390);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(245, 12);
            this.label12.TabIndex = 29;
            this.label12.Text = "————————————————————";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 310);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(245, 12);
            this.label11.TabIndex = 28;
            this.label11.Text = "————————————————————";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 119);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(245, 12);
            this.label10.TabIndex = 27;
            this.label10.Text = "————————————————————";
            // 
            // testDbPwd
            // 
            this.testDbPwd.Location = new System.Drawing.Point(93, 190);
            this.testDbPwd.Name = "testDbPwd";
            this.testDbPwd.Size = new System.Drawing.Size(162, 21);
            this.testDbPwd.TabIndex = 26;
            // 
            // testDbAcc
            // 
            this.testDbAcc.Location = new System.Drawing.Point(93, 163);
            this.testDbAcc.Name = "testDbAcc";
            this.testDbAcc.Size = new System.Drawing.Size(162, 21);
            this.testDbAcc.TabIndex = 25;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 193);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 22;
            this.label8.Text = "本地数据库密码";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 21;
            this.label9.Text = "本地数据库账号";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(95, 285);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "保存以上设置";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // update
            // 
            this.update.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.update.Location = new System.Drawing.Point(93, 441);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(111, 61);
            this.update.TabIndex = 17;
            this.update.Text = "开始更新";
            this.update.UseVisualStyleBackColor = true;
            this.update.Click += new System.EventHandler(this.update_Click);
            // 
            // updateInterval
            // 
            this.updateInterval.Location = new System.Drawing.Point(77, 217);
            this.updateInterval.Name = "updateInterval";
            this.updateInterval.Size = new System.Drawing.Size(178, 21);
            this.updateInterval.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 222);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "更新间隔(秒)";
            // 
            // updateBytime
            // 
            this.updateBytime.Location = new System.Drawing.Point(93, 364);
            this.updateBytime.Name = "updateBytime";
            this.updateBytime.Size = new System.Drawing.Size(111, 23);
            this.updateBytime.TabIndex = 14;
            this.updateBytime.Text = "开始按时间更新";
            this.updateBytime.UseVisualStyleBackColor = true;
            this.updateBytime.Click += new System.EventHandler(this.updateBytime_Click);
            // 
            // end
            // 
            this.end.CustomFormat = "yyyy-MM-dd";
            this.end.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.end.Location = new System.Drawing.Point(141, 337);
            this.end.Name = "end";
            this.end.Size = new System.Drawing.Size(114, 21);
            this.end.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(75, 322);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "按指定日期更新数据";
            // 
            // begin
            // 
            this.begin.CustomFormat = "yyyy-MM-dd";
            this.begin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.begin.Location = new System.Drawing.Point(8, 337);
            this.begin.Name = "begin";
            this.begin.Size = new System.Drawing.Size(118, 21);
            this.begin.TabIndex = 10;
            // 
            // tableList
            // 
            this.tableList.Location = new System.Drawing.Point(6, 258);
            this.tableList.Name = "tableList";
            this.tableList.Size = new System.Drawing.Size(247, 21);
            this.tableList.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 241);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "需要跑数据的表集合(以英文逗号隔开)";
            // 
            // dbPwd
            // 
            this.dbPwd.Location = new System.Drawing.Point(77, 95);
            this.dbPwd.Name = "dbPwd";
            this.dbPwd.Size = new System.Drawing.Size(178, 21);
            this.dbPwd.TabIndex = 7;
            // 
            // dbAcc
            // 
            this.dbAcc.Location = new System.Drawing.Point(77, 68);
            this.dbAcc.Name = "dbAcc";
            this.dbAcc.Size = new System.Drawing.Size(178, 21);
            this.dbAcc.TabIndex = 6;
            // 
            // dbName
            // 
            this.dbName.Location = new System.Drawing.Point(77, 41);
            this.dbName.Name = "dbName";
            this.dbName.Size = new System.Drawing.Size(178, 21);
            this.dbName.TabIndex = 5;
            // 
            // dbAddress
            // 
            this.dbAddress.Location = new System.Drawing.Point(77, 14);
            this.dbAddress.Name = "dbAddress";
            this.dbAddress.Size = new System.Drawing.Size(178, 21);
            this.dbAddress.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "数据库密码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "数据库账号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "数据库名称";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器地址";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "数据采集器";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.退出ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1116, 524);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数据采集器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button update;
        private System.Windows.Forms.TextBox updateInterval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button updateBytime;
        private System.Windows.Forms.DateTimePicker end;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker begin;
        private System.Windows.Forms.TextBox tableList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox dbPwd;
        private System.Windows.Forms.TextBox dbAcc;
        private System.Windows.Forms.TextBox dbName;
        private System.Windows.Forms.TextBox dbAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox testDbPwd;
        private System.Windows.Forms.TextBox testDbAcc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox testDBName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

