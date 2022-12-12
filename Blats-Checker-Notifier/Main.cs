using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Mail;
using System.Timers;
using System.Runtime.InteropServices;
using System.IO.Compression;
using AutoUpdaterDotNET;
using System.Diagnostics;
using System.Windows.Controls;

namespace Blats_Checker_Notifier
{
    public partial class Main : MaterialForm
    {
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        string[] pingR = new string[19];
        string[] portR = new string[9];
        int minutes = 4, seconds = 60;
        bool themeMode = false, autoLoad = false;
        

        public Main()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue800, Primary.LightBlue900, Primary.LightBlue500, Accent.LightBlue200, TextShade.WHITE);
            this.FormBorderStyle = FormBorderStyle.None;
            InitTimer();
            ipReciever();
            autoLoader();
        }

        #region Gui Settings

        MaterialSkinManager ThemeManager = MaterialSkinManager.Instance; //constructor for theme dark and light

        private void themeSwitcher_CheckedChanged(object sender, EventArgs e) //changing the theme
        {
            if(themeSwitcher.Checked)
            {
                ThemeManager.Theme = MaterialSkinManager.Themes.DARK;
                themeMode = true;
            }
            else
            {
                ThemeManager.Theme = MaterialSkinManager.Themes.LIGHT;
                themeMode = false;
            }
        }

        private void btnBlue_CheckedChanged(object sender, EventArgs e) //adding blue color on some fonts, labels, progressbar etc.
        {
            ThemeManager.ColorScheme = new ColorScheme(Primary.LightBlue800, Primary.LightBlue900, Primary.LightBlue500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void btnGrayBlue_CheckedChanged(object sender, EventArgs e) //adding bluegrey color on some fonts, labels, progressbar etc.
        {
            ThemeManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void btnGreen_CheckedChanged(object sender, EventArgs e) //adding green color on some fonts, labels, progressbar etc.
        {
            ThemeManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);
        }

        private void btnRed_CheckedChanged(object sender, EventArgs e) //adding red color on some fonts, labels, progressbar etc.
        {
            ThemeManager.ColorScheme = new ColorScheme(Primary.Red800, Primary.Red900, Primary.Red500, Accent.Red200, TextShade.WHITE);
        }
        #endregion

        #region TextBoxes Settings

        private void txtPortCheck_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtPortCheck_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtPortCheck.Text, "[^0-9]"))
            {
                txtPortCheck.Text = string.Empty;
            }
        }

        private void PortSelector_SelectedValueChanged(object sender, EventArgs e)
        {
            if (PortSelector.SelectedIndex == 1)
            {
                txtPortCheck.Text = "21";
            }
            else if (PortSelector.SelectedIndex == 2)
            {
                txtPortCheck.Text = "22";
            }
            else if (PortSelector.SelectedIndex == 3)
            {
                txtPortCheck.Text = "23";
            }
            else if (PortSelector.SelectedIndex == 4)
            {
                txtPortCheck.Text = "25";
            }
            else if (PortSelector.SelectedIndex == 5)
            {
                txtPortCheck.Text = "53";
            }
            else if (PortSelector.SelectedIndex == 6)
            {
                txtPortCheck.Text = "80";
            }
            else if (PortSelector.SelectedIndex == 7)
            {
                txtPortCheck.Text = "110";
            }
            else if (PortSelector.SelectedIndex == 8)
            {
                txtPortCheck.Text = "115";
            }
            else if (PortSelector.SelectedIndex == 9)
            {
                txtPortCheck.Text = "143";
            }
            else if (PortSelector.SelectedIndex == 10)
            {
                txtPortCheck.Text = "443";
            }
            else if (PortSelector.SelectedIndex == 11)
            {
                txtPortCheck.Text = "465";
            }
            else if (PortSelector.SelectedIndex == 12)
            {
                txtPortCheck.Text = "993";
            }
            else if (PortSelector.SelectedIndex == 13)
            {
                txtPortCheck.Text = "995";
            }
            else if (PortSelector.SelectedIndex == 14)
            {
                txtPortCheck.Text = "3306";
            }
            else if (PortSelector.SelectedIndex == 15)
            {
                txtPortCheck.Text = "6379";
            }
            else if (PortSelector.SelectedIndex == 16)
            {
                txtPortCheck.Text = "5900";
            }
        }

        private void TimeSelector_SelectedValueChanged(object sender, EventArgs e)
        {
            if (TimeSelector.SelectedIndex == 1)
            {
                minutes = 0;
            }
            if (TimeSelector.SelectedIndex == 2)
            {
                minutes = 2;
            }
            if (TimeSelector.SelectedIndex == 3)
            {
                minutes = 4;
            }
            if (TimeSelector.SelectedIndex == 4)
            {
                minutes = 9;
            }
            if (TimeSelector.SelectedIndex == 5)
            {
                minutes = 14;
            }
            if (TimeSelector.SelectedIndex == 6)
            {
                minutes = 19;
            }
            if (TimeSelector.SelectedIndex == 7)
            {
                minutes = 29;
            }
            if (TimeSelector.SelectedIndex == 8)
            {
                minutes = 59;
            }
        }
        #endregion

        #region Tools
        private void btnPingCheck_Click(object sender, EventArgs e) //ping tool ip or hostname
        {
            try
            {
                Ping p = new Ping();
                PingReply r;
                string s;
                s = txtIPHostname.Text;
                r = p.Send(s);
                if (r.Status == IPStatus.Success)
                {
                     MaterialMessageBox.Show("Ping to " + s.ToString() + "[" + r.Address.ToString() + "]" + " Successful"
                      + " Response delay = " + r.RoundtripTime.ToString() + " ms" + "\n", "Ping Tool", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
            }
            catch
            {
                if (string.IsNullOrWhiteSpace(txtIPHostname.Text) || txtIPHostname.Text == "")
                    MaterialMessageBox.Show("You need to enter ip or hostname. IP field cannot be blank.", "Ping Tool", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                else
                    MaterialMessageBox.Show("IP or Hostname is not reachable.", "Ping Tool", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
        }

        private void btnPortCheck_Click(object sender, EventArgs e) //port tool
        {
            try
            {
                string hostname = txtIPHostname2.Text;
                int portno = int.Parse(txtPortCheck.Text);
                IPAddress ipa = (IPAddress)Dns.GetHostAddresses(hostname)[0];

                System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                sock.Connect(ipa, portno);
                if (sock.Connected == true)  // Port is in use and connection is successful
                    MaterialMessageBox.Show("Checking port " + txtPortCheck.Text + " at " + txtIPHostname2.Text + ": Port is Open!", "Port Tool", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                sock.Close();

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (ex.ErrorCode == 10060)  // Port is unused and could not establish connection 
                    MaterialMessageBox.Show("Checking port " + txtPortCheck.Text + " at " + txtIPHostname2.Text + ": Port is Closed.", "Port Tool", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                else
                    MaterialMessageBox.Show("Error occured with ip/hostname or port.", "Port Tool", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
            catch
            {
                if (string.IsNullOrWhiteSpace(txtIPHostname2.Text) || txtIPHostname2.Text == "")
                    MaterialMessageBox.Show("You need to enter ip or hostname. IP field cannot be blank.", "Port Tool", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                else if (string.IsNullOrWhiteSpace(txtPortCheck.Text) || txtPortCheck.Text == "")
                    MaterialMessageBox.Show("You need to enter port number. Port field cannot be blank.", "Port Tool", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
        }

        private void btnSendTestEmail_Click(object sender, EventArgs e)
        {
            if (txtEmailFrom.Text == "" || txtEmailFrom.Text == null)
            {
                MaterialMessageBox.Show("You need to enter Email Sender. This field cannot be blank.", "Email Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
            else if(txtEmailPassFrom.Text == "" || txtEmailPassFrom.Text == null)
            {
                MaterialMessageBox.Show("You need to enter the Senders Password. This field cannot be blank.", "Email Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
            else if (txtEmailTo.Text == "" || txtEmailTo.Text == null)
            {
                MaterialMessageBox.Show("You need to enter the Recievers Email. This field cannot be blank.", "Email Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
            else
            {
                TestEmailSender();
            }

        }

        private void ipReciever()
        {
            try
            {
                string url = "https://services.blats.gr/ipreciever.php";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] ipAddressWithText = response.Split(':');
                string ipAddressWithHTMLEnd = ipAddressWithText[1].Substring(1);
                string[] ipAddress = ipAddressWithHTMLEnd.Split('<');
                string mainIP = ipAddress[0];
                lblCurrentIP.Text = "Your current IP Address: " + mainIP;
            }
            catch
            {
            }

        }


        private void EmailSender() //sending email when Port setting or ping settings found an error.
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            try
            {
                MailMessage newMail = new MailMessage();
                // use the Gmail SMTP Host
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                // Follow the RFS 5321 Email Standard
                newMail.From = new MailAddress(txtEmailFrom.Text);

                newMail.To.Add(txtEmailTo.Text);

                newMail.Subject = "Error(s) has been detected on your server(s)"; // declare the email subject

                //newMail.IsBodyHtml = true; newMail.Body = "<h1> This is my first Templated Email in C# </h1>"; // use HTML for the email body
                newMail.IsBodyHtml = true; newMail.Body = "<h1>" + string.Join("", pingR) + "\n" + string.Join("", portR) + "</h1>";// use HTML for the email body

                // enable SSL for encryption across channels
                client.EnableSsl = true;
                // Port 465 for SSL communication
                client.Port = 587;
                // Provide authentication information with Gmail SMTP server to authenticate your sender account
                client.Credentials = new System.Net.NetworkCredential(txtEmailFrom.Text, txtEmailPassFrom.Text);

                client.Send(newMail); // Send the constructed mail     
            }
            catch (Exception ex)
            {
                var messageForm = new Form() { Size = new Size(0, 0) };
                messageForm.StartPosition = FormStartPosition.CenterScreen;
                Task.Delay(TimeSpan.FromSeconds(20))
                    .ContinueWith((t) => messageForm.Close(), TaskScheduler.FromCurrentSynchronizationContext());
                MaterialMessageBox.Show(messageForm, (ex.ToString()), "Email Tool");
            }
        }

        private void TestEmailSender() //sending test email.
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            try
            {
                MailMessage newMail = new MailMessage();
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                newMail.From = new MailAddress(txtEmailFrom.Text);
                newMail.To.Add(txtEmailTo.Text);
                newMail.Subject = "This is a Test Email"; // declare the email subject
                newMail.IsBodyHtml = true; newMail.Body = "<h1> This is a test email, seems everything works perfect! </h1>"; // use HTML for the email body
                client.EnableSsl = true;
                client.Port = 587;
                client.Credentials = new System.Net.NetworkCredential(txtEmailFrom.Text, txtEmailPassFrom.Text);
                client.Send(newMail); // Send the constructed mail
                MaterialMessageBox.Show("Test Email was sent successfully!", "Email Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
            catch
            {
                MaterialMessageBox.Show("Something went wrong. We couldnt send your email, please check your credentials.", "Email Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
        }

        private void WhoIsChecker() // checking whois from cmd
        {
           // adding the path where the folder and the zip file will created
            string folderPath = @"C:\Blats-Notifier\WhoIs";

            // open cmd command into WhoIs
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C cd C:/Blats-Notifier/WhoIs && whois -v " + txtWhoIs.Text + " > " + txtWhoIs.Text + ".txt";
            process.StartInfo = startInfo;
            process.Start();
            System.Threading.Thread.Sleep(3000);
            //read into richtextbox the txt we generated before and delete the txt file we created
            txtWhoIsReply.Text = File.ReadAllText(folderPath + "\\" + txtWhoIs.Text + ".txt");
            File.Delete(folderPath + "\\" + txtWhoIs.Text + ".txt");
        }

        private void btnWhoIs_Click(object sender, EventArgs e)
        {
            WhoIsChecker();
        }
        #endregion

        #region Checker & Timer
        public void InitTimer()
        {
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000; // in miliseconds
            timer1.Start();
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            if (minutes > 0 && seconds > 0)
            {
                CountdownTimer();
            }
            if (minutes == 0 && seconds > 0)
            {
                CountdownTimer();
            }
            else if (minutes == 0 && seconds == 0)
            {
                if (TimeSelector.SelectedIndex == 1)
                {
                    minutes = 0;
                }
                if (TimeSelector.SelectedIndex == 2)
                {
                    minutes = 2;
                }
                if (TimeSelector.SelectedIndex == 3)
                {
                    minutes = 4;
                }
                if (TimeSelector.SelectedIndex == 4)
                {
                    minutes = 9;
                }
                if (TimeSelector.SelectedIndex == 5)
                {
                    minutes = 14;
                }
                if (TimeSelector.SelectedIndex == 6)
                {
                    minutes = 19;
                }
                if (TimeSelector.SelectedIndex == 7)
                {
                    minutes = 29;
                }
                if (TimeSelector.SelectedIndex == 8)
                {
                    minutes = 59;
                }
                seconds = 60;
                CountdownTimer();
                ScriptChecker();
                ipReciever();
                AutoUpdater.Start("https://services.blats.gr/blats-checker-notifier/AutoUpdater.xml");
            }
        }

        public void ScriptChecker()
        {
            automation();
        }

        public void CountdownTimer()
        {
            seconds--;
            if (minutes <= 9)
            {
                lblShowTime.Text = "Next check in: 0" + minutes + ":" + seconds;
            }
            else
            {
                lblShowTime.Text = "Next check in: " + minutes + ":" + seconds;
            }


            if (seconds < 10)
            {
                lblShowTime.Text = "Next check in: 0" + minutes + ":0" + seconds;
            }
            if (seconds == 0)
            {
                if (minutes > 0)
                {
                    minutes--;
                    seconds = 60;
                }
                else
                {
                    lblShowTime.Text = ("Results: Status OK!");
                }
            }
        }
        #endregion

        #region Automation
        private void automation()
        {
            Array.Clear(pingR);
            Array.Clear(portR);
            int i = 0, j = 0, y = 0, k = 0, l = 0;
            bool[] ping = new bool[19];
            bool[] port = new bool[9];
            string[] s = new string[] { txtPing.Text, txtPing2.Text, txtPing3.Text, txtPing4.Text, txtPing5.Text, txtPing6.Text, txtPing7.Text,
                                        txtPing8.Text, txtPing9.Text, txtPing10.Text, txtPing11.Text, txtPing12.Text, txtPing13.Text, txtPing14.Text,
                                        txtPing15.Text, txtPing16.Text, txtPing17.Text, txtPing18.Text, txtPing19.Text, txtPing20.Text};
            string[] s1 = new string[] { txtPortIP.Text, txtPortIP2.Text, txtPortIP3.Text, txtPortIP4.Text, txtPortIP5.Text, txtPortIP6.Text, txtPortIP7.Text,
                                         txtPortIP8.Text,txtPortIP9.Text,txtPortIP10.Text};
            string[] s2 = new string[] { txtPort.Text, txtPort2.Text, txtPort3.Text, txtPort4.Text, txtPort5.Text, txtPort6.Text, txtPort7.Text,
                                         txtPort8.Text,txtPort9.Text,txtPort10.Text};

            for (i = 0; i < 19; i++)
            {
                try
                {
                    Ping p = new Ping();
                    PingReply r;
                    if (s[i] == "")
                    {
                        ping[i] = true;
                    }
                    r = p.Send(s[i]);
                    if (r.Status == IPStatus.Success)
                    {
                        ping[i] = true;
                    }
                    else
                    {
                        ping[i] = false;
                    }
                }
                catch
                {
                    //we dont need exception!
                }
            }

            for (j = 0; j < 9; j++)
            {
                try
                {
                    if (s1[j] == "")
                    {
                        port[j] = true;
                    }
                    string hostname = s1[j];
                    int portno = int.Parse(s2[j]);
                    IPAddress ipa = (IPAddress)Dns.GetHostAddresses(hostname)[0];
                    System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    sock.Connect(ipa, portno);
                    if (sock.Connected == true)  // Port is in use and connection is successful
                    {
                        port[j] = true;
                    }
                    else
                    {
                        port[j] = false;
                    }
                    sock.Close();
                }
                catch
                {
                    // we dont need exception!
                }
            }

            for (y = 0; y < 9; y++)
            {
                if (ping[y] == false)
                {
                    pingR[k] = "Unreachable IP or Hostname: " + s[y] + " | At Ping tool: " + (y + 1) + "\n";
                    k++;
                }

                if (port[y] == false)
                {
                    portR[l] = "Port is closed or error occured: " + s1[y] + s2[y] + " | At Port tool: " + (y + 1) + "\n";
                    l++;
                }
            }
            if ((pingR[0] == null || pingR[0] == "") && (portR[0] == null || portR[0] == ""))
            {
                var messageForm = new Form() { Size = new Size(0, 0) };
                messageForm.StartPosition = FormStartPosition.CenterScreen;
                Task.Delay(TimeSpan.FromSeconds(10))
                    .ContinueWith((t) => messageForm.Close(), TaskScheduler.FromCurrentSynchronizationContext());
                MaterialMessageBox.Show(messageForm, "Everything works fine!", "Automation Tool");
            }
            else
            {
                var messageForm = new Form() { Size = new Size(0, 0) };
                messageForm.StartPosition = FormStartPosition.CenterScreen;
                Task.Delay(TimeSpan.FromSeconds(10))
                    .ContinueWith((t) => messageForm.Close(), TaskScheduler.FromCurrentSynchronizationContext());
                MaterialMessageBox.Show(messageForm, "Error(s) has been detected on your server(s). An email was sent successfully. \n\nError(s): \n" + string.Join("", pingR) + "\n" + string.Join("", portR), "Automation Tool");
                EmailSender();
            }
        }
        #endregion

        #region Save & Load
        private void btnSave_Click(object sender, EventArgs e)
        {
            // adding the path where the folder and the text file save button will create
            string filePath = @"C:\Blats-Notifier\MySettings.txt";
            //checking if there are empty fields before saving
            if (txtEmailFrom.Text == "" || txtEmailPassFrom.Text == "" || txtEmailTo.Text == "")
            {
                MaterialMessageBox.Show("All fields in the email settings must be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
            else
            {
                //checking if both ip and port fields are completed.
                if (txtPortIP.Text != "" && txtPort.Text == "" || txtPortIP.Text == "" && txtPort.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP2.Text != "" && txtPort2.Text == "" || txtPortIP2.Text == "" && txtPort2.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP3.Text != "" && txtPort3.Text == "" || txtPortIP3.Text == "" && txtPort3.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP4.Text != "" && txtPort4.Text == "" || txtPortIP4.Text == "" && txtPort4.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP5.Text != "" && txtPort5.Text == "" || txtPortIP5.Text == "" && txtPort5.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP6.Text != "" && txtPort6.Text == "" || txtPortIP6.Text == "" && txtPort6.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP7.Text != "" && txtPort7.Text == "" || txtPortIP7.Text == "" && txtPort7.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP8.Text != "" && txtPort8.Text == "" || txtPortIP8.Text == "" && txtPort8.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP9.Text != "" && txtPort9.Text == "" || txtPortIP9.Text == "" && txtPort9.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else if (txtPortIP10.Text != "" && txtPort10.Text == "" || txtPortIP10.Text == "" && txtPort10.Text != "")
                {
                    MaterialMessageBox.Show("Both ip and port fields should be completed.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                else
                {
                    string[] contents = new string[47];
                    if (themeMode == true)
                    {
                        contents[0] = "true";
                    }
                    else
                    {
                        contents[0] = "false";
                    }
                    contents[1] = minutes.ToString(); contents[2] = txtEmailFrom.Text;
                    contents[3] = txtEmailPassFrom.Text; contents[4] = txtEmailTo.Text;
                    contents[5] = txtPing.Text; contents[6] = txtPing2.Text;
                    contents[7] = txtPing3.Text; contents[8] = txtPing4.Text;
                    contents[9] = txtPing5.Text; contents[10] = txtPing6.Text;
                    contents[11] = txtPing7.Text; contents[12] = txtPing8.Text;
                    contents[13] = txtPing9.Text; contents[14] = txtPing10.Text;
                    contents[15] = txtPing11.Text; contents[16] = txtPing12.Text;
                    contents[17] = txtPing13.Text; contents[18] = txtPing14.Text;
                    contents[19] = txtPing15.Text; contents[20] = txtPing16.Text;
                    contents[21] = txtPing17.Text; contents[22] = txtPing18.Text;
                    contents[23] = txtPing19.Text; contents[24] = txtPing20.Text;
                    contents[25] = txtPortIP.Text; contents[26] = txtPort.Text;
                    contents[27] = txtPortIP2.Text; contents[28] = txtPort2.Text;
                    contents[29] = txtPortIP3.Text; contents[30] = txtPort3.Text;
                    contents[31] = txtPortIP4.Text; contents[32] = txtPort4.Text;
                    contents[33] = txtPortIP5.Text; contents[34] = txtPort5.Text;
                    contents[35] = txtPortIP6.Text; contents[36] = txtPort6.Text;
                    contents[37] = txtPortIP7.Text; contents[38] = txtPort7.Text;
                    contents[39] = txtPortIP8.Text; contents[40] = txtPort8.Text;
                    contents[41] = txtPortIP9.Text; contents[42] = txtPort9.Text;
                    contents[43] = txtPortIP10.Text; contents[44] = txtPort10.Text;
                    contents[45] = TimeSelector.SelectedIndex.ToString();
                    if (!File.Exists(filePath))
                    {
                        var MySettings = File.Create(filePath);
                        MySettings.Close();
                        File.WriteAllLines(filePath, contents);
                    }
                    else
                    {
                        File.WriteAllLines(filePath, contents);
                    }
                    MaterialMessageBox.Show("Settings saved successfully!", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string filePath = @"C:\Blats-Notifier\MySettings.txt";
            if (!File.Exists(filePath))
            {
                MaterialMessageBox.Show("No Settings to Load.", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
            else
            {
                string[] MySettings = File.ReadAllLines(filePath);
                if (MySettings[0] == "true")
                {
                    themeMode = true;
                    ThemeManager.Theme = MaterialSkinManager.Themes.DARK;
                    themeSwitcher.Checked = true;
                }
                else
                {
                    themeMode = false;
                    ThemeManager.Theme = MaterialSkinManager.Themes.LIGHT;
                }
                minutes = Int32.Parse(MySettings[1]); txtEmailFrom.Text = MySettings[2];
                txtEmailPassFrom.Text = MySettings[3]; txtEmailTo.Text = MySettings[4];
                txtPing.Text = MySettings[5]; txtPing2.Text = MySettings[6];
                txtPing3.Text = MySettings[7]; txtPing4.Text = MySettings[8];
                txtPing5.Text = MySettings[9]; txtPing6.Text = MySettings[10];
                txtPing7.Text = MySettings[11]; txtPing8.Text = MySettings[12];
                txtPing9.Text = MySettings[13]; txtPing10.Text = MySettings[14];
                txtPing11.Text = MySettings[15]; txtPing12.Text = MySettings[16];
                txtPing13.Text = MySettings[17]; txtPing14.Text = MySettings[18];
                txtPing15.Text = MySettings[19]; txtPing16.Text = MySettings[20];
                txtPing17.Text = MySettings[21]; txtPing18.Text = MySettings[22];
                txtPing19.Text = MySettings[23]; txtPing20.Text = MySettings[24];
                txtPortIP.Text = MySettings[25]; txtPort.Text = MySettings[26];
                txtPortIP2.Text = MySettings[27]; txtPort2.Text = MySettings[28];
                txtPortIP3.Text = MySettings[29]; txtPort3.Text = MySettings[30];
                txtPortIP4.Text = MySettings[31]; txtPort4.Text = MySettings[32];
                txtPortIP5.Text = MySettings[33]; txtPort5.Text = MySettings[34];
                txtPortIP6.Text = MySettings[35]; txtPort6.Text = MySettings[36];
                txtPortIP7.Text = MySettings[37]; txtPort7.Text = MySettings[38];
                txtPortIP8.Text = MySettings[39]; txtPort8.Text = MySettings[40];
                txtPortIP9.Text = MySettings[41]; txtPort9.Text = MySettings[42];
                txtPortIP10.Text = MySettings[43]; txtPort10.Text = MySettings[44];
                TimeSelector.SelectedIndex = Int32.Parse(MySettings[45]);
                MaterialMessageBox.Show("Settings loaded successfully!", "Save - Load Settings", MessageBoxButtons.OK, FlexibleMaterialForm.ButtonsPosition.Center);
            }
        }
        #endregion

        #region AutoLoader
        private void autoloaderSwitcher_CheckedChanged(object sender, EventArgs e)
        {
            string folderPath = @"C:\Blats-Notifier";
            string filePath = @"C:\Blats-Notifier\MyAutoLoaderSettings.txt";
            string[] loader = new string[2];
            if (autoloaderSwitcher.Checked)
            {
                autoLoad = true;
                System.IO.Directory.CreateDirectory(folderPath);
                loader[0] = "true";
                if (!File.Exists(filePath))
                {
                    var MySettings = File.Create(filePath);
                    MySettings.Close();
                    File.WriteAllLines(filePath, loader);
                }
                else
                {
                    File.WriteAllLines(filePath, loader);
                }
            }
            else
            {
                autoLoad = false;
                System.IO.Directory.CreateDirectory(folderPath);
                loader[0] = "false";
                if (!File.Exists(filePath))
                {
                    var MySettings = File.Create(filePath);
                    MySettings.Close();
                    File.WriteAllLines(filePath, loader);
                }
                else
                {
                    File.WriteAllLines(filePath, loader);
                }
            }
        }

        private void autoLoader()
        {
            string loadPath = @"C:\Blats-Notifier\MyAutoLoaderSettings.txt";
            if (!File.Exists(loadPath))
            {
                this.Show();
            }
            else
            {
                string[] MyLoadSettings = File.ReadAllLines(loadPath);
                if (MyLoadSettings[0] == "true")
                {
                    this.Show();
                    btnLoad_Click(new object(), new EventArgs());
                    autoloaderSwitcher.Checked = true;
                }
            }
        }
        #endregion






























    }
}
