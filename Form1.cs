using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Net;
using System.Data.SqlClient;
/*
 * Created by Kishore Kumar S and Akash C on 17-08-2023.
 */
namespace AccountDetailWindowsFormApp
{
    public partial class Form1 : Form
    {
        string AccountNumber;
        int Acc_Id;
        public Form1()
        {
            InitializeComponent();
            bdy_pnl.Visible = false;
            ControlExtension.Draggable(button1, true);
            ControlExtension.Draggable(bdy_pnl, true);
            MakeButtonRound(button1);
            LoadData();
        }

        private void Delay()
        {
            string filepath = @"readFile.json";
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source = HUDDLEBOARDV2\SQLEXPRESS; Initial Catalog=Huddle_V2;Integrated Security=True"))
                {
                    connection.Open();
                    string sqlQuery = "SELECT * FROM Widget WHERE Displayed = '" + 0 + "'";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var response = reader["APIResult"];
                            Acc_Id = (int)reader["id"];
                            string responseContent = response.ToString();
                            File.WriteAllText(filepath, responseContent);
                        }
                    }
                }

                string jsonData = File.ReadAllText(filepath);
                JArray jArray = JArray.Parse(jsonData);
                JArray sortedArray = new JArray(jArray[0].OrderByDescending(obj => (string)obj["Status"]));

                foreach (JObject jObject in sortedArray.Cast<JObject>())
                {
                    string Name = (string)jObject["Name"];
                    string DueDate = (string)jObject["DueDate"];
                    string Status = (string)jObject["Status"];
                    AccountNumber = (string)jObject["Account"];
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
                    }));

                    Panel dataPanel = new Panel
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        MinimumSize = new Size(350, 80),
                        Margin = new Padding(5),
                        Padding = new Padding(5),
                        AutoScroll = true
                    };

                    Label nameLabel = new Label
                    {
                        Text = $"Care Name: {Name}\n",
                        Font = new Font("Century Gothic", 10, FontStyle.Regular),
                        AutoSize = true
                    };

                    Label dueDateLabel = new Label
                    {
                        Text = $"Due Date: {DueDate}\n",
                        Font = new Font("Century Gothic", 10, FontStyle.Regular),
                        AutoSize = true
                    };

                    Label statusLabel = new Label
                    {
                        Text = $"Status: {Status}",
                        Font = new Font("Century Gothic", 10, FontStyle.Regular),
                        AutoSize = true
                    };

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

        private void Widget_Click(object sender, EventArgs e)
        {
            IsAccessible = true;
            if (bdy_pnl.Visible)
            {
                bdy_pnl.Visible = false;
            }
            else { bdy_pnl.Visible = true; }

            try
            {
                SqlConnection connection = new SqlConnection(@"Data Source = HUDDLEBOARDV2\SQLEXPRESS; Initial Catalog=Huddle_V2;Integrated Security=True");
                SqlCommand cmd =connection.CreateCommand();
                cmd.CommandText = "UPDATE Widget SET Displayed = 1 WHERE id= '"+ Acc_Id + "'";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ":: Something went wrong.");
            }
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


        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void Bdy_pnl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newLock = new Point(e.X + bdy_pnl.Location.X, e.Y + bdy_pnl.Location.Y);
                bdy_pnl.Location = newLock;
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void FlowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
