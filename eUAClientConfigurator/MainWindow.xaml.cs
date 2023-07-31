// Copyright PHOENIX CONTACT Electronics GmbH

using System;
using System.Windows;

namespace eUAClientConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Resources.Add("services", App.ServiceProvider);
            InitializeComponent();
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
    }
}
