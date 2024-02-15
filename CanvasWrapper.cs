using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Rectangle = System.Windows.Shapes.Rectangle;

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

        public CanvasWrapper(Canvas canvas, int localFieldWidth, int localFieldHeight) {
            this.canvas = canvas;
            this.fieldWidth = localFieldWidth;
            this.fieldHeight = localFieldHeight;
            actualFieldWidth = localFieldWidth;
            actualFieldHeight = localFieldHeight;
            ratio = fieldWidth / fieldHeight;

            Debug.WriteLine(((float)canvas.ActualHeight / (float)fieldHeight));
            Debug.WriteLine(fieldWidth * ((float)canvas.ActualHeight / (float)fieldHeight));

            if (canvas.ActualWidth < fieldWidth)
            {
                fieldHeight = (int)(fieldHeight * ((float)canvas.ActualWidth / (float)fieldWidth));
                // heightRatio = (float)canvas.ActualWidth / (float)fieldWidth;
                fieldWidth = (int)canvas.ActualWidth;
                widthRatio = (float)fieldHeight / (float)canvas.ActualHeight;
                heightRatio = (float)canvas.ActualWidth / (float)localFieldWidth;
            }
            if (canvas.ActualHeight < fieldHeight)
            {
                fieldWidth = (int)(fieldWidth * ((float)canvas.ActualHeight / (float)fieldHeight));
                // widthRatio = (float)canvas.ActualHeight / (float)fieldHeight;
                // heightRatio = (float)canvas.ActualWidth / (float)fieldWidth;
                fieldHeight = (int)canvas.ActualHeight;
                widthRatio = (float)canvas.ActualHeight / (float)localFieldHeight;
                heightRatio = (float)fieldWidth / (float)canvas.ActualWidth;
            }
            Debug.WriteLine($"{canvas.ActualWidth}x{canvas.ActualHeight}");
           


            // this.canvas.Children.Clear();
            // var borderRect = new Rectangle() { 
            //     Width = this.fieldWidth,
            //     Height = this.fieldHeight,
            //     Stroke = Brushes.Black,
            //     StrokeThickness = 1
            // };
            // Canvas.SetLeft(borderRect, 0);
            // Canvas.SetTop(borderRect, 0);
            // canvas.Children.Add(borderRect);
            
        }

        public void ClearCanvas()
        {
            canvas.Children.Clear();
            var width = new TextBlock()
            {
                Text = actualFieldWidth.ToString(),
                FontSize = 10,
                Foreground = Brushes.Black
            };
            width.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(width, fieldWidth - width.DesiredSize.Width - 2);
            Canvas.SetTop(width, fieldHeight - width.DesiredSize.Height - 2);
            canvas.Children.Add(width);
            
            var height = new TextBlock()
            {
                Text = actualFieldHeight.ToString(),
                FontSize = 10,
                Foreground = Brushes.Black,
                RenderTransform = new RotateTransform(-90),
                RenderTransformOrigin = new System.Windows.Point(0, 0)
            };
            height.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(height, fieldWidth - height.DesiredSize.Height  - 2);
            Canvas.SetTop(height, fieldHeight - height.DesiredSize.Width - 2);
            canvas.Children.Add(height);
        }

        public void DrawBorder(bool red = false)
        {
            var borderRect = new Rectangle() { 
                Width = fieldWidth,
                Height = fieldHeight,
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
                Width = plant.Totavv * 2 * widthRatio,
                Height = plant.Sortavv * 2 * heightRatio,
                Fill = new SolidColorBrush(c1),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            Canvas.SetLeft(shape1, (x - plant.Totavv) * widthRatio);
            Canvas.SetTop(shape1, (y - plant.Sortavv) * heightRatio);
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
                X1 = (x * widthRatio) - 5 * widthRatio,
                Y1 = (y * heightRatio) - 5 * heightRatio,
                X2 = (x * widthRatio) + 5 * widthRatio,
                Y2 = (y * heightRatio) + 5 * heightRatio,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            canvas.Children.Add(line1);

            var line2 = new Line()
            {
                X1 = (x * widthRatio) + 5 * widthRatio,
                Y1 = (y * heightRatio) - 5 * heightRatio,
                X2 = (x * widthRatio) - 5 * widthRatio,
                Y2 = (y * heightRatio) + 5 * heightRatio,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            canvas.Children.Add(line2);
        }
    }
}
