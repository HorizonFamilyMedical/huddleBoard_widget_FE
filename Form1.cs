using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Drawing2D;

/*
 * Created by Kishore Kumar S and Akash C on 17-08-2023.
 */

namespace AccountDetailWindowsFormApp
{
    public partial class Form1 : Form
    {
        string AccountNumber;
        string PatientFName;
        string PatientLName;
        string DOB;
        public Form1()
        {
            InitializeComponent();
            bdy_pnl.Visible = false;
            ControlExtension.Draggable(button1, true);
            ControlExtension.Draggable(bdy_pnl, true);
            MakeButtonRound(button1);
            

            string filepath = @"readFile.json";
            try
            {

                string jsonData = File.ReadAllText(filepath);
                JArray jArray = JArray.Parse(jsonData);
                JArray sortedArray = new JArray(jArray.OrderByDescending(obj => (string)obj["Status"]));
                string sortedJsonData = sortedArray.ToString();


                foreach (JObject jObject in sortedArray)
                {
                    string Name = (string)jObject["Name"];
                    string DueDate = (string)jObject["DueDate"];
                    string Status = (string)jObject["Status"];
                    AccountNumber = (string)jObject["Account Number"];
                    PatientFName = (string)jObject["PtFname"];
                    PatientLName = (string)jObject["PtLname"];
                    DOB = (string)jObject["DOB"];

                    accountLabel.Text = AccountNumber;
                    ptName.Text = $"{PatientFName} {PatientLName}";
                    dobLabel.Text = DOB;

                    Panel dataPanel = new Panel();
                    dataPanel.BorderStyle = BorderStyle.FixedSingle;
                    dataPanel.MinimumSize = new Size(350, 80);
                    dataPanel.Margin = new Padding(5);
                    dataPanel.Padding = new Padding(5);
                    dataPanel.AutoScroll = true;

                    Label nameLabel = new Label();
                    nameLabel.Text = $"Care Name: {Name}\n";
                    nameLabel.Font =  new Font("Century Gothic", 10, FontStyle.Regular);
                    nameLabel.AutoSize = true;

                    Label dueDateLabel = new Label();
                    dueDateLabel.Text = $"Due Date: {DueDate}\n";
                    dueDateLabel.Font = new Font("Century Gothic", 10, FontStyle.Regular);
                    dueDateLabel.AutoSize = true;

                    Label statusLabel = new Label();
                    statusLabel.Text = $"Status: {Status}";
                    statusLabel.Font = new Font("Century Gothic", 10, FontStyle.Regular);
                    statusLabel.AutoSize = true;

                    nameLabel.Location = new Point(10, 10);
                    dueDateLabel.Location = new Point(10, 30);
                    statusLabel.Location = new Point(10, 50);

                    dataPanel.Controls.Add(nameLabel);
                    dataPanel.Controls.Add(dueDateLabel);
                    dataPanel.Controls.Add(statusLabel);

                    flowLayoutPanel1.Controls.Add(dataPanel);
                    flowLayoutPanel1.AutoScroll = true;
                    Console.WriteLine("Complete");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error :: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsAccessible = true;
            if (bdy_pnl.Visible)
            {
                bdy_pnl.Visible = false;
            }
            else { bdy_pnl.Visible = true;}
           
        }

        private void MakeButtonRound(Button button)
        {
            int diameter = Math.Min(button.Width, button.Height);
            button.Width = diameter;
            button.Height = diameter;

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, diameter, diameter);

            button.Region = new Region(path);
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void bdy_pnl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newLock = new Point(e.X + bdy_pnl.Location.X, e.Y + bdy_pnl.Location.Y);
                bdy_pnl.Location = newLock;
            }
        }

     
    }
}
