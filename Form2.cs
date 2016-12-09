using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomAttributesDataBaseFirst
{
    public partial class Form2 : Form
    {
        public string NameAttribute { get; set; }
        public string SomeData { get; set; }
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(List<string> atr) : this()
        {
            comboBox1.DataSource = atr;
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            NameAttribute = textBox1.Text;
            SomeData = textBox2.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem!=null)
            textBox1.Text = comboBox1.SelectedItem.ToString();
        }
    }
}
