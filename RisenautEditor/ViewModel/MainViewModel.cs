using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

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
        }

        private GameFile GameFile
        {
            get { return game_file; }
            set
            {
                if (Set(() => GameFile, ref game_file, value))
                {
                    RaisePropertyChanged(() => Title);
                    RaisePropertyChanged(() => Blocks);
                    RaisePropertyChanged(() => Levels);
                }
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

        public RelayCommand OpenFileCommand { get; private set; }

        private void OpenFile()
        {
            GameFile game = file_service.OpenGame();
            if (game != null)
            {
                GameFile = game;
                SelectedLevelIndex = 0;
            }
        }

        public string Title
        {
            get
            {
                if (game_file != null)
                {
                    return Path.GetFileName(game_file.FileName) + " - " + title_base;
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
    }
}
