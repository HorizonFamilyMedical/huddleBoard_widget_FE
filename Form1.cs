using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Net;

/*
 * Created by Kishore Kumar S and Akash C on 17-08-2023.
 */
namespace AccountDetailWindowsFormApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bdy_pnl.Visible = false;
            ControlExtension.Draggable(button1, true);
            ControlExtension.Draggable(bdy_pnl, true);
            MakeButtonRound(button1);
            LoadData();
        }

        private async void Delay()
        {
            string filepath = @"readFile.json";
            try
            {

                string jsonData = File.ReadAllText(filepath);
                JArray jArray = JArray.Parse(jsonData);
                JArray sortedArray = new JArray(jArray.OrderByDescending(obj => (string)obj["Status"]));



                foreach (JObject jObject in sortedArray)
                {
                    string Name = (string)jObject["Name"];
                    string DueDate = (string)jObject["DueDate"];
                    string Status = (string)jObject["Status"];
                    string AccountNumber = (string)jObject["Account"];
                    string PatientFName = (string)jObject["PtFname"];
                    string PatientLName = (string)jObject["PtLname"];
                    string DOB = (string)jObject["DOB"];
                    string ipHostName = Dns.GetHostName();
                    string ipAddress = Dns.GetHostByName(ipHostName).AddressList[0].ToString();

                    Invoke(new Action(() =>
                    {
                        accountLabel.Text = AccountNumber;
                        ptName.Text = $"{PatientFName} {PatientLName}";
                        dobLabel.Text = DOB;

                        // ... (create and add controls to the flowLayoutPanel1)
                    }));

                    Panel dataPanel = new Panel();
                    dataPanel.BorderStyle = BorderStyle.FixedSingle;
                    dataPanel.MinimumSize = new Size(350, 80);
                    dataPanel.Margin = new Padding(5);
                    dataPanel.Padding = new Padding(5);
                    dataPanel.AutoScroll = true;

                    Label nameLabel = new Label();
                    nameLabel.Text = $"Care Name: {Name}\n";
                    nameLabel.Font = new Font("Century Gothic", 10, FontStyle.Regular);
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

                    Invoke(new Action(() =>
                    {
                        flowLayoutPanel1.AutoScroll = true;
                        flowLayoutPanel1.Controls.Add(dataPanel);
                    }));
                    //flowLayoutPanel1.AutoScroll = true;
                    //Console.WriteLine("Complete");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error :: " + ex.Message);
            }
        }
        private async void LoadData()
        {
            while (true)
            {
                await Task.Run(() => Delay());
                // Delay for 5 seconds
                await Task.Delay(5000);
                flowLayoutPanel1.Controls.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsAccessible = true;
            if (bdy_pnl.Visible)
            {
                bdy_pnl.Visible = false;
            }
            else { bdy_pnl.Visible = true; }

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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
