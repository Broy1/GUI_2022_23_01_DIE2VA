using GUI_2022_23_01_DIE2VA.Interfaces;
using GUI_2022_23_01_DIE2VA.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI_2022_23_01_DIE2VA.Renderer
{
    public class Display : FrameworkElement
    {
        IGameModel model;
        Size size;

        public void SetupModel(IGameModel model)
        {
            this.model = model;
        }

        public void Resize(Size size)
        {
            this.size = size;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (model != null && size.Width > 50 && size.Height > 50)
            {
                double rectWidth = size.Width / model.GameMatrix.GetLength(1);
                double rectHeight = size.Height / model.GameMatrix.GetLength(0);

                drawingContext.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.Black, 0),
                    new Rect(0, 0, size.Width, size.Height));

                for (int i = 0; i < model.GameMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < model.GameMatrix.GetLength(1); j++)
                    {
                        ImageBrush brush = new ImageBrush();
                        switch (model.GameMatrix[i, j])
                        {
                            case GameLogic.GameItem.player:
                                brush = new ImageBrush
                                    (new BitmapImage(new Uri(Path.Combine("Images", "pika.bmp"), UriKind.RelativeOrAbsolute)));
                                break;
                            case GameLogic.GameItem.wall:
                                brush = new ImageBrush
                                    (new BitmapImage(new Uri(Path.Combine("Images", "wall.bmp"), UriKind.RelativeOrAbsolute)));
                                break;
                            case GameLogic.GameItem.crate:
                                brush = new ImageBrush
                                    (new BitmapImage(new Uri(Path.Combine("Images", "crate.bmp"), UriKind.RelativeOrAbsolute)));
                                break;
                            case GameLogic.GameItem.key:
                                brush = new ImageBrush
                                    (new BitmapImage(new Uri(Path.Combine("Images", "key.bmp"), UriKind.RelativeOrAbsolute)));
                                break;
                            case GameLogic.GameItem.floor:
                                break;
                            case GameLogic.GameItem.door:
                                brush = new ImageBrush
                                    (new BitmapImage(new Uri(Path.Combine("Images", "door.bmp"), UriKind.RelativeOrAbsolute)));
                                break;
                            default:
                                break;
                        }

                        drawingContext.DrawRectangle(brush
                                    , new Pen(Brushes.LightBlue, 0),
                                    new Rect(j * rectWidth, i * rectHeight, rectWidth, rectHeight)
                                    );
                    }
                }
            }
        }

    }
}
