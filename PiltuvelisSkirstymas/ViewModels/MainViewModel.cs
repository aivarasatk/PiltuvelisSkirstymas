using PiltuvelisSkirstymas.Models;
using PiltuvelisSkirstymas.Services.Logger;

namespace PiltuvelisSkirstymas.ViewModels
{
    public class MainViewModel : ViewModelBase<MainModel>
    {
        private readonly ILogService _logger;

        public MainViewModel(ILogService logger)
        {
            _logger = logger;
        }
    }
}
