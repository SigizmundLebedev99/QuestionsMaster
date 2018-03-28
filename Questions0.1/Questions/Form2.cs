using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Questions
{
    public partial class Form2 : Form
    {
        static string fn;
        public static string fileName { get { return fn; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new Exception("Hе указан путь к файлу");
                else fn = value;
            }
        }
        public Form2()
        {
            InitializeComponent();          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text)|| string.IsNullOrWhiteSpace(textBox3.Text)) MessageBox.Show("Заполните данные!");
            else
            {
                StreamWriter writer = new StreamWriter(fileName);
                writer.WriteLine("ФИО тестируемого: " + textBox1.Text + "; Группа " + textBox2.Text + DateTime.Now +  Environment.NewLine);
                writer.Close();
                (new Form1()).Show();
                this.Hide();
            }
        }

        private void brouseButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                fileName = openFileDialog1.FileName;
                textBox3.Text = fileName;
            }

        }
    }
}
