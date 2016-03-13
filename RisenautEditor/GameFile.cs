using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisenautEditor
{
    /// <summary>
    /// Represents a game file of type FIL.
    /// </summary>
    class GameFile
    {
        private byte[] game_data;

        private const int type_b_header_size = 0x2C;
        private const int load_addr_offset = 0x28;

        private const int levels_addr = 0x3900;
        private const int number_of_levels = 25;
        private const int level_size = 16 * 16;

        private const int blocks_addr = 0x53B5;
        private const int number_of_blocks = 8;
        private const int block_size = 4 * 8;

        public GameFile(string path)
        {
            game_data = File.ReadAllBytes(path);

            int load_addr = game_data[load_addr_offset] + game_data[load_addr_offset + 1] * 256;
            int levels_offset = levels_addr - load_addr + type_b_header_size;
            int blocks_offset = blocks_addr - load_addr + type_b_header_size;

            if (game_data.Length < blocks_offset + block_size * number_of_blocks)
                throw new Exception("File is truncated.");

            var levels = new Level[number_of_levels];
            for (int i = 0; i < number_of_levels; i++)
            {
                levels[i] = new Level(game_data, levels_offset + level_size * i, i + 1);
            }
            Levels = Array.AsReadOnly(levels);

            var blocks = new Sprite[number_of_blocks];
            for (int i = 0; i < number_of_blocks; i++)
            {
                blocks[i] = new Sprite(game_data, blocks_offset + block_size * i, 8, 8);
            }
            Blocks = Array.AsReadOnly(blocks);
        }

        public IReadOnlyList<Level> Levels { get; private set; }
        public IReadOnlyList<Sprite> Blocks { get; private set; }
    }
}
