using IO;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PiltuvelisSkirstymas.Enums;
using PiltuvelisSkirstymas.Models;
using PiltuvelisSkirstymas.Services.Mapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Serilog;

namespace PiltuvelisSkirstymas.ViewModels
{
    public class MainViewModel : ViewModelBase<MainModel>
    {
        private bool _fileSelectWasOpened;

        private readonly ILogger _logger;
        private readonly IEipReader _eipReader;
        private readonly IEipWriter _eipWriter;
        private readonly IMapper _mapper;

        public MainViewModel(ILogger logger,
            IEipReader eipReader,
            IEipWriter eipWriter,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eipReader = eipReader ?? throw new ArgumentNullException(nameof(eipReader));
            _eipWriter = eipWriter ?? throw new ArgumentNullException(nameof(eipWriter));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            InitializeModel();
        }

        private void InitializeModel()
        {
            Model.SelectGenFile = new RelayCommand(() => GenFileSelect(fileExtension: "eip"));
            Model.ExecuteExport = new AsyncRelayCommand(OnExecuteExport);
        }

        private void GenFileSelect(string fileExtension)
        {
            Model.GenFileFullPath = FileSelect(fileExtension);
            Model.GenFileName = Path.GetFileName(Model.GenFileFullPath);
        }

        private string FileSelect(string fileExtension)
        {
            Model.IsLoading = true;
            var fileSelectDialog = new OpenFileDialog();

            if (!_fileSelectWasOpened)
            {
                fileSelectDialog.InitialDirectory = Directory.GetCurrentDirectory();
                _fileSelectWasOpened = true;
            }
            
            fileSelectDialog.Filter = $"File (*.{fileExtension})|*.{fileExtension}";

            var result = fileSelectDialog.ShowDialog();

            Model.IsLoading = false;
            return result == true ? fileSelectDialog.FileName : string.Empty;
        }


        private async Task OnExecuteExport()
        {
            if (string.IsNullOrWhiteSpace(Model.GenFileFullPath) || Model.LineStart <= 0)
            {
                ShowFadingStatusBarMessage(MessageType.Error, "Yra įvesties klaidų, patikrinkite ar pasirinkti failai ir nustatytas eilutes numeris");
                return;
            }

            Model.IsLoading = true;
            try
            {
                var eipData = await _eipReader.GetParsedEipContentsAsync(Model.GenFileFullPath);
                var output = _mapper.MapToOutput(eipData.Where(e => e.LineNr >= Model.LineStart));
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
