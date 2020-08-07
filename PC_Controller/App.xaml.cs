﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PC_Controller
{
    public partial class App : Application
    {
        public const int CompatibilityVersion = 1;
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            if (Communication.Main.socket != null)
            {
                if (Communication.Main.socket.Connected)
                {
                    Communication.Main.SendData("DISCONNECT");
                    MainPage.Navigation.PopToRootAsync();
                }
            }
        }

        protected override void OnResume()
        {
        }
    }
}
