// Copyright PHOENIX CONTACT Electronics GmbH

using Arp.OpcUA.Client;
using Arp.OpcUA.ClientConfiguration;
using Arp.OpcUA.Core;
using Arp.OpcUA.ServerCatalog;
using Arp.OpcUA.ServerRepository;
using Arp.OpcUA.UI.Core;
using eUAClientConfigurator.Services.BrowseForFile;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Windows;

namespace eUAClientConfigurator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider? ServiceProvider { get; set; }
        protected override async void OnStartup(StartupEventArgs e)
        {
            // for the open file dialog 
            System.Threading.Thread.CurrentThread.SetApartmentState(System.Threading.ApartmentState.STA);
            SetupExceptionHandler();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            await ServiceProvider.GetRequiredService<IServerCatalogService>().StartAsync();
            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
        }

        private static void SetupExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
            {
#if DEBUG
                MessageBox.Show(error.ExceptionObject.ToString(), caption: "Error");
#else
                MessageBox.Show("An error has occurred.", caption: "Error");
#endif
            };
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            //Use Cases
            services.AddServerCatalog();
            services.AddClientConfiguration();

            //Infrastructure
            services.AddUAClient();
            services.AddServerRepository();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

            //UI
            services.AddWpfBlazorWebView();
            services.AddUICore();
            services.AddSingleton<IBrowseForFileService, BrowseForFileService>();

        }
    }
}
