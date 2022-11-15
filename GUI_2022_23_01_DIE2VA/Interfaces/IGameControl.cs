using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GUI_2022_23_01_DIE2VA.Logic.GameLogic;

namespace GUI_2022_23_01_DIE2VA.Interfaces
{
    internal interface IGameControl
    {
        void Move(Directions direction);
    }
}
