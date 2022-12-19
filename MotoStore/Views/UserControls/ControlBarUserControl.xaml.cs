﻿using MotoStore.ViewModels;
using System;
using System.Collections.Generic;
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

namespace MotoStore.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ControlBarUserControl.xaml
    /// </summary>
    public partial class ControlBarUserControl : UserControl
    {
        public ViewModels.ControlBarViewModel ViewModel
        {
            get;
        }

        public ControlBarUserControl(ViewModels.ControlBarViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        public ControlBarUserControl()
        {
            InitializeComponent();

            this.DataContext = ViewModel = new ControlBarViewModel();
        }
    }
}
