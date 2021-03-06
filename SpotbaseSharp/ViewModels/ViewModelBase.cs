﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using Microsoft.Practices.Unity;

namespace SpotbaseSharp.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected IUnityContainer Container;

        protected ViewModelBase()
        {
            var app = (App) Application.Current;
            Container = app.Container;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            string name = this.GetPropertyNameFromExpression(property);
            RaisePropertyChanged(name);
        }
    }
}