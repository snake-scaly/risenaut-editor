using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisenautEditor
{
    /// <summary>
    /// Represents a game file of type FIL.
    /// </summary>
    class GameFile : ObservableObject
    {
        private byte[] game_data;
        private string file_name;

        private const int type_b_header_size = 0x2C;
        private const int load_addr_offset = 0x28;

        private const int levels_addr = 0x3900;
        private const int number_of_levels = 16;
        private const int level_size = 16 * 16;

        private const int blocks_addr = 0x53B5;
        private const int number_of_blocks = 7;
        private const int block_size = 4 * 8;

        private const int monster_addr = 0x5983;
        private const int monster_code = 9;

        public GameFile(string path)
        {
            FileName = path;

            game_data = File.ReadAllBytes(path);

            int load_addr = game_data[load_addr_offset] + game_data[load_addr_offset + 1] * 256;
            int levels_offset = levels_addr - load_addr + type_b_header_size;
            int blocks_offset = blocks_addr - load_addr + type_b_header_size;
            int monster_offset = monster_addr - load_addr + type_b_header_size;

            if (game_data.Length < blocks_offset + block_size * number_of_blocks)
                throw new Exception("File is truncated.");

            var levels = new Level[number_of_levels];
            for (int i = 0; i < number_of_levels; i++)
            {
                levels[i] = new Level(game_data, levels_offset + level_size * i, i + 1);
                levels[i].PropertyChanged += OnLevelPropertyChanged;
            }
            Levels = Array.AsReadOnly(levels);

            var blocks = new Sprite[number_of_blocks + 1];
            for (int i = 0; i < number_of_blocks; i++)
            {
                blocks[i] = new Sprite(game_data, blocks_offset + block_size * i, 8, 8, i);
            }
            blocks[number_of_blocks] = new Sprite(game_data, monster_offset, 8, 8, monster_code);
            Blocks = Array.AsReadOnly(blocks);
        }

        public void Save(string path)
        {
            File.WriteAllBytes(path, game_data);
            FileName = path;
            foreach (Level level in Levels)
            {
                level.SetUnchanged();
            }
        }

        private void OnLevelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsModified".Equals(e.PropertyName))
            {
                RaisePropertyChanged(() => IsModified);
            }
        }

        public string FileName
        {
            get { return file_name; }
            set { Set(() => FileName, ref file_name, value); }
        }

        public IList<Level> Levels { get; private set; }
        public IList<Sprite> Blocks { get; private set; }

        public bool IsModified
        {
            get
            {
                foreach (var l in Levels)
                {
                    if (l.IsModified)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
