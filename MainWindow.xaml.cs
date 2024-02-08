﻿using System;
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
        CanvasWrapper canvasWrapper;

        public int GardenWidth;
        public int GardenHeight;

        public MainWindow()
        {
            /*
            var dlg = new SizeDialog();
            dlg.ShowDialog();

            if (dlg.WidthValue == null || dlg.HeightValue == null)
            {
                Close();
                return;
            }

            GardenWidth = (int)dlg.WidthValue;
            GardenHeight = (int)dlg.HeightValue;
            */

            GardenWidth = (int)10000;
            GardenHeight = (int)10000;

            InitializeComponent();
        }

        private void PlantList_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("PlantList_Loaded");

            PlantList.Items.Clear();
            var plants = Database.GetAllPlantsOrdered();

            foreach (var plant in plants)
            {
                if (plant == null) continue;
                int amount = 0;
                if (plantAmounts.ContainsKey(plant.Id))
                {
                    amount = plantAmounts[plant.Id];
                }

                var item = new
                {
                    plant,
                    bad = false,
                    good = false,
                    amount = (plant.Id % 16 == 0 ? 4 : 0)
                };
                PlantList.Items.Add(item);
            }
        }

        private void PlantList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentItemI = PlantList.SelectedIndex;
            dynamic currentItem = PlantList.SelectedItem;
            if (currentItem?.plant == null) return;
            MenuUpDown.Value = currentItem.amount;
            RefreshPlantList();
        }

        private void AddPlantButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPlantDialog();
            dialog.Closed += (s, e) => PlantList_Loaded(null, null);
            dialog.ShowDialog();
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentItemI = PlantList.SelectedIndex;
            dynamic currentItem = PlantList.SelectedItem;
            if (currentItem?.plant == null) return;
            Plant selectedPlant = currentItem.plant;
            Database.RemovePlantDefinitionById(selectedPlant.Id);
        }

        private void MenuEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private Dictionary<long, int> plantAmounts = new Dictionary<long, int>();

        private void MenuIntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var currentItemI = PlantList.SelectedIndex;
            dynamic currentItem = PlantList.SelectedItem;
            if (currentItem?.plant == null) return;
            Plant selectedPlant = currentItem.plant;
            if (MenuUpDown.Value == null)
            {
                return;
            }
            if (plantAmounts.ContainsKey(selectedPlant.Id))
            {
                plantAmounts[selectedPlant.Id] = (int)MenuUpDown.Value;
            }
            else
            {
                plantAmounts.Add(selectedPlant.Id, (int)MenuUpDown.Value);
            }
        }

        private void RefreshPlantList()
        {
            var currentItemI = PlantList.SelectedIndex;
            dynamic currentItem = PlantList.SelectedItem;
            if (currentItem?.plant == null) return;
            Plant selectedPlant = currentItem.plant;
            var goods = Database.GetNeighIds(selectedPlant.Id, true);
            var bads = Database.GetNeighIds(selectedPlant.Id, false);
            for (int i = 0; i < PlantList.Items.Count; i++)
            {
                var item = PlantList.Items[i];
                var plant = (Plant)((dynamic)item).plant;
                if (plant == null) continue;
                int amount = 0;
                if (plantAmounts.ContainsKey(plant.Id))
                {
                    amount = plantAmounts[plant.Id];
                }

                if (goods.Contains(plant.Id))
                {
                    PlantList.Items[i] = new
                    {
                        good = true,
                        bad = false,
                        plant,
                        amount
                    };
                }
                else if (bads.Contains(plant.Id))
                {
                    PlantList.Items[i] = new
                    {
                        good = false,
                        bad = true,
                        plant,
                        amount
                    };
                }
                else
                {
                    if (((dynamic)item).good || ((dynamic)item).bad)
                    {
                        PlantList.Items[i] = new
                        {
                            good = false,
                            bad = false,
                            plant,
                            amount
                        };
                    }
                }
            }
            PlantList.SelectedIndex = currentItemI;
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            PlantList_Loaded(null, null);
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            canvasWrapper = new CanvasWrapper(mainCanvas, 0, 0);
            RedrawCanvas();
            SolveButton_Click(null, null);
        }

        private void RedrawCanvas()
        {
            foreach (var p in placedPlants)
            {
                canvasWrapper.DrawPlant(p.plant, p.x, p.y);
            }
        }

        List<PositionedPlant> placedPlants = new List<PositionedPlant>();

        static long GetXDist(in Plant p1, in Plant p2)
        {
            return Math.Max(p1.Totavv, p2.Totavv);
        }
        static long GetYDist(in Plant p1, in Plant p2)
        {
            return Math.Max(p1.Sortavv, p2.Sortavv);
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            var plantsToPlace = new List<Plant>();
            foreach (var plantCnt in PlantList.Items)
            {
                var count = (plantCnt as dynamic).amount;
                for (int i = 0; i < count; i++)
                {
                    plantsToPlace.Add((plantCnt as dynamic).plant);
                }
            }

            if (plantsToPlace.Count == 0)
                return;

            {
                var p = plantsToPlace.PopFirst();
                placedPlants.Add(new PositionedPlant(p, (int)p.Totavv/2, (int)p.Sortavv/2));

                long currX = 0;
                long currY = 0;
                while (plantsToPlace.Count > 0)
                {
                    var p1 = plantsToPlace.PopFirst();
                    currX += (int)placedPlants.Last().plant.Totavv / 2 + (int)p1.Totavv / 2;
                    placedPlants.Add(new PositionedPlant(p1,
                        (int)currX,
                        (int)p1.Sortavv/2));
                    if (currX > GardenWidth)
                    {
                        currX = p1.Totavv/2;
                        currY += p1.Sortavv / 2;
                    }
                }
            }

            RedrawCanvas();
        }
    }

    struct PositionedPlant
    {
        public Plant plant;
        public int x;
        public int y;

        public PositionedPlant(in Plant p, int x, int y)
        {
            this.plant = p;
            this.x = x;
            this.y = y;
        }
    }

    static class ListExtension
    {
        public static T PopFirst<T>(this List<T> list)
        {
            T r = list[0];
            list.RemoveAt(0);
            return r;
        }
    }
}
