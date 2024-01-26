using System;
using System.Collections.Generic;
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
    }
}
