﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZeroKey.GUI.Pages
{    
    /// <summary>
    /// Interaktionslogik für ManageCards.xaml
    /// </summary>
    public partial class ManageCards : Page
    {
        public class DataObject
        {
            public DataObject()
            {
                Cards = new List<string>();
                Cards.Add("One");
                Cards.Add("Two");
                Cards.Add("Three");
                Cards.Add("Three");
                Cards.Add("Three");
                Cards.Add("Three");
                Cards.Add("Three");
                Cards.Add("Three");
                Cards.Add("Three");
                Cards.Add("Three");
                Cards.Add("Three");
            }

            public IList<string> Cards { get; set; }
        }

        public class Cards
        {
            private string _nameValue;

            public string Name
            {
                get { return _nameValue; }
                set { _nameValue = value; }
            }

            private double _idValue;

            public double Id
            {
                get { return _idValue; }

                set
                {
                    if (value != _idValue)
                    {
                        _idValue = value;
                    }
                }
            }
        }

        public ManageCards()
        {
            InitializeComponent();
            DataContext = new DataObject();
        }
    }
}