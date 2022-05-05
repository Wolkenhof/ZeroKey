﻿using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.ServiceLocation;
using NFCLoc.UI.ViewModel.ViewModels;
using NFCLoc.UI.ViewModel.Services;
using GalaSoft.MvvmLight.Messaging;
using NFCLoc.UI.View.Views;

namespace NFCLoc.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceLocator.Current.GetInstance<ILogger>().Info("NFCLoc startup");
            Messenger.Default.Register<AboutViewModel>(this, ProcessAboutMessage);

            base.OnStartup(e);
        }
        private void ProcessAboutMessage(AboutViewModel message)
        {
            AboutWindow about = new AboutWindow();
            about.DataContext = message;
            about.ShowDialog();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.Current.GetInstance<ILogger>().Info("NFCLoc exit");

            base.OnExit(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().IsBusy = false;

            var message = e.Exception?.Message;
            if (e.Exception?.InnerException != null)
                message = $"{message}{Environment.NewLine}{e.Exception.InnerException.Message}";

            ServiceLocator.Current.GetInstance<ILogger>().Error(message);
            ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorDialog(message);

            e.Handled = true;
        }
    }
}