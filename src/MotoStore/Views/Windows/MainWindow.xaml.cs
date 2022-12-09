using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace MotoStore.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        public ViewModels.MainWindowViewModel ViewModel
        {
            get;
        }
        public MainWindow()
        {
            
            InitializeComponent();
            
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService)
        {
            
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
            SetPageService(pageService);

            navigationService.SetNavigationControl(RootNavigation);
            //LoginView Lgv = new LoginView();
            //Lgv.Show();
            
        }

        #region INavigationWindow methods

        public Frame GetFrame()
            => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
            => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        public void HideWindow()
            => Hide();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}