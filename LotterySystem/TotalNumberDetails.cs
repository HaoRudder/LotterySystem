using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LotterySystem
{
    public partial class TotalNumberDetails : Form
    {
        string _table;
        string _name;
        bool _type;
        public TotalNumberDetails()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 传参赋值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <param name="_type">true：代表查询常用总数，false：为专查特码类型</param>
        public void GetStatisticsData(string table, bool type)
        {
            _table = table;
            _type = type;
        }

        public void QueryList()
        {
            var btime = Convert.ToDateTime(begin.Text);
            var etime = Convert.ToDateTime(end.Text);

            var byTimeType = comboBox1.Text;
            var groupBy = string.Empty;
            switch (byTimeType)
            {
                case "按小时查询": groupBy = " date_format(time,'%Y-%m-%d %H')"; begin.CustomFormat = "yyyy-MM-dd HH:00:00"; end.CustomFormat = "yyyy-MM-dd HH:59:59"; break;
                case "按当天查询": groupBy = " date_format(time,'%Y-%m-%d')"; begin.CustomFormat = "yyyy-MM-dd"; end.CustomFormat = "yyyy-MM-dd"; btime = Convert.ToDateTime(btime.ToString("yyyy-MM-dd 00:00:00")); etime = Convert.ToDateTime(etime.ToString("yyyy-MM-dd 23:59:59")); break;
                case "按当月查询": groupBy = " date_format(time,'%Y-%m')"; begin.CustomFormat = "yyyy-MM"; end.CustomFormat = "yyyy-MM"; btime = Convert.ToDateTime(btime.ToString("yyyy-MM-01")); etime = Convert.ToDateTime(etime.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:59:59"));  break;
            }
            button1.Text = "查询中...";
            button1.Enabled = false;
            Task.Run(() =>
            {
                var dt = BLL.Logic.GetTotalNumberDetails(_table, Convert.ToDateTime(btime), Convert.ToDateTime(etime), _type, groupBy);
                Invoke(new MethodInvoker(delegate
                {
                    list.DataSource = dt;
                    button1.Text = "查询";
                    button1.Enabled = true;

                    if (dt == null)
                    {
                        MessageBox.Show(@"未获取到数据，请重试");
                        return;
                    }
                    //HandleDgv();
                    HandleDataGridView(list);
                }));
            });
        }

        private void TotalNumberDetails_Shown(object sender, EventArgs e)
        {
            begin.Text = DateTime.Now.ToString("yyyy-MM-dd");
            end.Text = DateTime.Now.ToString("yyyy-MM-dd");
            Text = $"总开奖次数报表";
            comboBox1.SelectedIndex = 1;
            QueryList();
        }

        private void TotalNumberDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.from3 = null;
        }

        public void HandleDataGridView(DataGridView item)
        {
            for (int j = 0; j < 2; j++)
            {
                item.Columns[0].Frozen = false;//固定列

                var width = 0;
                for (int i = 0; i < item.Columns.Count; i++)
                {
                    //将每一列都调整为自动适应模式
                    item.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                    //记录整个DataGridView的宽度
                    width += item.Columns[i].Width;
                }
                item.AutoSizeColumnsMode = width > item.Size.Width ? DataGridViewAutoSizeColumnsMode.DisplayedCells : DataGridViewAutoSizeColumnsMode.Fill;

                item.Columns[0].Frozen = true;//固定列
            }
        }

        public void HandleDgv()
        {
            if (list.Rows.Count > 22)
            {
                list.Columns[0].Width = 87;
                list.Columns[1].Width = 45;
                list.Columns[2].Width = 45;
                list.Columns[3].Width = 45;
                list.Columns[4].Width = 45;
            }
            else
            {
                list.Columns[0].Width = 84;
                list.Columns[1].Width = 50;
                list.Columns[2].Width = 50;
                list.Columns[3].Width = 50;
                list.Columns[4].Width = 50;
            }
            list.Columns[5].Width = 50;
            list.Columns[6].Width = 50;
            list.Columns[7].Width = 50;
            list.Columns[8].Width = 50;
            list.Columns[9].Width = 60;
            list.Columns[10].Width = 60;
            list.Columns[11].Width = 60;
            list.Columns[12].Width = 60;
            list.Columns[13].Width = 65;
            list.Columns[14].Width = 65;
            list.Columns[15].Width = 65;
            list.Columns[16].Width = 65;
            list.Columns[17].Width = 65;
            list.Columns[18].Width = 63;
            list.Columns[19].Width = 63;
            list.Columns[20].Width = 75;
            list.Columns[21].Width = 63;
            list.Columns[22].Width = 63;
            list.Columns[23].Width = 63;


            for (int i = 0; i < list.ColumnCount; i++)
            {
                list.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QueryList();
        }
    }
}
