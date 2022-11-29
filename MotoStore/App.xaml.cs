using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MotoStore.Models;
using MotoStore.Services;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;
using MotoStore.Services.Contracts;

using Ninject;

using TomsToolbox.Composition;
using TomsToolbox.Composition.Ninject;
using TomsToolbox.Wpf.Composition;
using TomsToolbox.Wpf.Styles;

namespace MotoStore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                // App Host
                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Window resolver service
                services.AddSingleton<IWindowService, WindowService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window container with navigation
                services.AddScoped<INavigationWindow, Views.Container>();
                services.AddScoped<ViewModels.ContainerViewModel>();

                // Views and ViewModels
                services.AddScoped<Views.Windows.LoginWindow>();
                services.AddScoped<ViewModels.LoginViewModel>();
                services.AddScoped<Views.Pages.DashboardPage>();
                services.AddScoped<ViewModels.DashboardViewModel>();
                services.AddScoped<Views.Pages.DataPage>();
                services.AddScoped<ViewModels.DataViewModel>();
                services.AddScoped<Views.Pages.SettingsPage>();
                services.AddScoped<ViewModels.SettingsViewModel>();
                services.AddScoped<Views.Pages.ReportPage>();
                services.AddScoped<ViewModels.ReportViewModel>();
                services.AddScoped<Views.Pages.IOPage>();
                services.AddScoped<ViewModels.IOViewModel>();
                services.AddScoped<Views.Pages.DataPagePages.MotoListPage>();
                services.AddScoped<ViewModels.MotoListViewModel>();
                services.AddScoped<Views.Pages.DataPagePages.EmployeeListPage>();
                services.AddScoped<ViewModels.EmployeeListViewModel>();
                services.AddScoped<Views.Pages.DataPagePages.CustomerListPage>();
                services.AddScoped<ViewModels.CustomerListViewModel>();
                services.AddScoped<Views.Pages.DataPagePages.SupplierListPage>();
                services.AddScoped<ViewModels.SupplierListViewModel>();

                // Configuration
                services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        private readonly IKernel _kernel = new StandardKernel();

        protected override void OnStartup(StartupEventArgs e)
        {
            var vCulture = new CultureInfo("vi-VN");

            Thread.CurrentThread.CurrentCulture = vCulture;
            Thread.CurrentThread.CurrentUICulture = vCulture;
            CultureInfo.DefaultThreadCurrentCulture = vCulture;
            CultureInfo.DefaultThreadCurrentUICulture = vCulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
            XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));

            base.OnStartup(e);

            // setup visual composition infrastructure, using Ninject
            _kernel.BindExports(GetType().Assembly);
            IExportProvider exportProvider = new ExportProvider(_kernel);
            _kernel.Bind<IExportProvider>().ToConstant(exportProvider);

            // setup global export provider locator for XAML
            ExportProviderLocator.Register(exportProvider);

            // register all controls tagged as data templates
            var dynamicDataTemplates = DataTemplateManager.CreateDynamicDataTemplates(exportProvider);
            Resources.MergedDictionaries.Add(dynamicDataTemplates);
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            LiveCharts.Configure(config =>
                config
                    // registers SkiaSharp as the library backend
                    .AddSkiaSharp()

                    // adds the default supported types
                    .AddDefaultMappers());

            // register mappers
            //.HasMap<City>((city, point) =>
            //{
            //    point.PrimaryValue = city.Population;
            //    point.SecondaryValue = point.Context.Index;
            //}));
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}