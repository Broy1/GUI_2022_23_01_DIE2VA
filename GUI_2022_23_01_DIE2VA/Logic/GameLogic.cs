using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GUI_2022_23_01_DIE2VA.Logic
{
    public class GameLogic : IGameModel, IGameControl
    {
        public enum GameItem
        {
            player,wall,floor,door,key,crate,sand
        }

        public enum Directions
        {
            left,right,up,down
        }

        public GameItem[,] GameMatrix { get; set; }

        private Queue<string> levels;

        private int KeysCollected { get; set; }

        private int LevelCount { get; set; }

        private ICollection<TimeSpan> highscores;

        private Stopwatch sw;

        public GameLogic()
        {
            sw = new Stopwatch();
            highscores = new List<TimeSpan>();
            levels = new Queue<string>();
            LevelCount = 1;
            var lvls = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Levels"),
                "*.lvl");

            foreach (var item in lvls)
            {
                levels.Enqueue(item);
            }
            sw.Start();
            LoadNext(levels.Dequeue());
            LevelCount++;
        }

        public void Move(Directions direction)
        {
            // sets the coordinates to the next step and checks the type of the next block
            var coords = WhereAmI();
            int next_i = coords[0];
            int next_j = coords[1];

            int i = next_i;
            int j = next_j;

            //ActivateGravity();

            // moving
            switch (direction)
            {
                case Directions.left:
                    if (next_j - 1 >= 0)
                    {
                        next_j--;
                    }
                    break;
                case Directions.right:
                    if (next_j + 1 < GameMatrix.GetLength(1))
                    {
                        next_j++;
                    }
                    break;
                case Directions.up:
                    if (next_i - 1 >= 0)
                    {
                        next_i--;
                    }
                    break;
                case Directions.down:
                    if (next_i + 1 < GameMatrix.GetLength(0))
                    {
                        next_i++;
                    }
                    break;
                default:
                    break;
            }

            // if floor is the next block
            if (GameMatrix[next_i, next_j] == GameItem.floor)
            {
                GameMatrix[next_i, next_j] = GameItem.player;
                GameMatrix[i, j] = GameItem.floor;

                //if (GameMatrix[i - 1, j] != GameItem.floor
                //&& GameMatrix[i - 1, j] != GameItem.wall)
                //{
                //    ;
                //    GameItem tmp = GameMatrix[i - 1, j];
                //    Fall(i - 1, j, tmp, direction);
                //}
            }

            // if key is the next block
            else if (GameMatrix[next_i, next_j] == GameItem.key)
            {
                // if the place is empty after the key you can push it
                if (GameMatrix[next_i, next_j + 1] == GameItem.floor || GameMatrix[next_i, next_j - 1] == GameItem.floor)
                {
                    // pushing to the right
                    if (direction == Directions.right)
                    {
                        GameMatrix[next_i, next_j] = GameItem.player;
                        GameMatrix[i, j] = GameItem.floor;
                        GameMatrix[next_i, next_j + 1] = GameItem.key;

                        // if the new place under the key is empty it will fall
                        if (GameMatrix[next_i + 1, next_j + 1] == GameItem.floor)
                        {
                            Fall(next_i, next_j, GameItem.key, Directions.right);
                        }
                    }
                    // pushing to the left
                    else if (direction == Directions.left)
                    {
                        GameMatrix[next_i, next_j] = GameItem.player;
                        GameMatrix[i, j] = GameItem.floor;
                        GameMatrix[next_i, next_j - 1] = GameItem.key;

                        // if the new place under the key is empty it will fall
                        if (GameMatrix[next_i + 1, next_j - 1] == GameItem.floor)
                        {
                            Fall(next_i, next_j, GameItem.key, Directions.left);
                        }
                    }
                }

                // check if under the key is the door
                if (GameMatrix[next_i + 1, next_j + 1] == GameItem.door)
                {
                    GameMatrix[next_i, next_j + 1] = GameItem.floor;
                    KeysCollected++;
                }
                //if (GameMatrix[next_i + 1, next_j - 1] == GameItem.door)
                //{
                //    GameMatrix[next_i, next_j - 1] = GameItem.floor;
                //    KeysCollected++;
                //}
            }

            // if crate is the next block
            else if (GameMatrix[next_i, next_j] == GameItem.crate)
            {
                // if the place is empty after the key you can push it
                if (GameMatrix[next_i, next_j + 1] == GameItem.floor || GameMatrix[next_i, next_j - 1] == GameItem.floor)
                {
                    // pushing to the right
                    if (direction == Directions.right)
                    {
                        GameMatrix[next_i, next_j] = GameItem.player;
                        GameMatrix[i, j] = GameItem.floor;
                        GameMatrix[next_i, next_j + 1] = GameItem.crate;

                        // if the new place under the key is empty it will fall
                        if (GameMatrix[next_i + 1, next_j + 1] == GameItem.floor)
                        {
                            Fall(next_i, next_j, GameItem.crate, Directions.right);
                        }
                    }
                    // pushing to the left
                    else if (direction == Directions.left)
                    {
                        GameMatrix[next_i, next_j] = GameItem.player;
                        GameMatrix[i, j] = GameItem.floor;
                        GameMatrix[next_i, next_j - 1] = GameItem.crate;

                        // if the new place under the key is empty it will fall
                        if (GameMatrix[next_i + 1, next_j - 1] == GameItem.floor)
                        {
                            Fall(next_i, next_j, GameItem.crate, Directions.left);
                        }
                    }
                }
            }

            // sand disappears
            else if (GameMatrix[next_i,next_j] == GameItem.sand)
            {
                GameMatrix[next_i, next_j] = GameItem.player;
                GameMatrix[i, j] = GameItem.floor;
            }

            // entering next level through the door
            else if (GameMatrix[next_i, next_j] == GameItem.door)
            {
                // loading next level, saving current time
                if (levels.Count > 0 && KeysCollected == LevelCount)
                {
                    TimeSpan ts = sw.Elapsed;
                    MessageBox.Show($"Level complete! Your current time: {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}", "Keep going!", MessageBoxButton.OK);
                    KeysCollected = 0;
                    LevelCount++;
                    LoadNext(levels.Dequeue());
                }
                else
                {
                    if (KeysCollected == LevelCount)
                    {
                        // end of the adventure
                        sw.Stop();
                        TimeSpan comptime = sw.Elapsed;
                        MessageBox.Show($"Well done! You caught them all! Your time: {comptime}","Congratulations!", MessageBoxButton.OK);
                        this.SaveHighScore(comptime);
                    }
                    else
                    {
                        MessageBox.Show($"You need to collect {LevelCount - KeysCollected} more Pokeballs!", "Not enough Pokeballs!");

                    }
                }
            }

        }

        public void SaveHighScore(TimeSpan time)
        {
            if (File.Exists("./highscore.json"))
            {
                this.highscores = this.LoadHighscore();
                this.highscores.Add(time);
                this.highscores = this.highscores.OrderBy(t => t.TotalSeconds).ToList();
            }
            else
            {
                this.highscores.Add(time);
            }
            string highscore = JsonConvert.SerializeObject(this.highscores);
            File.WriteAllText("./highscore.json", highscore);
        }

        private ICollection<TimeSpan> LoadHighscore()
        {
            this.highscores = JsonConvert.DeserializeObject<IList<TimeSpan>>(File.ReadAllText("./highscore.json"));
            return this.highscores;
        }

        public void ActivateGravity()
        {
            for (int i = 0; i < GameMatrix.GetLength(1); i++)
            {
                for (int j = 0; j < GameMatrix.GetLength(0); j++)
                {
                    GameItem item = GameMatrix[i, j];
                    if (item == GameItem.crate ||
                        item == GameItem.key)
                    {
                        if (GameMatrix[i + 1, j] == GameItem.floor )
                        {
                            FreeFall(i,j,item);
                        }
                    }
                }
            }
        }

        private void FreeFall(int i, int j, GameItem item)
        {
            while (GameMatrix[i + 1, j] == GameItem.floor)
            {
                GameMatrix[i + 1, j] = item;
                GameMatrix[i, j] = GameItem.floor;
                i++;
                if (i == 19)
                {
                    break;
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
                case 's': return GameItem.sand;
                default:
                    return GameItem.floor;
            }
        }
    }
}
