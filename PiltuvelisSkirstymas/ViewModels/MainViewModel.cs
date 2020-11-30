using IO;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PiltuvelisSkirstymas.Enums;
using PiltuvelisSkirstymas.Models;
using PiltuvelisSkirstymas.Services;
using PiltuvelisSkirstymas.Services.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PiltuvelisSkirstymas.ViewModels
{
    public class MainViewModel : ViewModelBase<MainModel>
    {
        private readonly ILogService _logger;
        private readonly IEipReader _eipReader;
        private readonly IEipWriter _eipWriter;

        public MainViewModel(ILogService logger,
            IEipReader eipReader,
            IEipWriter eipWriter)
        {
            _logger = logger;
            _eipReader = eipReader;
            _eipWriter = eipWriter;

            InitializeModel();
        }

        private void InitializeModel()
        {
            Model.SelectGenFile = new RelayCommand(() => GenFileSelect(fileExtension: "eip"));
            Model.SelectOperationsFile = new RelayCommand(() => OperationsFileSelect(fileExtension: "xlsx"));
            Model.ExecuteExport = new AsyncRelayCommand(OnExecuteExport);
        }

        private void GenFileSelect(string fileExtension) 
            => Model.GenFileFullPath = FileSelect(fileExtension);

        private void OperationsFileSelect(string fileExtension)
            => Model.OperationsFileFullPath = FileSelect(fileExtension);

        private string FileSelect(string fileExtension)
        {
            Model.IsLoading = true;
            var fileSelectDialog = new OpenFileDialog();
            fileSelectDialog.Filter = $"File (*.{fileExtension})|*.{fileExtension}";

            var result = fileSelectDialog.ShowDialog();

            Model.IsLoading = false;
            return result == true ? fileSelectDialog.FileName : string.Empty;
        }


        private async Task OnExecuteExport()
        {
            if (string.IsNullOrWhiteSpace(Model.GenFileFullPath)
                || string.IsNullOrWhiteSpace(Model.OperationsFileFullPath)
                || Model.LineStart <= 0)
            {
                ShowFadingStatusBarMessage(MessageType.Error, "Yra įvesties klaidų, patikrinkite ar pasirinkti failai ir nustatytas eilutes numeris");
                return;
            }

            Model.IsLoading = true;
            try
            {
                var eipData = await _eipReader.GetParsedEipContentsAsync(Model.GenFileFullPath);
                var output = EipTransformer.ToOutput(eipData, Model.LineStart);
                await _eipWriter.WriteAsync(new IO.Dto.I06Output(output.ToArray()));

                ShowFadingStatusBarMessage(MessageType.Information, "Baigta. Sugeneruotas failas išvesties aplanke");
            }
            catch(Exception ex)
            {
                _logger.Error("Failed to generate eip export", ex);
                ShowFadingStatusBarMessage(MessageType.Error, $"Klaida vykdant exportą: {ex.Message}");
            }

            Model.IsLoading = false;

        }
    }
}
