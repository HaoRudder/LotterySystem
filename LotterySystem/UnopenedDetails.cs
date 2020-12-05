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
    public partial class UnopenedDetails : Form
    {
        string _table;
        string _name;
        bool _type;
        public static RecordDetails from3;
        public UnopenedDetails()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 传参赋值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <param name="type">true：连续未开，false：连续最高</param>
        public void GetStatisticsData(string table, string name, bool type)
        {
            _table = table;
            _name = name;
            _type = type;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        public void QueryList()
        {
            Text = $"{(_type ? "连续未开详情 " : "连续最高")} -{_name}";
            var begin = Convert.ToDateTime(this.begin.Text);
            var end = Convert.ToDateTime(this.end.Text);
            list.DataSource = null;
            totalList.DataSource = null;
            detailsList.DataSource = null;
            button1.Text = "查询中...";
            button1.Enabled = false;
            Task.Run(() =>
            {
                var dt = new DataTable();


                if (!_type)
                {
                    #region 最高连开


                    dt = BLL.Logic.GetHighestContinuousStatisticsData(_table, _name, begin, end);
                    if (dt == null)
                    {
                        MessageBox.Show("未获取数据，请重试");
                        Invoke(new MethodInvoker(delegate
                        {
                            button1.Text = "查询";
                            button1.Enabled = true;
                        }));
                        return;
                    }
                    Invoke(new MethodInvoker(delegate
                    {
                        list.DataSource = dt;
                        int width;
                        list.Columns[0].Width = 50;
                        if (dt.Rows.Count > 18)
                        {
                            width = list.Width - list.Columns[0].Width - 20;
                        }
                        else
                        {
                            width = list.Width - list.Columns[0].Width - 3;
                        }
                        for (int i = 1; i < list.Columns.Count; i++)
                        {
                            list.Columns[i].Width = width / (list.Columns.Count - 1);
                            list.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                            list.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }));
                    var total = new DataTable();
                    var dr = total.NewRow();
                    total.Columns.Add("最高连开数", typeof(string));
                    dr["最高连开数"] = "总连开期数";
                    var arr = dt.AsEnumerable().Select(x => x.Field<string>("连开期数")).ToArray();
                    foreach (string item in arr.Distinct().OrderBy(Convert.ToInt32))
                    {
                        total.Columns.Add(item, typeof(string));
                        dr[item] = arr.LongCount(x => x == item).ToString();
                    }
                    total.Rows.Add(dr);
                    Invoke(new MethodInvoker(delegate
                    {
                        totalList.DataSource = total;
                        var width = 0;
                        var height = 15;
                        if (totalList.Columns.Count >= 17)
                        {
                            totalList.Height = 63;
                            detailsList.Height = 393;
                            detailsList.Location = new Point(443, 64);
                            for (int i = 0; i < totalList.Columns.Count; i++)
                            {
                                totalList.Columns[0].Width = 82;
                                totalList.Columns[i].Width = 50;
                                totalList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                            }
                        }
                        else
                        {
                            if (totalList.Height != 48)
                            {
                                totalList.Height = totalList.Height - height;
                                detailsList.Height = detailsList.Height + height;
                                detailsList.Location = new Point(443, 64 - height);
                            }

                            totalList.Columns[0].Width = 80;
                            width = totalList.Width - totalList.Columns[0].Width;
                            for (int i = 1; i < totalList.Columns.Count; i++)
                            {
                                totalList.Columns[i].Width = width / (totalList.Columns.Count - 1) - 3;

                                totalList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                            }

                        }
                    }));
                    var details = new DataTable();

                    details.Columns.Add("连开数/时间", typeof(int));

                    var timeList = dt.AsEnumerable().Select(x => x.Field<string>("开始时间")).ToList().Select(x => x.Substring(0, x.IndexOf(" "))).Distinct();
                    var isHour = timeList.Count() == 1;

                    if (isHour)
                    {
                        timeList = new List<string>
                        {
                            timeList.ToList()[0]+" 1:00:00",
                            timeList.ToList()[0]+" 2:00:00",
                            timeList.ToList()[0]+" 3:00:00",
                            timeList.ToList()[0]+" 4:00:00",
                            timeList.ToList()[0]+" 5:00:00",
                            timeList.ToList()[0]+" 6:00:00",
                            timeList.ToList()[0]+" 7:00:00",
                            timeList.ToList()[0]+" 8:00:00",
                            timeList.ToList()[0]+" 9:00:00",
                            timeList.ToList()[0]+" 10:00:00",
                            timeList.ToList()[0]+" 11:00:00",
                            timeList.ToList()[0]+" 12:00:00",
                            timeList.ToList()[0]+" 13:00:00",
                            timeList.ToList()[0]+" 14:00:00",
                            timeList.ToList()[0]+" 15:00:00",
                            timeList.ToList()[0]+" 16:00:00",
                            timeList.ToList()[0]+" 17:00:00",
                            timeList.ToList()[0]+" 18:00:00",
                            timeList.ToList()[0]+" 19:00:00",
                            timeList.ToList()[0]+" 20:00:00",
                            timeList.ToList()[0]+" 21:00:00",
                            timeList.ToList()[0]+" 22:00:00",
                            timeList.ToList()[0]+" 23:00:00",
                            timeList.ToList()[0]+" 0:00:00"
                        };
                    }
                    foreach (var item in timeList)
                    {
                        var newDt = dt.Select($" 开始时间 like '%{item}%' and 结束时间 like '%{item}%'");
                        if (isHour)
                        {
                            newDt = dt.Select($" CONVERT(开始时间,System.DateTime) <= '{Convert.ToDateTime(item).ToString("yyyy/MM/dd HH:59:59")}' and CONVERT(结束时间,System.DateTime) >= '{Convert.ToDateTime(item).ToString("yyyy/MM/dd HH:00:00")}'");
                        }
                        var newArr = newDt.AsEnumerable().Select(x => x.Field<string>("连开期数")).ToArray();
                        details.Columns.Add(item, typeof(string));
                        foreach (string val in newArr.Distinct().OrderBy(Convert.ToInt32))
                        {
                            int j = -1;
                            for (int i = 0; i < details.Rows.Count; i++)
                            {
                                if (details.Rows[i]["连开数/时间"].ToString() == val)
                                {
                                    j = i;
                                    break;
                                }
                            }

                            var num = newArr.LongCount(x => x == val).ToString();

                            if (j == -1)
                            {
                                var newDr = details.NewRow();
                                newDr["连开数/时间"] = val;
                                newDr[item] = num;
                                details.Rows.Add(newDr);
                            }
                            else
                            {
                                details.Rows[j][item] = num;
                            }
                        }
                    }
                    details.DefaultView.Sort = "连开数/时间 ASC";
                    details = details.DefaultView.ToTable();
                    Invoke(new MethodInvoker(delegate
                    {
                        detailsList.DataSource = details;
                        detailsList.Columns[0].Frozen = true;
                        for (int i = 0; i < detailsList.Columns.Count; i++)
                        {
                            detailsList.Columns[0].Width = 80;
                            detailsList.Columns[i].Width = 120;
                            detailsList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }));
                    #endregion
                }
                else
                {
                    #region 连续未开
                    dt = BLL.Logic.GetStatisticsData(_table, _name, begin, end);
                    if (dt == null)
                    {
                        MessageBox.Show("未获取数据，请重试");
                        Invoke(new MethodInvoker(delegate
                        {
                            button1.Text = "查询";
                            button1.Enabled = true;
                        }));
                        return;
                    }
                    Invoke(new MethodInvoker(delegate
                    {
                        list.DataSource = dt;
                        int width;
                        list.Columns[0].Width = 50;
                        if (dt.Rows.Count > 18)
                        {
                            width = list.Width - list.Columns[0].Width - 20;
                        }
                        else
                        {
                            width = list.Width - list.Columns[0].Width - 3;
                        }
                        for (int i = 1; i < list.Columns.Count; i++)
                        {
                            list.Columns[i].Width = width / (list.Columns.Count - 1);
                            list.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                            list.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }));
                    var total = new DataTable();
                    var dr = total.NewRow();
                    total.Columns.Add("连续未开数", typeof(string));
                    dr["连续未开数"] = "总未开期数";
                    //1、添加列
                    var arr = dt.AsEnumerable().Select(x => x.Field<string>("未开期数")).ToArray();
                    foreach (string item in arr.Distinct().OrderBy(Convert.ToInt32))
                    {
                        total.Columns.Add(item, typeof(string));
                        dr[item] = arr.LongCount(x => x == item).ToString();
                    }
                    total.Rows.Add(dr);
                    Invoke(new MethodInvoker(delegate
                    {
                        totalList.DataSource = total;
                        var width = 0;
                        var height = 15;
                        if (totalList.Columns.Count >= 17)
                        {
                            totalList.Height = 63;
                            detailsList.Height = 393;
                            detailsList.Location = new Point(443, 64);
                            for (int i = 0; i < totalList.Columns.Count; i++)
                            {
                                totalList.Columns[0].Width = 82;
                                totalList.Columns[i].Width = 50;
                                totalList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                            }
                        }
                        else
                        {
                            if (totalList.Height != 48)
                            {
                                totalList.Height = totalList.Height - height;
                                detailsList.Height = detailsList.Height + height;
                                detailsList.Location = new Point(443, 64 - height);
                            }

                            totalList.Columns[0].Width = 80;
                            width = totalList.Width - totalList.Columns[0].Width;
                            for (int i = 1; i < totalList.Columns.Count; i++)
                            {
                                totalList.Columns[i].Width = width / (totalList.Columns.Count - 1) - 3;

                                totalList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                            }

                        }
                    }));
                    var details = new DataTable();

                    details.Columns.Add("未开数/时间", typeof(int));

                    var timeList = dt.AsEnumerable().Select(x => x.Field<string>("开始时间")).ToList().Select(x => x.Substring(0, x.IndexOf(" "))).Distinct();
                    var isHour = timeList.Count() == 1;

                    if (isHour)
                    {
                        timeList = new List<string>
                        {
                            timeList.ToList()[0]+" 1:00:00",
                            timeList.ToList()[0]+" 2:00:00",
                            timeList.ToList()[0]+" 3:00:00",
                            timeList.ToList()[0]+" 4:00:00",
                            timeList.ToList()[0]+" 5:00:00",
                            timeList.ToList()[0]+" 6:00:00",
                            timeList.ToList()[0]+" 7:00:00",
                            timeList.ToList()[0]+" 8:00:00",
                            timeList.ToList()[0]+" 9:00:00",
                            timeList.ToList()[0]+" 10:00:00",
                            timeList.ToList()[0]+" 11:00:00",
                            timeList.ToList()[0]+" 12:00:00",
                            timeList.ToList()[0]+" 13:00:00",
                            timeList.ToList()[0]+" 14:00:00",
                            timeList.ToList()[0]+" 15:00:00",
                            timeList.ToList()[0]+" 16:00:00",
                            timeList.ToList()[0]+" 17:00:00",
                            timeList.ToList()[0]+" 18:00:00",
                            timeList.ToList()[0]+" 19:00:00",
                            timeList.ToList()[0]+" 20:00:00",
                            timeList.ToList()[0]+" 21:00:00",
                            timeList.ToList()[0]+" 22:00:00",
                            timeList.ToList()[0]+" 23:00:00",
                            timeList.ToList()[0]+" 0:00:00"
                        };
                    }
                    foreach (var item in timeList)
                    {
                        var newDt = dt.Select($" 开始时间 like '%{item}%' and 结束时间 like '%{item}%'");
                        if (isHour)
                        {
                            newDt = dt.Select($" CONVERT(开始时间,System.DateTime) <= '{Convert.ToDateTime(item).ToString("yyyy/MM/dd HH:59:59")}' and CONVERT(结束时间,System.DateTime) >= '{Convert.ToDateTime(item).ToString("yyyy/MM/dd HH:00:00")}'");
                        }
                        var newArr = newDt.AsEnumerable().Select(x => x.Field<string>("未开期数")).ToArray();
                        details.Columns.Add(item, typeof(string));
                        foreach (string val in newArr.Distinct().OrderBy(Convert.ToInt32))
                        {
                            int j = -1;
                            for (int i = 0; i < details.Rows.Count; i++)
                            {
                                if (details.Rows[i]["未开数/时间"].ToString() == val)
                                {
                                    j = i;
                                    break;
                                }
                            }

                            var num = newArr.LongCount(x => x == val).ToString();

                            if (j == -1)
                            {
                                var newDr = details.NewRow();
                                newDr["未开数/时间"] = val;
                                newDr[item] = num;
                                details.Rows.Add(newDr);
                            }
                            else
                            {
                                details.Rows[j][item] = num;
                            }
                        }
                    }

                    Invoke(new MethodInvoker(delegate
                    {
                        details.DefaultView.Sort = "未开数/时间 ASC";
                        details = details.DefaultView.ToTable();

                        detailsList.DataSource = details;
                        detailsList.Columns[0].Frozen = true;
                        for (int i = 0; i < detailsList.Columns.Count; i++)
                        {
                            detailsList.Columns[0].Width = 80;
                            detailsList.Columns[i].Width = 120;
                            detailsList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }));
                    #endregion
                }

                Invoke(new MethodInvoker(delegate
                {
                    button1.Text = "查询";
                    button1.Enabled = true;
                }));
            });
        }

        private void UnopenedDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.from2 = null;
        }

        private void UnopenedDetails_Shown(object sender, EventArgs e)
        {
            QueryList();
        }

        /// <summary>
        /// 处理Dgv
        /// </summary>
        /// <param name="dropDown">是否有下拉框</param>
        public void HandleDgv(DataGridView list, bool dropDown, int colLength = 0)
        {
            if (list.Rows.Count <= 0)
            {
                return;
            }
            int width;
            list.Columns[0].Width = colLength;
            if (dropDown)
            {
                width = list.Width - list.Columns[0].Width - 20;
            }
            else
            {
                width = list.Width - list.Columns[0].Width - 3;
            }
            for (int i = 1; i < list.Columns.Count; i++)
            {
                list.Columns[i].Width = width / (list.Columns.Count - 1);
                list.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                list.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < list.Rows.Count; i++)
            {
                list.Rows[i].Cells[3].Style.BackColor = Color.FromArgb(214, 67, 10);
                list.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                list.Rows[i].Cells[3].Style.Font = new Font(list.Font, FontStyle.Bold);
            }

            var height = 15;
            if (totalList.Columns.Count >= 17)
            {
                totalList.Height = 63;
                detailsList.Height = 393;
                detailsList.Location = new Point(443, 64);
                for (int i = 0; i < totalList.Columns.Count; i++)
                {
                    totalList.Columns[0].Width = 82;
                    totalList.Columns[i].Width = 50;
                    totalList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            else
            {
                if (totalList.Height != 48)
                {
                    totalList.Height = totalList.Height - height;
                    detailsList.Height = detailsList.Height + height;
                    detailsList.Location = new Point(443, 64 - height);
                }

                totalList.Columns[0].Width = 80;
                width = totalList.Width - totalList.Columns[0].Width;
                for (int i = 1; i < totalList.Columns.Count; i++)
                {
                    totalList.Columns[i].Width = width / (totalList.Columns.Count - 1) - 3;

                    totalList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

            }

            detailsList.Columns[0].Frozen = true;
            for (int i = 0; i < detailsList.Columns.Count; i++)
            {
                detailsList.Columns[0].Width = 80;
                detailsList.Columns[i].Width = 120;
                detailsList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            QueryList();
        }

        private void UnopenedDetails_Load(object sender, EventArgs e)
        {
            //begin.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            //end.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            begin.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            end.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
        }

        private void list_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var num = Convert.ToInt32(list.Rows[e.RowIndex].Cells[3].Value);
            var begin = new DateTime();
            var end = new DateTime();
            if (!_type)
            {
                begin = Convert.ToDateTime(list.Rows[e.RowIndex].Cells[1].Value);
                end = Convert.ToDateTime(list.Rows[e.RowIndex].Cells[2].Value);
            }
            else
            {
                if (num > 0)
                {
                    begin = Convert.ToDateTime(list.Rows[e.RowIndex == 0 ? 0 : e.RowIndex - 1].Cells[1].Value);
                }
                else
                {
                    begin = Convert.ToDateTime(list.Rows[e.RowIndex].Cells[1].Value);
                }
                end = Convert.ToDateTime(list.Rows[e.RowIndex].Cells[2].Value);
            }

            if (from3 == null)
            {
                from3 = new RecordDetails();
                from3.Assignment(_table, _name, begin, end, _type, num > 0);
                from3.Show();
            }
            else
            {
                from3.Activate();
            }
        }
    }
}
