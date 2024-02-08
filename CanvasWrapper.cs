using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace garden_planner
{
    class CanvasWrapper
    {
        private Canvas canvas;
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
                Stroke = System.Windows.Media.Brushes.Black,
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
    }
}
