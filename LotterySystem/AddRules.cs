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

            this.groupBox1.Click += new System.EventHandler(this.groupBox1_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {

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
    }
}
