﻿using Meziantou.Framework.Win32;
using NFCLoc.UI.ViewModel.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NFCLoc.UI.View.Views
{
    /// <summary>
    /// Interaction logic for MedatixxManager.xaml
    /// </summary>
    public partial class MedatixxManager : Window
    {
        private NFCReader NFC = new NFCReader();

        // Get appdata folder
        private static string AppDataPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NFCLoc");
        private static string ListFile = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "NFCLoc", "idlist.cfg");

        public MedatixxManager()
        {
            InitializeComponent();
            SetCurrentUserDatabase();

            // Stop Windows Service (NFCLoc)
            try
            {
                ServiceController service = new ServiceController("NFCLocService");
                if (service.Status.Equals(ServiceControllerStatus.Running) || service.Status.Equals(ServiceControllerStatus.StartPending))
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }

            }
            catch { MessageBox.Show((string)Application.Current.FindResource("cannot_stop_service")); }
        }

        private void SetCurrentUserDatabase()
        {
            if (!File.Exists(ListFile))
            {
                try { Directory.CreateDirectory(AppDataPath); File.WriteAllText(ListFile, ""); }
                catch { MessageBox.Show("Cannot create file " + ListFile); Environment.Exit(1); }
            }

            string listpath = File.ReadAllText(ListFile);
            listBox.Items.Clear();

            // Write every line into listBox
            foreach (string line in listpath.Split('\n'))
            {
                if (line.Trim() != "")
                {
                    listBox.Items.Add(line);
                }
            }
        }

        private void RegisterCard(object sender, RoutedEventArgs e)
        {
            if (username.Text.Trim() == "" || password.Text.Trim() == "" || cID.Text.Trim() == "")
            {
                MessageBox.Show((string)Application.Current.FindResource("data_empty"));
                return;
            }
            
            foreach (string line in File.ReadLines(ListFile))
            {
                if (line.Contains(cID.Text))
                {
                    MessageBox.Show((string)Application.Current.FindResource("already_registered"));
                    return;
                }
            }

            // Register credentials in Windows Database
            string appName = $"NFCLoc_{cID.Text}";

            CredentialManager.WriteCredential(
                applicationName: appName,
                userName: username.Text,
                secret: password.Text,
                persistence: CredentialPersistence.LocalMachine);


            StreamWriter file = new StreamWriter(ListFile, append: true);
            file.WriteLine($"{username.Text} | {cID.Text}");
            file.Close();

            SetCurrentUserDatabase();
        }

        private void RemoveCard(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
            {
                MessageBox.Show((string)Application.Current.FindResource("no_card_selected"));
                return;
            }
            string tmpUsername = listBox.SelectedItem.ToString().Split('|')[0];
            string Username = tmpUsername.Remove(tmpUsername.Length - 1);
            string cid = listBox.SelectedItem.ToString().Split('|')[1].Trim();

            var tempFile = System.IO.Path.GetTempFileName();
            var linesToKeep = File.ReadLines(ListFile).Where(l => l != $"{Username} | {cid}");

            // Remove credentials from Windows Database
            CredentialManager.DeleteCredential(applicationName: $"NFCLoc_{cid}");

            File.WriteAllLines(tempFile, linesToKeep);
            File.Delete(ListFile);
            File.Move(tempFile, ListFile);

            // Refresh
            SetCurrentUserDatabase();
        }

        private void ReadCard(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NFC.Connect())
                {
                    string id = NFC.GetCardUID();
                    string id2 = id.ToUpper();
                    cID.Text = $"{id2}9000";
                }
                else
                {
                    MessageBox.Show((string)Application.Current.FindResource("connection_failed"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                try
                {
                    string tmpUsername = listBox.SelectedItem.ToString().Split('|')[0];
                    string Username = tmpUsername.Remove(tmpUsername.Length - 1);
                    string cid = listBox.SelectedItem.ToString().Split('|')[1].Trim();
                    var cred = CredentialManager.ReadCredential(applicationName: $"NFCLoc_{cid}");

                    username.Text = Username;
                    cID.Text = cid;
                }
                catch {;}
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Start Windows Service (NFCLoc)
                ServiceController service = new ServiceController("NFCLocService");
                if (service.Status.Equals(ServiceControllerStatus.Stopped) || service.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            catch { MessageBox.Show((string)Application.Current.FindResource("cannot_start_service")); }
        }
    }
}