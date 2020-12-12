using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;

namespace LotterySystem
{
    public partial class OddsSetting : Form
    {
        public OddsSetting()
        {
            InitializeComponent();
        }

        private static int _Type = 0;
        private void OddsSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnalogData.from6 = null;
            ManualSimulation.from6 = null;
            NewAnalogData.from6 = null;
        }

        /// <summary>
        /// 传递赋值
        /// </summary>
        /// <param name="type"></param>
        public void Assignment(int type)
        {
            _Type = type;
        }

        private void OddsSetting_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //var zxc = BLL.OddsBusiness.GetOddssInfo();

            //zxc[0].baozitongsha = true;
            //var sd = BLL.OddsBusiness.ModifyOddssInfo(zxc[0]);

            var str = string.Empty;

            var baozitongsha = this.baozitongsha.CheckState;
            var baozihuiben = this.baozihuiben.CheckState;
            var duizihuiben = this.duizihuiben.CheckState;
            var shunzihuiben = this.shunzihuiben.CheckState;
            var linjiuhuiben = this.linjiuhuiben.CheckState;
            var dadan = this.dadan.Text;
            var xiaodan = this.xiaodan.Text;
            var dashuang = this.dashuang.Text;
            var xiaoshuang = this.xiaoshuang.Text;
            var duizi = this.duizi.Text;
            var shunzi = this.shunzi.Text;
            var baozi = this.baozi.Text;
            var jishu = this.jishu.Text;
            var da = this.da.Text;
            var xiao = this.xiao.Text;
            var dan = this.dan.Text;
            var shuang = this.shuang.Text;
            var tema0 = this.tema0.Text;
            var tema1 = this.tema1.Text;
            var tema2 = this.tema2.Text;
            var tema3 = this.tema3.Text;
            var tema4 = this.tema4.Text;
            var tema5 = this.tema5.Text;
            var tema6 = this.tema6.Text;
            var tema7 = this.tema7.Text;
            var tema8 = this.tema8.Text;
            var tema9 = this.tema9.Text;
            var tema10 = this.tema10.Text;
            var tema11 = this.tema11.Text;
            var tema12 = this.tema12.Text;
            var tema13 = this.tema13.Text;
            var tema14 = this.tema14.Text;
            var tema15 = this.tema15.Text;
            var tema16 = this.tema16.Text;
            var tema17 = this.tema17.Text;
            var tema18 = this.tema18.Text;
            var tema19 = this.tema19.Text;
            var tema20 = this.tema20.Text;
            var tema21 = this.tema21.Text;
            var tema22 = this.tema22.Text;
            var tema23 = this.tema23.Text;
            var tema24 = this.tema24.Text;
            var tema25 = this.tema25.Text;
            var tema26 = this.tema26.Text;
            var tema27 = this.tema27.Text;
            var topzuhe = this.topzuhe.Text;
            var topsixiang = this.topsixiang.Text;
            var toptema = this.toptema.Text;
            var topduizi = this.topduizi.Text;
            var downzuhe = this.downzuhe.Text;
            var downsixiang = this.downsixiang.Text;
            var downtema = this.downtema.Text;
            var downduizi = this.downduizi.Text;
            var fenshu = this.fenshu.Text;
            var dadanxiaoshuang = this.dadanxiaoshuang.Text;
            var xiaodandashuang = this.xiaodandashuang.Text;
            var duishunbao1314 = this.duishunbao1314.Text;

            if (_Type == 1)
            {
                str = Tool.Helper.ReadOddsSettings("", "", _Type);

                var list = str.Split('\n').ToList();
                list.RemoveAll(x => x.Contains(comboBox1.Text));
                var content = $"{comboBox1.Text}-baozitongsha:{baozitongsha},baozihuiben:{baozihuiben },duizihuiben:{duizihuiben},shunzihuiben:{shunzihuiben},linjiuhuiben:{linjiuhuiben},dadan:{dadan},xiaodan:{xiaodan},dashuang:{dashuang},xiaoshuang:{xiaoshuang},duizi:{duizi},shunzi:{shunzi},baozi:{baozi}," +
                              $"jishu:{jishu},da:{da},xiao:{xiao},dan:{dan},shuang:{shuang},tema0:{tema0},tema1:{tema1},tema2:{tema2},tema3:{tema3},tema4:{tema4},tema5:{tema5},tema6:{tema6},tema7:{tema7},tema8:{tema8},tema9:{tema9},tema10:{tema10},tema11:{tema11},tema12:{tema12},tema13:{tema13}," +
                              $"tema14:{tema14},tema15:{tema15},tema16:{tema16},tema17:{tema17},tema18:{tema18},tema19:{tema19},tema20:{tema20},tema21:{tema21},tema22:{tema22},tema23:{tema23},tema24:{tema24},tema25:{tema25},tema26:{tema26},tema27:{tema27},topzuhe:{topzuhe},topsixiang:{topsixiang}," +
                              $"toptema:{toptema},topduizi:{topduizi},downzuhe:{downzuhe},downsixiang:{downsixiang},downtema:{downtema},downduizi:{downduizi},fenshu:{fenshu},dadanxiaoshuang:{dadanxiaoshuang},xiaodandashuang:{xiaodandashuang},duishunbao1314:{duishunbao1314}" + "\n";
                content = list.Aggregate(content, (current, item) => current + "\n" + item);

                //content = str + "\n" + content;
                var path = Environment.CurrentDirectory;
                var name = string.Empty;
                if (_Type == 1)
                {
                    name = "OddsSettingManualSimulation.ini";
                }
                else
                {
                    name = "OddsSetting.ini";
                }
                var result = Tool.Helper.WriteFile(path, name, content);
                if (result)
                {
                    MessageBox.Show("保存成功");
                }
            }
            else
            {
                str = Tool.Helper.ReadOddsSettings();
            }

            var isSuccess = OddsBusiness.ModifyOddssInfo(new OddssInfo
            {
                OddsID = Convert.ToInt32(comboBox1.Text.Substring(comboBox1.Text.Length - 1)),
                OddsName = comboBox1.Text,
                baozitongsha = baozitongsha == CheckState.Checked,
                baozihuiben = baozihuiben == CheckState.Checked,
                duizihuiben = duizihuiben == CheckState.Checked,
                shunzihuiben = shunzihuiben == CheckState.Checked,
                linjiuhuiben = linjiuhuiben == CheckState.Checked,
                dadan = Convert.ToDecimal(dadan),
                xiaodan = Convert.ToDecimal(xiaodan),
                dashuang = Convert.ToDecimal(dashuang),
                xiaoshuang = Convert.ToDecimal(xiaoshuang),
                duizi = Convert.ToDecimal(duizi),
                shunzi = Convert.ToDecimal(shunzi),
                baozi = Convert.ToDecimal(baozi),
                jishu = Convert.ToDecimal(jishu),
                da = Convert.ToDecimal(da),
                xiao = Convert.ToDecimal(xiao),
                dan = Convert.ToDecimal(dan),
                shuang = Convert.ToDecimal(shuang),
                tema0 = Convert.ToDecimal(tema0),
                tema1 = Convert.ToDecimal(tema1),
                tema2 = Convert.ToDecimal(tema2),
                tema3 = Convert.ToDecimal(tema3),
                tema4 = Convert.ToDecimal(tema4),
                tema5 = Convert.ToDecimal(tema5),
                tema6 = Convert.ToDecimal(tema6),
                tema7 = Convert.ToDecimal(tema7),
                tema8 = Convert.ToDecimal(tema8),
                tema9 = Convert.ToDecimal(tema9),
                tema10 = Convert.ToDecimal(tema10),
                tema11 = Convert.ToDecimal(tema11),
                tema12 = Convert.ToDecimal(tema12),
                tema13 = Convert.ToDecimal(tema13),
                tema14 = Convert.ToDecimal(tema14),
                tema15 = Convert.ToDecimal(tema15),
                tema16 = Convert.ToDecimal(tema16),
                tema17 = Convert.ToDecimal(tema17),
                tema18 = Convert.ToDecimal(tema18),
                tema19 = Convert.ToDecimal(tema19),
                tema20 = Convert.ToDecimal(tema20),
                tema21 = Convert.ToDecimal(tema21),
                tema22 = Convert.ToDecimal(tema22),
                tema23 = Convert.ToDecimal(tema23),
                tema24 = Convert.ToDecimal(tema24),
                tema25 = Convert.ToDecimal(tema25),
                tema26 = Convert.ToDecimal(tema26),
                tema27 = Convert.ToDecimal(tema27),
                topzuhe = Convert.ToDecimal(topzuhe),
                topsixiang = Convert.ToDecimal(topsixiang),
                toptema = Convert.ToDecimal(toptema),
                topduizi = Convert.ToDecimal(topduizi),
                downzuhe = Convert.ToDecimal(downzuhe),
                downsixiang = Convert.ToDecimal(downsixiang),
                downtema = Convert.ToDecimal(downtema),
                downduizi = Convert.ToDecimal(downduizi),
                fenshu = Convert.ToDecimal(fenshu),
                dadanxiaoshuang = Convert.ToDecimal(dadanxiaoshuang),
                xiaodandashuang = Convert.ToDecimal(xiaodandashuang),
                duishunbao1314 = Convert.ToDecimal(duishunbao1314),
            });

            if (isSuccess)
            {
                MessageBox.Show("保存成功");
            }
            else
            {
                MessageBox.Show("保存失败");
            }
        }

        /// <summary>
        /// 获取所有控件
        /// </summary>
        /// <param name="fatherControl"></param>
        /// <param name="strList"></param>
        private void GetControls(Control fatherControl, List<Control> strList)
        {
            var sonControls = fatherControl.Controls;
            //遍历所有控件
            foreach (Control c in sonControls)
            {
                strList.Add(c);
                GetControls(c, strList);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            var list = Tool.Helper.ReadOddsSettings(comboBox1.Text, "", _Type);

            var model = OddsBusiness.GetOddssInfo().FirstOrDefault(x => x.OddsName == comboBox1.Text);

            //递归循环出页面所有控件
            var conList = new List<Control>();
            foreach (Control ctl in Controls)
            {
                GetControls(ctl, conList);
            }
            var textCon = conList.FindAll(x => x.GetType().Name == "TextBox");
            var cheCon = conList.FindAll(x => x.GetType().Name == "CheckBox");
            if (string.IsNullOrWhiteSpace(list))
            {
                foreach (var txt in textCon)
                {
                    txt.Text = "";
                }
                foreach (var che in cheCon)
                {
                    ((CheckBox)che).CheckState = CheckState.Unchecked;
                }
                return;
            }

            var strList = list.Split('-')[1].Split(',').ToList();
            //foreach (var item in strList)
            //{
            //    var che = cheCon.FirstOrDefault(x => x.Name == item.Split(':')[0]);
            //    var txt = textCon.FirstOrDefault(x => x.Name == item.Split(':')[0]);
            //    if (txt != null)
            //    {
            //        txt.Text = item.Split(':')[1];
            //    }
            //    if (che != null)
            //    {
            //        ((CheckBox)che).CheckState = item.Split(':')[1] == "Unchecked" ? CheckState.Unchecked : CheckState.Checked;
            //    }
            //}

            var modelProperties = model.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo p in modelProperties)
            {
                var txt = textCon.FirstOrDefault(x => x.Name == p.Name);
                if (txt != null)
                {
                    txt.Text = p.GetValue(model).ToString();
                }
                var che = cheCon.FirstOrDefault(x => x.Name == p.Name);
                if (che != null)
                {
                    ((CheckBox)che).CheckState = Convert.ToBoolean(p.GetValue(model)) ? CheckState.Checked : CheckState.Unchecked;
                }


                Console.WriteLine("Name:{0} Value:{1}", p.Name, p.GetValue(model));
            }
        }
    }
}
