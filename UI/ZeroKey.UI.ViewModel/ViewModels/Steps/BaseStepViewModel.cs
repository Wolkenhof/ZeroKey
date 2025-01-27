﻿using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;

namespace ZeroKey.UI.ViewModel.ViewModels
{
    public abstract class BaseStepViewModel : ViewModelBase, IStepViewModel, IInitializeAsync
    {
        public abstract int Index { get; }
        public virtual bool CancelIsVisible => true;
        public virtual bool NextIsVisible => true;
        public virtual string NextText => (string)Application.Current.FindResource("next");
        public virtual Func<Task<bool>> NextAction => null;
        public virtual Action CancelAction => null;
        public Action ToNext { get; set; }
        public NewRingViewModel NewRingViewModel { get; set; }

        public virtual async Task InitializeAsync()
        {
            await Task.Yield();
        }
    }
}