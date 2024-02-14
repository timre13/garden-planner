using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace garden_planner
{
    class CanvasWrapper
    {
        private readonly Canvas canvas;
        private int fieldWidth;
        private int fieldHeight;

        private int actualFieldWidth;
        private int actualFieldHeight;

        private float ratio;
        private float heightRatio = 1;
        private float widthRatio = 1;

        public CanvasWrapper(Canvas canvas, int fieldWidth, int fieldHeight) {
            this.canvas = canvas;
            this.fieldWidth = fieldWidth;
            this.fieldHeight = fieldHeight;
            ratio = fieldWidth / fieldHeight;

            Debug.WriteLine(((float)canvas.ActualHeight / (float)fieldHeight));
            Debug.WriteLine(fieldWidth * ((float)canvas.ActualHeight / (float)fieldHeight));

            if (canvas.ActualWidth < fieldWidth)
            {
                fieldHeight = (int)(fieldHeight * ((float)canvas.ActualWidth / (float)fieldWidth));
                heightRatio = (float)canvas.ActualWidth / (float)fieldWidth;
                fieldWidth = (int)canvas.ActualWidth;
            }
            if (canvas.ActualHeight < fieldHeight)
            {
                fieldWidth = (int)(fieldWidth * ((float)canvas.ActualHeight / (float)fieldHeight));
                widthRatio = (float)canvas.ActualHeight / (float)fieldHeight;
                fieldHeight = (int)canvas.ActualHeight;
            }
            Debug.WriteLine($"{canvas.ActualWidth}x{canvas.ActualHeight}");
           


            this.canvas.Children.Clear();
            var borderRect = new Rectangle() { 
                Width = fieldWidth,
                Height = fieldHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(borderRect, 0);
            Canvas.SetTop(borderRect, 0);
            canvas.Children.Add(borderRect);
        }

        public void ClearCanvas()
        {
            canvas.Children.Clear();
            var clearRect = new Rectangle() { 
                Width = canvas.ActualWidth,
                Height = canvas.ActualHeight,
                Fill = Brushes.White,
                StrokeThickness = 1,
            };
            Canvas.SetLeft(clearRect, 0);
            Canvas.SetTop(clearRect, 0);
            canvas.Children.Add(clearRect);
        }

        public void DrawBorder(bool red = false)
        {
            var borderRect = new Rectangle() { 
                Width = canvas.ActualWidth,
                Height = canvas.ActualHeight,
                Stroke = red ? Brushes.Crimson : Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(borderRect, 0);
            Canvas.SetTop(borderRect, 0);
            canvas.Children.Add(borderRect);

        }

        public void DebugText(int x, int y, string text)
        {
            var textBlock = new TextBlock()
            {
                Text = text,
                FontSize = 10,
                Foreground = System.Windows.Media.Brushes.Black
            };
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            canvas.Children.Add(textBlock);
        }

        public void DrawPlant(in Plant plant, int x, int y)
        {
            var c1 = ((Color)ColorConverter.ConvertFromString(plant.Color));
            c1.A = 100;
            var shape1 = new Rectangle()
            {
                Width = plant.Totavv * 2,
                Height = plant.Sortavv * 2,
                Fill = new SolidColorBrush(c1),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            Canvas.SetLeft(shape1, (x - plant.Totavv));
            Canvas.SetTop(shape1, (y - plant.Sortavv));
            canvas.Children.Add(shape1);

            // var shape = new Ellipse
            // {
            //     Width = plant.Sortav * 2 ?? 10,
            //     Height = plant.Sortav * 2?? 10,
            //     Fill = new SolidColorBrush(c1),
            //     Stroke = new SolidColorBrush(Colors.Black),
            //     StrokeThickness = 1
            // };
            // Canvas.SetLeft(shape, (double)(x - plant.Sortav!));
            // Canvas.SetTop(shape, (double)(y - plant.Sortav!));
            // canvas.Children.Add(shape);

            var line1 = new Line()
            {
                X1 = x-5,
                Y1 = y-5,
                X2 = x+5,
                Y2 = y+5,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            canvas.Children.Add(line1);

            var line2 = new Line()
            {
                X1 = x - 5,
                Y1 = y + 5,
                X2 = x + 5,
                Y2 = y - 5,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            canvas.Children.Add(line2);
        }
    }
}
