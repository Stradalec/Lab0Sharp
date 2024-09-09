using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab0
{
    public partial class Form1 : Form, IView
    {
        public Form1()
        {
            InitializeComponent();
            Presenter programPresenter = new Presenter(this);
            openFileDialog1 = new OpenFileDialog();
        }

        void IView.ReturnResultOfWidth(double width)
        {
            int limitation = Convert.ToInt32(inputBox4.Text);
            width = Math.Round(width, limitation);
            outputBox.Text = width.ToString();
        }

        void IView.ReturnResultOfHeight(double[] height)
        {
            manHeightBox.Text = height[0].ToString();
            womanHeightBox.Text = height[1].ToString();
        }

        void IView.ReturnTallestHeight(Lab0.Person[] humans)
        {
            manBox.Items.Clear();
            womanBox.Items.Clear();
            manBox.Items.Add(humans[0].Name);
            manBox.Items.Add(humans[0].Surname);
            manBox.Items.Add(humans[0].Height);
            womanBox.Items.Add(humans[1].Name);
            womanBox.Items.Add(humans[1].Surname);
            womanBox.Items.Add(humans[1].Height);
        }
        double IView.Density()
        {
            return Convert.ToDouble(inputBox1.Text);
        }

        double IView.Radius()
        {
            return Convert.ToDouble(inputBox2.Text);
        }

        string IView.PathToFile()
        {
            return pathBox.Text;
        }

        string IView.NameOfPerson()
        {
            return nameBox.Text;
        }

        string IView.SurnameOfPerson()
        {
            return surnameBox.Text;
        }

        private void genderBox_KeyPress(object sender, KeyPressEventArgs keyPress)
        {
            // Разрешить ввод только букв "м", "М", "ж" и "Ж"
            if (!(char.IsLetter(keyPress.KeyChar) && (keyPress.KeyChar == 'м' || keyPress.KeyChar == 'М' || keyPress.KeyChar == 'ж' || keyPress.KeyChar == 'Ж')))
            {
                // Отменить ввод символа
                keyPress.Handled = true;
            }
        }


        Gender IView.GenderOfPerson()
        {
            if (manButton.Checked == true)
            {
                return Gender.Male;
            }
            else
            {
                return Gender.Female;
            }
        }

        double IView.HeightOfPerson()
        {
            return Convert.ToDouble(HeightBox.Text);
        }

        double IView.Mass()
        {
            return Convert.ToDouble(inputBox3.Text);
        }

        public event EventHandler<EventArgs> ReturnWidth;
        public event EventHandler<EventArgs> AddPersonInList;
        public event EventHandler<EventArgs> CreateDocument;
        public event EventHandler<EventArgs> OpenDocument;
        public event EventHandler<EventArgs> FindHeight;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void WidthButton_Click(object sender, EventArgs inputEvent)
        {
            ReturnWidth(sender, inputEvent);
        }

        private void searchButton_Click(object sender, EventArgs inputEvent)
        {
            openFileDialog1.ShowDialog(this);
            pathBox.Text = openFileDialog1.FileName;
            FindHeight(sender, inputEvent);
        }

        private void addButton_Click(object sender, EventArgs inputEvent)
        {
            
            AddPersonInList(sender, inputEvent);
        }

        private void createButton_Click(object sender, EventArgs inputEvent)
        {
            CreateDocument(sender, inputEvent);
        }
    }
}
