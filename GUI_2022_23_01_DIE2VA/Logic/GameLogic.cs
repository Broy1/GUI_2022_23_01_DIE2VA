using GUI_2022_23_01_DIE2VA.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_2022_23_01_DIE2VA.Logic
{
    public class GameLogic : IGameModel, IGameControl
    {
        public enum GameItem
        {
            player, wall, floor, door, key, crate
        }

        public enum Directions
        {
            left, right, up, down
        }

        public GameItem[,] GameMatrix { get; set; }

        private Queue<string> levels;

        public GameLogic()
        {
            levels = new Queue<string>();
            var lvls = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Levels"),
                "*.lvl");

            foreach (var item in lvls)
            {
                levels.Enqueue(item);
            }
            LoadNext(levels.Dequeue());
        }

        public void Move(Directions direction)
        {
            var coords = WhereAmI();
            int i = coords[0];
            int j = coords[1];

            int old_i = i;
            int old_j = j;

            // moving
            switch (direction)
            {
                case Directions.left:
                    if (j - 1 >= 0)
                    {
                        j--;
                    }
                    break;
                case Directions.right:
                    if (j + 1 < GameMatrix.GetLength(1))
                    {
                        j++;
                    }
                    break;
                case Directions.up:
                    if (i - 1 >= 0)
                    {
                        i--;
                    }
                    break;
                case Directions.down:
                    if (i + 1 < GameMatrix.GetLength(0))
                    {
                        i++;
                    }
                    break;
                default:
                    break;
            }

            // if floor is the next block
            if (GameMatrix[i, j] == GameItem.floor)
            {
                GameMatrix[i, j] = GameItem.player;
                GameMatrix[old_i, old_j] = GameItem.floor;
            }

            // if key is the next block
            else if (GameMatrix[i, j] == GameItem.key)
            {
                // pushing to the right
                if (direction == Directions.right)
                {
                    GameMatrix[i, j] = GameItem.player;
                    GameMatrix[old_i, old_j] = GameItem.floor;
                    GameMatrix[i, j + 1] = GameItem.key;

                    // if the new place under the key is empty it will fall
                    if (GameMatrix[i + 1, j + 1] == GameItem.floor)
                    {
                        Fall(i, j, GameItem.key, Directions.right);
                    }
                }
                // pushing to the left
                else if (direction == Directions.left)
                {
                    GameMatrix[i, j] = GameItem.player;
                    GameMatrix[old_i, old_j] = GameItem.floor;
                    GameMatrix[i, j - 1] = GameItem.key;

                    // if the new place under the key is empty it will fall
                    if (GameMatrix[i + 1, j - 1] == GameItem.floor)
                    {
                        Fall(i, j, GameItem.key, Directions.left);
                    }
                }
            }

            // if crate is the next block
            else if (GameMatrix[i, j] == GameItem.crate)
            {
                // pushing to the right
                if (direction == Directions.right)
                {
                    GameMatrix[i, j] = GameItem.player;
                    GameMatrix[old_i, old_j] = GameItem.floor;
                    GameMatrix[i, j + 1] = GameItem.crate;

                    // if the new place under the key is empty it will fall
                    if (GameMatrix[i + 1, j + 1] == GameItem.floor)
                    {
                        Fall(i, j, GameItem.crate, Directions.right);
                    }
                }
                // pushing to the left
                else if (direction == Directions.left)
                {
                    GameMatrix[i, j] = GameItem.player;
                    GameMatrix[old_i, old_j] = GameItem.floor;
                    GameMatrix[i, j - 1] = GameItem.crate;

                    // if the new place under the key is empty it will fall
                    if (GameMatrix[i + 1, j - 1] == GameItem.floor)
                    {
                        Fall(i, j, GameItem.crate, Directions.left);
                    }
                }
            }

            // entering next level through the door
            else if (GameMatrix[i, j] == GameItem.door)
            {
                if (levels.Count > 0)
                {
                    LoadNext(levels.Dequeue());
                }
            }
        }

        private void Fall(int i, int j, GameItem item, Directions direction)
        {
            if (direction == Directions.right)
            {
                while (GameMatrix[i + 1, j + 1] == GameItem.floor)
                {
                    GameMatrix[i, j + 1] = GameItem.floor;
                    GameMatrix[i + 1, j + 1] = item;
                    i++;
                }
            }
            else if (direction == Directions.left)
            {
                while (GameMatrix[i + 1, j - 1] == GameItem.floor)
                {
                    GameMatrix[i, j - 1] = GameItem.floor;
                    GameMatrix[i + 1, j - 1] = item;
                    i++;
                }
            }
        }

        private int[] WhereAmI()
        {
            for (int i = 0; i < GameMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < GameMatrix.GetLength(1); j++)
                {
                    if (GameMatrix[i, j] == GameItem.player)
                    {
                        return new int[] { i, j };
                    }
                }
            }
            return new int[] { -1, -1 };
        }

        public void LoadNext(string path)
        {
            string[] lines = File.ReadAllLines(path);
            GameMatrix = new GameItem[int.Parse(lines[1]), int.Parse(lines[0])];
            for (int i = 0; i < GameMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < GameMatrix.GetLength(1); j++)
                {
                    GameMatrix[i, j] = ConvertToEnum(lines[i + 2][j]);
                }
            }
        }

        private GameItem ConvertToEnum(char v)
        {
            switch (v)
            {
                case 'w': return GameItem.wall;
                case 'p': return GameItem.player;
                case 'd': return GameItem.door;
                case 'k': return GameItem.key;
                case 'c': return GameItem.crate;
                default:
                    return GameItem.floor;
            }
        }
    }
}
