using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisenautEditor
{
    class LevelCollection
    {
        private byte[] game_data;
        private int levels_offset;

        private const int level_size = 16 * 16;

        public Level this[int index]
        {
            get
            {
                return new Level(game_data, levels_offset + level_size * index);
            }
        }
    }
}
