using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace garden_planner
{
    /// <summary>
    /// Interaction logic for SizeDialog.xaml
    /// </summary>
    public partial class SizeDialog : Window
    {
        public int? WidthValue { get; private set; }
        public int? HeightValue { get; private set; }

        public SizeDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            WidthValue = WidthTB.Value;
            HeightValue = HeightTB.Value;
            Close();
        }
    }
}
