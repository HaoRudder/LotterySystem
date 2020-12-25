using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LotterySystem
{
    public partial class AddRules : Form
    {
        public AddRules()
        {
            InitializeComponent();

            checkedListBox1.LostFocus += new EventHandler(checkedListBox1_LostFocus);
            textBox3.LostFocus += new EventHandler(checkedListBox1_LostFocus);

            checkedListBox2.LostFocus += new EventHandler(checkedListBox2_LostFocus);
            textBox9.LostFocus += new EventHandler(checkedListBox2_LostFocus);

            checkedListBox3.LostFocus += new EventHandler(checkedListBox3_LostFocus);
            textBox10.LostFocus += new EventHandler(checkedListBox3_LostFocus);

            this.groupBox1.Click += new System.EventHandler(this.groupBox1_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var isPasse = Verification();
            if (!isPasse)
            {
                return;
            }

            var id = GetID();
            if (id == -1)
            {
                MessageBox.Show("请选中需要操作的规则");
                return;
            }
            AddOrUpdate(id);
        }

        public void AddOrUpdate(int id = 0)
        {
            var result = new BLL.RulesBusiness().AddRules(new BLL.Ruleinfo
            {
                ID = id,
                OpenContent = textBox3.Text,
                BetContent = textBox9.Text,
                JudgeCondition = comboBox2.Text,
                JudgeNumber = Convert.ToInt32(textBox1.Text),
                NoWinBetNumber = Convert.ToInt32(string.IsNullOrWhiteSpace(textBox2.Text) ? "0" : textBox2.Text),
                NoWinBetConent = textBox10.Text,
                StopProfit = Convert.ToInt32(string.IsNullOrWhiteSpace(textBox6.Text) ? "0" : textBox6.Text),
                StopLoss = Convert.ToInt32(string.IsNullOrWhiteSpace(textBox8.Text) ? "0" : textBox8.Text),
                intervalBetHours = Convert.ToInt32(string.IsNullOrWhiteSpace(textBox7.Text) ? "0" : textBox7.Text),
                OddsID = comboBox6.Text == "赔率1" ? 1 : 2,
                IsTurnBet = checkBox2.CheckState == CheckState.Checked ? 1 : 0,
                ProfitMultiple = textBox4.Text,
                LossMultiple = textBox5.Text,
                IsLossBetNow = checkBox3.CheckState == CheckState.Checked ? 1 : 0,
                IsProfitBetNow = checkBox6.CheckState == CheckState.Checked ? 1 : 0,
                BetGearStop = checkBox7.CheckState == CheckState.Checked ? 1 : 0,
                //CrackAfterBet = checkBox1.CheckState == CheckState.Checked ? 1 : 0,
                RuleType = Convert.ToInt32(comboBox1.Text.Substring(0, 1)),
            });

            if (result)
            {
                MessageBox.Show("操作成功");
            }
            else
            {
                MessageBox.Show("操作失败");
            }
            GetRuleinfo();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var id = GetID();
            if (id == -1)
            {
                MessageBox.Show("请选中需要操作的规则");
                return;
            }

            var r = MessageBox.Show("是否确认输出此规则", "警告信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (r == DialogResult.OK)
            {
                var result = new BLL.RulesBusiness().DelRuleInfo(id);

                if (result)
                {
                    MessageBox.Show("删除成功");
                    GetRuleinfo();
                }
                else
                {
                    MessageBox.Show("删除失败");
                }
            }
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            var name = (sender as TextBox).Name;
            if (name == "textBox3")
            {
                checkedListBox1.Visible = true;
            }
            else if (name == "textBox9")
            {
                checkedListBox2.Visible = true;
            }
            else if (name == "textBox10")
            {
                checkedListBox3.Visible = true;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var str = string.Empty;
            foreach (var item in checkedListBox1.CheckedItems)
            {
                str += item + "|";
            }
            textBox3.Text = !string.IsNullOrWhiteSpace(str) ? str.Substring(0, str.Length - 1) : "";
        }

        private void AddRules_Load(object sender, EventArgs e)
        {
            //this.Controls.index(this.list, 1);
            list.SendToBack();
            label2.SendToBack();
            label5.SendToBack();
            label6.SendToBack();
            GetRuleinfo();

        }

        private void DisplayHScroll()
        {
            list.IntegralHeight = true;
            var g = list.CreateGraphics();

            var maxLength = 0;
            for (int i = 0; i < list.Items.Count; i++)
            {
                var row = list.Items[i];
                int hzSize = (int)g.MeasureString(row.ToString(), list.Font).Width;
                if (hzSize > maxLength)
                {
                    maxLength = hzSize;
                }
            }
            list.HorizontalExtent = maxLength;
        }

        public void GetRuleinfo()
        {
            var dt = new BLL.RulesBusiness().GetRuleinfoList();

            var list = new List<string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var str = string.Empty;
                var row = dt.Rows[i];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    var colNmae = dt.Columns[j].ColumnName;
                    var col = colNmae == "OddsID" ? row[colNmae] == "1" ? "赔率1" : "赔率2" : row[colNmae];
                    str += col + ",";
                }
                str = str.Substring(0, str.Length - 1);
                list.Add(str);
            }
            this.list.DataSource = list;

            DisplayHScroll();
        }

        private void checkedListBox1_LostFocus(object sender, EventArgs e)
        {
            var control = GetFocusedControl();

            if (control == null)
            {
                return;
            }
            var name = control.Name;
            if (name != "textBox3" && name != "checkedListBox1")
            {
                checkedListBox1.Visible = false;
            }
        }

        private void checkedListBox2_LostFocus(object sender, EventArgs e)
        {
            var control = GetFocusedControl();

            if (control == null)
            {
                return;
            }
            var name = control.Name;
            if (name != "textBox9" && name != "checkedListBox2")
            {
                checkedListBox2.Visible = false;
            }
        }

        private void checkedListBox3_LostFocus(object sender, EventArgs e)
        {
            var control = GetFocusedControl();

            if (control == null)
            {
                return;
            }
            var name = control.Name;
            if (name != "textBox10" && name != "checkedListBox3")
            {
                checkedListBox3.Visible = false;
            }
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();
        ///获取 当前拥有焦点的控件
        private Control GetFocusedControl()
        {
            Control focusedControl = null;
            IntPtr focusedHandle = GetFocus();
            if (focusedHandle != IntPtr.Zero)
                focusedControl = Control.FromChildHandle(focusedHandle);
            return focusedControl;
        }

        private void AddRules_Click(object sender, EventArgs e)
        {
            checkedListBox1.Visible = false;
        }
        private void groupBox1_Click(object sender, EventArgs e)
        {
            checkedListBox1.Visible = false;
            groupBox1.Focus();
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var str = string.Empty;
            foreach (var item in checkedListBox2.CheckedItems)
            {
                str += item + "|";
            }
            textBox9.Text = !string.IsNullOrWhiteSpace(str) ? str.Substring(0, str.Length - 1) : "";
        }

        private void AddRules_FormClosing(object sender, FormClosingEventArgs e)
        {
            NewAnalogData.from7 = null;
        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var str = string.Empty;
            foreach (var item in checkedListBox3.CheckedItems)
            {
                str += item + "|";
            }
            textBox10.Text = !string.IsNullOrWhiteSpace(str) ? str.Substring(0, str.Length - 1) : "";
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            var id = GetID();
            var data = GetRuleinfo(id);

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
            for (int i = 0; i < data.OpenContent.Split('|').Length; i++)
            {
                var val = data.OpenContent.Split('|')[i];
                var index = checkedListBox1.Items.IndexOf(val);
                checkedListBox1.SetItemChecked(index, true);
            }
            textBox3.Text = data.OpenContent;


            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                if (checkedListBox2.GetItemChecked(i))
                {
                    checkedListBox2.SetItemChecked(i, false);
                }
            }
            for (int i = 0; i < data.BetContent.Split('|').Length; i++)
            {
                var val = data.BetContent.Split('|')[i];
                var index = checkedListBox2.Items.IndexOf(val);
                checkedListBox2.SetItemChecked(index, true);
            }
            textBox9.Text = data.BetContent;

            comboBox2.Text = data.JudgeCondition;
            textBox1.Text = data.JudgeNumber.ToString();
            textBox2.Text = data.NoWinBetNumber.ToString();


            for (int i = 0; i < checkedListBox3.Items.Count; i++)
            {
                if (checkedListBox3.GetItemChecked(i))
                {
                    checkedListBox3.SetItemChecked(i, false);
                }
            }
            for (int i = 0; i < data.NoWinBetConent.Split('|').Length; i++)
            {
                var val = data.NoWinBetConent.Split('|')[i];
                if (string.IsNullOrWhiteSpace(val))
                {
                    continue;
                }
                var index = checkedListBox3.Items.IndexOf(val);
                checkedListBox3.SetItemChecked(index, true);
            }
            textBox10.Text = data.NoWinBetConent;

            textBox6.Text = data.StopProfit.ToString();
            textBox8.Text = data.StopLoss.ToString();
            textBox7.Text = data.intervalBetHours.ToString();
            comboBox6.Text = data.OddsID == 1 ? "赔率1" : "赔率2";
            checkBox2.CheckState = data.IsTurnBet == 1 ? CheckState.Checked : CheckState.Unchecked;
            textBox4.Text = data.ProfitMultiple;
            textBox5.Text = data.LossMultiple;
            checkBox3.CheckState = data.IsLossBetNow == 1 ? CheckState.Checked : CheckState.Unchecked;
            checkBox6.CheckState = data.IsProfitBetNow == 1 ? CheckState.Checked : CheckState.Unchecked;
            checkBox7.CheckState = data.BetGearStop == 1 ? CheckState.Checked : CheckState.Unchecked;
            comboBox1.SelectedIndex = data.RuleType - 1;
        }

        public BLL.Ruleinfo GetRuleinfo(int id)
        {
            return new BLL.RulesBusiness().GetRuleinfo(id);
        }

        public int GetID()
        {
            if (list.SelectedItem == null)
            {
                return -1;
            }
            var str = list.SelectedItem.ToString();
            var id = Convert.ToInt32(str.Split(',')[0]);
            return id;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var isPasse = Verification();
            if (!isPasse)
            {
                return;
            }
            AddOrUpdate();
        }

        public bool Verification()
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("开奖内容不能为空");
                textBox3.Focus();
                textBox3_Click(textBox3, null);
                return false;
            }
            if (string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                MessageBox.Show("投注条件不能为空");
                comboBox2.Focus();
                comboBox2.DroppedDown = true;
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("判断期数不能为空");
                textBox1.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox9.Text))
            {
                MessageBox.Show("投注内容不能为空");
                textBox3_Click(textBox9, null);
                textBox9.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("规则类型不能为空");
                comboBox1.Focus();
                comboBox1.DroppedDown = true;
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("盈利倍投不能为空,至少含有一个档位");
                textBox4.Focus();
                return false;
            }
            return true;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            var name = ((CheckBox)sender).Name;
            if (name == "checkBox3")
            {
                if (checkBox3.CheckState == CheckState.Checked)
                {
                    checkBox6.CheckState = CheckState.Unchecked;
                }
            }
            else
            {
                if (checkBox6.CheckState == CheckState.Checked)
                {
                    checkBox3.CheckState = CheckState.Unchecked;
                }
            }
        }
    }
}
