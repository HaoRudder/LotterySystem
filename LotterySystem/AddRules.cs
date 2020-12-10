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
            var result = new BLL.RulesBusiness().AddRules(new BLL.Ruleinfo
            {
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
                BetGearStop = checkBox1.CheckState == CheckState.Checked ? 1 : 0,
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

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
                str += item + ",";
            }
            textBox3.Text = !string.IsNullOrWhiteSpace(str) ? str.Substring(0, str.Length - 1) : "";
        }

        private void AddRules_Load(object sender, EventArgs e)
        {
            var dt = GetRuleinfo();

            var list = new List<string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var str = string.Empty;
                var row = dt.Rows[i];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    var col = row[dt.Columns[j].ColumnName];
                    str += col + ",";
                }
                str = str.Substring(0, str.Length - 1);
                list.Add(str);
            }
            this.list.DataSource = list;
        }

        public DataTable GetRuleinfo()
        {
            return new BLL.RulesBusiness().GetRuleinfoList();
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
                str += item + ",";
            }
            textBox9.Text = !string.IsNullOrWhiteSpace(str) ? str.Substring(0, str.Length - 1) : "";
        }

        private void AddRules_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnalogData.from7 = null;
            ManualSimulation.from7 = null;
        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var str = string.Empty;
            foreach (var item in checkedListBox3.CheckedItems)
            {
                str += item + ",";
            }
            textBox10.Text = !string.IsNullOrWhiteSpace(str) ? str.Substring(0, str.Length - 1) : "";
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            var str = ((ListBox)sender).SelectedItem.ToString();
            var id = str.Split(',')[0];
            var data = GetRuleinfo(Convert.ToInt32(id));

            textBox3.Text = data.OpenContent;
            textBox9.Text = data.BetContent;
            comboBox2.Text = data.JudgeCondition;
            textBox1.Text = data.JudgeNumber.ToString();
            textBox2.Text = data.NoWinBetNumber.ToString();
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
            checkBox1.CheckState = data.BetGearStop == 1 ? CheckState.Checked : CheckState.Unchecked;
        }

        public BLL.Ruleinfo GetRuleinfo(int id)
        {
            return new BLL.RulesBusiness().GetRuleinfo(id);
        }
    }
}
