using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel.Models
{
    // All the objects that can interact are GameItems.
    public abstract class GameItem
    {
        public int X_Row { get; set; }
        public int Y_Column { get; set; }

        public bool Overlaps(GameItem othergameitem)
        {
            return this.X_Row == othergameitem.X_Row && this.Y_Column == othergameitem.Y_Column;
        }
    }
}
