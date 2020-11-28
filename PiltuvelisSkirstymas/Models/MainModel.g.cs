

namespace PiltuvelisSkirstymas.Models
{
    public partial class MainModel : ModelBase
    {
        public bool IsLoading 
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
    }
}
