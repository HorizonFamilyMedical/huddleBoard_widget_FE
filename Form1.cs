using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Tesseract;
using ScreenTest;
using System.Net;

/*
 * Created by Kishore Kumar S and Akash C on 17-08-2023.
 */

namespace ScreenTest
{
    public class PrintScreen
    {
        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);

            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);

            return img;
        }

        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }

        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        }
    }
}

namespace AccountDetailWindowsFormApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            DirectoryInfo di = new DirectoryInfo(@"C:\Users\DELL\Desktop");
            if (!di.Exists) { di.Create(); }
            PrintScreen ps = new PrintScreen();
            ps.CaptureScreenToFile(di + $"\\screenShootImg.png", ImageFormat.Png);
            Console.WriteLine("No.1 Completed");
            var path = @"C:\Users\DELL\source\repos\testingNew\testingNew\tessdata";
            var sourceFilePath = di + $"\\screenShootImg.png";
            using (var engine = new TesseractEngine(path, "eng"))
            {
                engine.SetVariable("user_defined_dpi", "70");
                using (var img = Pix.LoadFromFile(sourceFilePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        Console.WriteLine("---Image Text---");
                        Console.WriteLine(text);
                        string txtFilePath = @"C:\Users\DELL\Documents\textFile.txt";
                        if (!File.Exists(txtFilePath))
                        {
                            using (StreamWriter sw = File.CreateText(txtFilePath))
                            {
                                sw.WriteLine(text);
                            }
                        }
                        else
                        {
                            using (StreamWriter sw = File.AppendText(txtFilePath))
                            {
                                sw.Write(text);
                            }
                            File.Delete(di + $"\\screenShootImg.png");
                        }
                        List<List<string>> groups = new List<List<string>>();
                        List<string> current = null;
                        string word = "Account No:";
                        string trgtLine = null;
                        string num = null;
                        string number = null;
                        foreach (var line in File.ReadAllLines(txtFilePath))
                        {
                            if (line.Contains(word))
                            {
                                trgtLine = line;
                                current = new List<string>();
                                groups.Add(current);
                                num = trgtLine.Substring(trgtLine.IndexOf(word), 17);
                                number = Regex.Replace(num, "[^0-9]+", string.Empty);
                                File.AppendAllText(txtFilePath, $"A/c no: {number}");
                                try
                                {
                                    HttpClient client = new HttpClient();
                                    client.BaseAddress = new Uri("http://Huddleboardv2:81/api/GetPatientGaps");
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    HttpResponseMessage response = client.GetAsync($"/{number}").Result;
                                    string msg = response.ToString();
                                    if (response.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine(response.Content);
                                        File.WriteAllText(txtFilePath, $"[{msg}]");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.ToString();
                                }
                                break;
                            }
                        }
                    }
                }
            }


            bdy_pnl.Visible = false;
            ControlExtension.Draggable(button1, true);
            ControlExtension.Draggable(bdy_pnl, true);
            MakeButtonRound(button1);
            LoadData();
        }

        private async void Delay()
        {
            string filepath = @"horizon.json";
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
