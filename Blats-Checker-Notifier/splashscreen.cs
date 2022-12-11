﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using AutoUpdaterDotNET;

namespace Blats_Checker_Notifier
{

    public partial class splashscreen : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int RightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        

        public splashscreen()
        {
            AutoUpdater.Start("https://services.blats.gr/blats-checker-notifier/AutoUpdater.xml");
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            ProgressBar1.Value = 0;
            if (File.Exists("PresentationNative_cor3.dll"))
            {
            }
            else
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                WebClient Client = new WebClient();
                Client.DownloadFile("https://services.blats.gr/blats-checker-notifier/PresentationNative_cor3.dll", "PresentationNative_cor3.dll");
                string path2 = @"PresentationNative_cor3.dll";
                File.SetAttributes(path2, File.GetAttributes(path) | FileAttributes.Hidden);
            }
 
        }
 
        private void timer1_Tick(object sender, EventArgs e)
        {
            ProgressBar1.Value += 2;
            ProgressBar1.Text = ProgressBar1.Value.ToString() + "%";

            if(ProgressBar1.Value == 100)
            {
                timer1.Enabled = false;
                Main form = new Main();
                form.Show();
                this.Hide();
            }
        }
    }
}
