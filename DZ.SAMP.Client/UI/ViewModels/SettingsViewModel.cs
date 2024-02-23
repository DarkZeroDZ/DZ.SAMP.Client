using DZ.SAMP.Client.Models;
using DZ.SAMP.Client.MVVM;
using DZ.SAMP.Client.UI.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;

namespace DZ.SAMP.Client.UI.Views
{
    public class SettingsViewModel : ViewModelBase
    {
        public List<Language> Languages { get; set; }
        public Language Language
        {
            get => _language; 
            set
            {
                _language = value;

                var oldlanguage = this.VM.Settings.Language;

                this.VM.Settings.Language = value;

                this.VM.Settings.Save();

                if(oldlanguage != value)
                {
                    Process.Start(System.Windows.Application.ResourceAssembly.Location);
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }
        public MainViewModel VM { get; set; }

        public ICommand SelectFolderCommand => new RelayCommand(this.SelectFolder);


        private Language _language;

        public SettingsViewModel(MainViewModel vm)
        {
            this.VM = vm;

            this.Languages = new List<Language>
            {
                new Language("Deutsch", "de-DE"),
                new Language("English", "en-US")
            };

            this.Language = vm.Settings.Language;
        }

        private void SelectFolder()
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();

            if(!string.IsNullOrEmpty(dialog.SelectedPath))
                this.VM.Settings.SingleplayerLocation = dialog.SelectedPath;

            this.OnPropertyChanged();
        }
    }
}