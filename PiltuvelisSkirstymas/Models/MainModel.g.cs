﻿

namespace PiltuvelisSkirstymas.Models
{
    public partial class MainModel : ModelBase
    {
        public bool IsLoading 
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string GenFileName
        {
            get => _genFileName;
            set => SetProperty(ref _genFileName, value);
        }
        public string OperationsFileName
        {
            get => _operationsFileName;
            set => SetProperty(ref _operationsFileName, value);
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
