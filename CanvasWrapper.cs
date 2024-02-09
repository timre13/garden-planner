using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace garden_planner
{
    class CanvasWrapper
    {
        private Canvas canvas;
        private int fieldWidth;
        private int fieldHeight;

        public CanvasWrapper(Canvas canvas, int fieldWidth, int fieldHeight) {
            this.canvas = canvas;
            this.fieldWidth = fieldWidth;
            this.fieldHeight = fieldHeight;

            this.canvas.Children.Clear();
            var borderRect = new Rectangle() { 
                Width = canvas.ActualWidth,
                Height = canvas.ActualHeight,
                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(borderRect, 0);
            Canvas.SetTop(borderRect, 0);
            canvas.Children.Add(borderRect);
        }

        public void ClearCanvas()
        {
            canvas.Children.Clear();
        }

        public void DrawPlant(in Plant plant, int x, int y)
        {
            var c1 = ((Color)ColorConverter.ConvertFromString(plant.Color));
            c1.A = 100;
            var shape1 = new Rectangle()
            {
                Width = plant.Totavv,
                Height = plant.Sortavv,
                Fill = new SolidColorBrush(c1),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            Canvas.SetLeft(shape1, (x - plant.Totavv / 2));
            Canvas.SetTop(shape1, (y - plant.Sortavv / 2));
            canvas.Children.Add(shape1);

            var shape = new Ellipse
            {
                Width = plant.Sortav ?? 10,
                Height = plant.Sortav ?? 10,
                Fill = new SolidColorBrush(c1),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            Canvas.SetLeft(shape, (double)(x - plant.Sortav! / 2));
            Canvas.SetTop(shape, (double)(y - plant.Sortav! / 2));
            canvas.Children.Add(shape);

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
