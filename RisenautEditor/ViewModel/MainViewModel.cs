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
        private int tile_brush = 1;
        private int selected_tile_index = -1;

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
                    if (TileBrush < Blocks.Count())
                    {
                        SelectedTileIndex = TileBrush;
                    }
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

        public IList<Sprite> Blocks
        {
            get
            {
                return GameFile != null ? GameFile.Blocks : new Sprite[0];
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

        public int TileBrush
        {
            get { return tile_brush; }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Must be 0..255");
                }
                if (Set(() => TileBrush, ref tile_brush, value))
                {
                    RaisePropertyChanged(() => TileBrushHex);
                    int index = -1;
                    for (int i = 0; i < Blocks.Count; i++)
                    {
                        if (Blocks[i].Index == value)
                        {
                            index = i;
                        }
                    }
                    SelectedTileIndex = index;
                }
            }
        }

        public string TileBrushHex
        {
            get { return string.Format("{0:X}", tile_brush); }
            set { TileBrush = Convert.ToInt32(value, 16); }
        }

        public int SelectedTileIndex
        {
            get { return selected_tile_index; }
            set
            {
                if (Set(() => SelectedTileIndex, ref selected_tile_index, value) && value >= 0 && value <= 255)
                {
                    TileBrush = Blocks[value].Index;
                }
            }
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            if (GameFile != null && GameFile.IsModified && !file_service.QueryGameModified(GameFile))
            {
                e.Cancel = true;
            }
        }
    }
}
