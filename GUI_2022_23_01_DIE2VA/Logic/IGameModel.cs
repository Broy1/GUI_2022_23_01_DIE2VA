using static GUI_2022_23_01_DIE2VA.Logic.GameLogic;

namespace GUI_2022_23_01_DIE2VA.Logic
{
    public interface IGameModel
    {
        GameItem[,] GameMatrix { get; set; }
    }
}