using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RisenautEditor.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private IFileService file_service;
        private GameFile game_file;
        private string title_base = "Risenaut Editor";
        private int selected_level_index = -1;
        private Level selected_level;

        public MainViewModel(IFileService file_service)
        {
            this.file_service = file_service;
            OpenFileCommand = new RelayCommand(OpenFile);
            SaveFileCommand = new RelayCommand(SaveFile, () => GameFile != null && GameFile.IsModified);
            SaveFileAsCommand = new RelayCommand(SaveFileAs, () => GameFile != null);
            AboutCommand = new RelayCommand(About);
        }

        private GameFile GameFile
        {
            get { return game_file; }
            set
            {
                if (value != game_file)
                {
                    if (game_file != null)
                    {
                        game_file.PropertyChanged -= GameFilePropertyChanged;
                    }
                    game_file = value;
                    if (game_file != null)
                    {
                        game_file.PropertyChanged += GameFilePropertyChanged;
                    }
                    RaisePropertyChanged(() => GameFile);
                    RaisePropertyChanged(() => Title);
                    RaisePropertyChanged(() => Blocks);
                    RaisePropertyChanged(() => Levels);
                    RaisePropertyChanged(() => IsModified);
                    SaveFileCommand.RaiseCanExecuteChanged();
                    SaveFileAsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void GameFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsModified".Equals(e.PropertyName))
            {
                RaisePropertyChanged(() => IsModified);
                RaisePropertyChanged(() => Title);
                SaveFileCommand.RaiseCanExecuteChanged();
            }
            else if ("FileName".Equals(e.PropertyName))
            {
                RaisePropertyChanged(() => Title);
            }
        }

        public string TitleBase
        {
            get { return title_base; }
            set
            {
                if (Set(() => TitleBase, ref title_base, value))
                {
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        public ICommand OpenFileCommand { get; private set; }

        private void OpenFile()
        {
            if (GameFile != null && GameFile.IsModified && !file_service.QueryGameModified(GameFile))
            {
                return;
            }
            GameFile game = file_service.OpenGame();
            if (game != null)
            {
                GameFile = game;
                SelectedLevelIndex = 0;
            }
        }

        public RelayCommand SaveFileCommand { get; private set; }

        private void SaveFile()
        {
            file_service.SaveGame(GameFile);
        }

        public RelayCommand SaveFileAsCommand { get; private set; }

        private void SaveFileAs()
        {
            file_service.SaveGameAs(GameFile);
        }

        public RelayCommand AboutCommand { get; private set; }

        private void About()
        {
            new AboutBox().ShowDialog();
        }

        public string Title
        {
            get
            {
                if (game_file != null)
                {
                    return Path.GetFileName(game_file.FileName) +
                        (IsModified ? "* - " : " - ") +
                        title_base;
                }
                return title_base;
            }
        }

        public IEnumerable<Sprite> Blocks
        {
            get
            {
                return GameFile != null ? GameFile.Blocks : Enumerable.Empty<Sprite>();
            }
        }

        public IEnumerable<Level> Levels
        {
            get
            {
                return GameFile != null ? GameFile.Levels : Enumerable.Empty<Level>();
            }
        }

        public int SelectedLevelIndex
        {
            get { return selected_level_index; }
            set
            {
                if (Set(() => SelectedLevelIndex, ref selected_level_index, value))
                {
                    if (value >= 0 && value < GameFile.Levels.Count)
                    {
                        SelectedLevel = GameFile.Levels[value];
                    }
                    else
                    {
                        SelectedLevel = null;
                    }
                }
            }
        }

        public Level SelectedLevel
        {
            get { return selected_level; }
            set
            {
                Set(() => SelectedLevel, ref selected_level, value);
            }
        }

        public bool IsModified { get { return game_file != null ? game_file.IsModified : false; } }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            if (GameFile != null && GameFile.IsModified && !file_service.QueryGameModified(GameFile))
            {
                e.Cancel = true;
            }
        }
    }
}
