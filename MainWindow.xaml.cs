using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace garden_planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public struct PlantListItem
        {
            public Plant plant;
            public bool bad;
            public bool good;
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Root.Items.Clear();
            var plants = Database.GetAllPlantsOrdered();
            foreach (var plant in plants)
            {
                if (plant == null) continue;
                var item = new
                {
                    plant,
                    bad = false,
                    good = false
                };
                Root.Items.Add(item);
            }
        }

        private void Root_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic currentItem = Root.SelectedItem;
            if (currentItem?.plant == null) return;
            Plant selectedPlant = currentItem.plant;
            var goods = Database.GetNeighIds(selectedPlant.Id, true);
            var bads = Database.GetNeighIds(selectedPlant.Id, false);
            for (int i = 0; i < Root.Items.Count; i++)
            {
                var item = Root.Items[i];
                var plant = (Plant)((dynamic)item).plant;
                if (goods.Contains(plant.Id))
                {
                    Root.Items[i] = new
                    {
                        good = true,
                        bad = false,
                        plant
                    };
                } else if (bads.Contains(plant.Id))
                {
                    Root.Items[i] = new
                    {
                        good = false,
                        bad = true,
                        plant
                    };
                } else
                {
                    if (((dynamic)item).good || ((dynamic)item).bad)
                    {
                        Root.Items[i] = new
                        {
                            good = false,
                            bad = false,
                            plant
                        };
                    }
                }
            }
        }
    }
}
