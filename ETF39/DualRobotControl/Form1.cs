using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using PuppetMaster;

namespace DualRobotControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Tuple<string, string>[] strats;

        private void Form1_Load(object sender, EventArgs e)
        {
            List<Tuple<string, string>> stratList = new List<Tuple<string, string>>();
            string[] dirs = Directory.GetDirectories(Controller.GetPath());

            for (int i = 0; i < dirs.Length; i++)
            {
                string[] files = Directory.GetFiles(dirs[i]);
                for (int j = 0; j < files.Length; j++)
                {
                    if (files[j].ToLower().EndsWith("main.strat"))
                    {
                        string name = dirs[i].Substring(dirs[i].LastIndexOf('\\') + 1);
                        stratList.Add(new Tuple<string, string>(name, dirs[i]));
                    }
                }
            }

            foreach (Tuple<string, string> strat in stratList)
            {
                comboBox1.Items.Add(strat.Item1);
                comboBox2.Items.Add(strat.Item1);
            }

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            strats = stratList.ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controller c = new Controller(textBox1.Text, comboBox1.Text, true, false);
            c.RunInNewThread();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Controller c = new Controller(textBox2.Text, comboBox2.Text, true, false);
            c.RunInNewThread();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Controller c = new Controller(textBox1.Text, comboBox1.Text, true, false);
            c.RunInNewThread();

            c = new Controller(textBox2.Text, comboBox2.Text, true, false);
            c.RunInNewThread();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
