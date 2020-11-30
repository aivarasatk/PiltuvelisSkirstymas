using Microsoft.Toolkit.Mvvm.Input;

namespace PiltuvelisSkirstymas.Models
{
    public partial class MainModel : ModelBase
    {
        public MainModel()
        {

        }

        private bool _isLoading;

        private string _genFileFullPath;
        private string _operationsFileFullPath;
        private int _lineStart;

        public RelayCommand SelectGenFile { get; set; }
        public RelayCommand SelectOperationsFile { get; set; }
        public AsyncRelayCommand ExecuteExport { get; set; }
    }
}
