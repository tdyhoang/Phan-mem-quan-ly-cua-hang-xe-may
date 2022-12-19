using System.Windows.Controls;
using System;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataPage : INavigableView<ViewModels.DataViewModel>
    {
        public ViewModels.DataViewModel ViewModel
        {
            get;
        }

        public DataPage(ViewModels.DataViewModel viewModel, IPageService pageService, INavigationService navigationService)
        {
            ViewModel = viewModel;

            InitializeComponent();

            SetPageService(pageService);

            navigationService.SetNavigationControl(DataNavigation);
        }

        #region INavigationWindow methods

        public Frame GetFrame()
            => DataFrame;

        public INavigation GetNavigation()
            => DataNavigation;

        public bool Navigate(Type pageType)
            => DataNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => DataNavigation.PageService = pageService;

        #endregion INavigationWindow methods
    }
}
