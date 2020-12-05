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

    public partial class RecordDetails : Form
    {
        DateTime _begin;
        DateTime _end;
        string _table;
        string _name;
        DataTable _Dt;
        bool _type;
        bool _isNum;
        public RecordDetails()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 传参赋值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        public void Assignment(string table, string name, DateTime begin, DateTime end,bool type,bool isNum)
        {
            _begin = begin;
            _end = end;
            _table = table;
            _name = name;
            _type = type;
            _isNum = isNum;
        }

        private void RecordDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnopenedDetails.from3 = null;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        public void QueryList()
        {
            var dt = BLL.Logic.GetRecordDetails(_table, _begin, _end,_type, _isNum);
            list.DataSource = dt;
            _Dt = dt;
            list.Columns[2].DefaultCellStyle.Format = "yyyy-MM-dd  HH:mm:ss";
            HandleDgv();
        }

        /// <summary>
        /// 页面加载完成后再加载列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordDetails_Shown(object sender, EventArgs e)
        {
            this.Text = $"详情记录 -{_name}";
            QueryList();
        }

        public void HandleDgv()
        {

            for (int i = 0; i < list.Rows.Count; i++)
            {
                list.Rows[i].Cells[4].Style.BackColor = Color.FromArgb(214, 67, 10);
                list.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                list.Rows[i].Cells[4].Style.Font = new Font(list.Font, FontStyle.Bold);

                list.Rows[i].Cells[5].Style.BackColor = Color.FromArgb(214, 67, 10);
                list.Rows[i].Cells[5].Style.ForeColor = Color.Yellow;
                list.Rows[i].Cells[5].Style.Font = new Font(list.Font, FontStyle.Bold); 
            }

            list.Columns[0].Width = 50;
            list.Columns[4].Width = 40;
            list.Columns[5].Width = 40;
            var width = list.Width - (list.Columns[0].Width + list.Columns[4].Width + list.Columns[5].Width) - 2;
            if (list.Rows.Count > 18)
            {
                width = width - 21;
            }

            list.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            for (int i = 1; i < list.ColumnCount; i++)
            {
                if (i < 4)
                {
                    list.Columns[i].Width = width/(list.ColumnCount-3);
                }
                list.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
    }
}
