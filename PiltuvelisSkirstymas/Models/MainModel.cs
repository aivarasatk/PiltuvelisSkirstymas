using Microsoft.Toolkit.Mvvm.Input;

namespace PiltuvelisSkirstymas.Models
{
    public partial class MainModel : ModelBase
    {
        public MainModel()
        {

        }

        private bool _isLoading;
        public RelayCommand Ok { get; set; }
    }
}
