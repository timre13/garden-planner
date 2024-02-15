using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace garden_planner
{
    class CanvasWrapper
    {
        private Canvas canvas;
        private int fieldWidth;
        private int fieldHeight;
        private Action<Plant> plantClickCallback;

        public CanvasWrapper(Canvas canvas, int fieldWidth, int fieldHeight, Action<Plant> plantClickCallback) {
            this.canvas = canvas;
            this.fieldWidth = fieldWidth;
            this.fieldHeight = fieldHeight;
            this.plantClickCallback = plantClickCallback;

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
            var clearRect = new Rectangle() { 
                Width = canvas.ActualWidth,
                Height = canvas.ActualHeight,
                Fill = System.Windows.Media.Brushes.White,
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

        public void DrawPlant(Plant plant, int x, int y)
        {
#if false
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

            var shape = new Ellipse
            {
                Width = plant.Sortav * 2 ?? 10,
                Height = plant.Sortav * 2?? 10,
                Fill = new SolidColorBrush(c1),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            Canvas.SetLeft(shape, (double)(x - plant.Sortav!));
            Canvas.SetTop(shape, (double)(y - plant.Sortav!));
            canvas.Children.Add(shape);
#else

            MouseButtonEventHandler onPlantClick = (s, e) => {
                Debug.WriteLine($"Clicked on {plant.Name}");
                plantClickCallback(plant);
            };

            var shape = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(((Color)ColorConverter.ConvertFromString(plant.Color))),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Cursor = Cursors.Hand,
                ToolTip = new StackPanel
                {
                    Children =
                    {
                        new Label
                        {
                            Content = plant.Name.ToUpper(),
                            FontWeight = FontWeights.Bold,
                            Foreground = new SolidColorBrush(((Color)ColorConverter.ConvertFromString(plant.Color))),
                            Effect = new DropShadowEffect{ Color=Colors.Black, ShadowDepth=1 }
                        },
                        new Label
                        {
                            Content = $"Pozíció: {x}cm, {y}cm",
                            FontWeight = FontWeights.Bold,
                        }
                    }
                }
            };
            shape.MouseEnter += (s, e) => {
                (s as Shape)!.Stroke = new SolidColorBrush(Colors.Green);
                (s as Shape)!.StrokeThickness = 4;
            };
            shape.MouseLeave += (s, e) => {
                (s as Shape)!.Stroke = new SolidColorBrush(Colors.Black);
                (s as Shape)!.StrokeThickness = 1;
            };
            shape.MouseUp += onPlantClick;
            Canvas.SetLeft(shape, x - 10);
            Canvas.SetTop(shape, y - 10);
            canvas.Children.Add(shape);
#endif

            var line1 = new Line()
            {
                X1 = x-5,
                Y1 = y-5,
                X2 = x+5,
                Y2 = y+5,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Cursor = Cursors.Hand,
                IsHitTestVisible = false
            };
            canvas.Children.Add(line1);

            var line2 = new Line()
            {
                X1 = x - 5,
                Y1 = y + 5,
                X2 = x + 5,
                Y2 = y - 5,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Cursor = Cursors.Hand,
                IsHitTestVisible = false
            };
            canvas.Children.Add(line2);
        }
    }
}
