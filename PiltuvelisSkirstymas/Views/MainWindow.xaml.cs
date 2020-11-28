using Microsoft.Toolkit.Mvvm.DependencyInjection;
using PiltuvelisSkirstymas.IoC;
using PiltuvelisSkirstymas.ViewModels;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace PiltuvelisSkirstymas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            IocConfiguration.Configure();
            var mainViewModel = Ioc.Default.GetService<MainViewModel>();

            DataContext = mainViewModel.Model;

            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;//yields smoother window movement

            InitializeComponent();
        }
    }
}
