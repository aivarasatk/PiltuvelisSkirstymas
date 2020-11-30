

namespace PiltuvelisSkirstymas.Models
{
    public partial class MainModel : ModelBase
    {
        public bool IsLoading 
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string GenFileFullPath
        {
            get => _genFileFullPath;
            set => SetProperty(ref _genFileFullPath, value);
        }

        public string OperationsFileFullPath
        {
            get => _operationsFileFullPath;
            set => SetProperty(ref _operationsFileFullPath, value);
        }

        public int LineStart
        {
            get => _lineStart;
            set => SetProperty(ref _lineStart, value);
        }
    }
}
