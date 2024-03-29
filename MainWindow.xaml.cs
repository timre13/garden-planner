﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace garden_planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        CanvasWrapper? canvasWrapper;

        readonly int GardenWidth;
        readonly int GardenHeight;

        private bool canvasError;

        public MainWindow()
        {
            var dlg = new SizeDialog();
            dlg.ShowDialog();

            if (dlg.WidthValue == null || dlg.HeightValue == null)
            {
                Close();
                return;
            }

            GardenWidth = (int)dlg.WidthValue;
            GardenHeight = (int)dlg.HeightValue;

            InitializeComponent();
        }

        private void PlantList_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("PlantList_Loaded");

            PlantList.Items.Clear();
            var plants = Database.GetAllPlantsOrdered();

            foreach (var plant in plants)
            {
                int amount = 0;
                if (plantAmounts.TryGetValue(plant.Id, out var plantAmount))
                {
                    amount = plantAmount;
                }
                
                var item = new PlantListItem(
                    plant,
                    false,
                    false,
                    amount,
                    amount > 0
                );
                
                PlantList.Items.Add(item);
            }
        }

        private void PlantList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlantList.SelectedItem == null) return;
            PlantListItem currentItem = (PlantListItem)PlantList.SelectedItem;
            MenuUpDown.Value = currentItem.amount;
            RefreshPlantList();
        }

        private void AddPlantButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPlantDialog();
            dialog.Closed += (_, _) => PlantList_Loaded(new object(), new RoutedEventArgs());
            dialog.ShowDialog();
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            PlantListItem currentItem = (PlantListItem)PlantList.SelectedItem;
            Plant selectedPlant = currentItem.plant;
            Database.RemovePlantDefinitionById(selectedPlant.Id);
        }

        private void MenuEdit_Click(object sender, RoutedEventArgs e)
        {
            PlantListItem currentItem = (PlantListItem)PlantList.SelectedItem;
            Plant selectedPlant = currentItem.plant;
            var dialog = new AddPlantDialog(
                selectedPlant.Name,
                selectedPlant.Sortav,
                selectedPlant.Totav,
                selectedPlant.Color,
                selectedPlant.Id,
                Database.GetNeighs(selectedPlant.Id, true).Select(x => x.Name).ToList(),
                Database.GetNeighs(selectedPlant.Id, false).Select(x => x.Name).ToList()
            );
            dialog.Closed += (_, _) => PlantList_Loaded(new object(), new RoutedEventArgs());
            dialog.ShowDialog();
        }

        private readonly Dictionary<long, int> plantAmounts = new Dictionary<long, int>();

        private void MenuIntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (PlantList.SelectedItem == null) return;
            var currentItem = (PlantListItem)PlantList.SelectedItem;
            Plant selectedPlant = currentItem.plant;
            if (MenuUpDown.Value == null)
            {
                return;
            }
            plantAmounts[selectedPlant.Id] = (int)MenuUpDown.Value;
        }

        private void RefreshPlantList()
        {
            var currentItemI = PlantList.SelectedIndex;
            if (PlantList.SelectedItem == null) return;
            var currentItem = (PlantListItem)PlantList.SelectedItem;
            Plant selectedPlant = currentItem.plant;
            var goods = Database.GetNeighIds(selectedPlant.Id, true);
            var bads = Database.GetNeighIds(selectedPlant.Id, false);
            for (int i = 0; i < PlantList.Items.Count; i++)
            {
                var item = (PlantListItem)PlantList.Items[i];
                var plant = item.plant;
                int amount = 0;
                if (plantAmounts.TryGetValue(plant.Id, out var plantAmount))
                {
                    amount = plantAmount;
                }

                if (goods.Contains(plant.Id))
                {
                    PlantList.Items[i] = new PlantListItem(
                        plant,
                        true,
                        false,
                        amount,
                        amount > 0
                    );
                }
                else if (bads.Contains(plant.Id))
                {
                    PlantList.Items[i] = new PlantListItem(
                        plant,
                        false,
                        true,
                        amount,
                        amount > 0
                        );
                }
                else
                {
                    if (item.good || item.bad)
                    {
                        PlantList.Items[i] = new PlantListItem(
                            plant,
                            false,
                            false,
                            amount,
                            amount > 0
                            );
                    }
                }
            }
            PlantList.SelectedIndex = currentItemI;
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            PlantList_Loaded(new object(), new RoutedEventArgs());
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            canvasWrapper = new CanvasWrapper(mainCanvas, GardenWidth, GardenHeight,
                (p) => {
                    PlantList.SelectedIndex = PlantList.Items.OfType<dynamic>()
                        .Select(x => x.plant as Plant).ToList()
                        .FindIndex(y => y!.Id == p.Id);
                    PlantList.ScrollIntoView(PlantList.SelectedItem);
            });
            RedrawCanvas();
            SolveButton_Click(new object(), new RoutedEventArgs());
        }

        private void RedrawCanvas()
        {
            if (canvasWrapper == null) return;
            canvasWrapper.ClearCanvas();
            foreach (var p in placedPlants)
            {
                canvasWrapper.DrawPlant(p.plant, p.x, p.y);
            }
            canvasWrapper.DrawBorder(canvasError);
        }

        readonly List<PositionedPlant> placedPlants = new List<PositionedPlant>();

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            placedPlants.Clear();
            canvasError = false;
            var unorderedPlantsToPlace = new List<Plant>();
            foreach (PlantListItem plantCnt in PlantList.Items)
            {
                int count = plantCnt.amount;
                for (int i = 0; i < count; i++)
                {
                    unorderedPlantsToPlace.Add(plantCnt.plant);
                }
            }

            if (unorderedPlantsToPlace.Count == 0)
                return;
            
            var plantsToPlace = new List<Plant> { unorderedPlantsToPlace.PopFirst() };

            bool noGoods = false;
            bool onlyBads = false;
            int index = 0;
            while (unorderedPlantsToPlace.Count > 0)
            {
                if (index > unorderedPlantsToPlace.Count - 1)
                {
                    if (noGoods)
                    {
                        onlyBads = true;
                    }
                    else
                    {
                        noGoods = true;
                    }
                    index = 0;
                    continue;
                }
                Plant p = unorderedPlantsToPlace[index];
                var goodNeihbours = Database.GetNeighIds(p.Id, true);
                var badNeihbours = Database.GetNeighIds(p.Id, false);
                if (noGoods)
                {
                    if (onlyBads)
                    {
                        plantsToPlace.Add(unorderedPlantsToPlace.PopFirst());
                        index = 0;
                        onlyBads = false;
                        noGoods = false;
                        continue;
                    }
                    if (!badNeihbours.Contains(plantsToPlace.Last().Id))
                    {
                        plantsToPlace.Add(p);
                        unorderedPlantsToPlace.RemoveAt(index);
                        index = 0;
                        noGoods = false;
                        continue;
                    }
                }
                else
                {
                    if (goodNeihbours.Contains(plantsToPlace.Last().Id))
                    {
                        plantsToPlace.Add(p);
                        unorderedPlantsToPlace.RemoveAt(index);
                        index = 0;
                        continue;
                    }
                }
                index++;
            }

            {
                var p = plantsToPlace.PopFirst();
                placedPlants.Add(new PositionedPlant(p, (int)p.Totavv, (int)p.Sortavv));

                long currX = (int)p.Totavv;
                long currY = 0;

                long mostSorTav = p.Sortavv;
                long lineWidth = p.Totavv * 2;

                while (plantsToPlace.Count > 0)
                {
                    var p1 = plantsToPlace.PopFirst();
                    
                    currX += (int)placedPlants.Last().plant.Totavv + (int)p1.Totavv;
                    lineWidth += p1.Totavv * 2;
                    if (lineWidth > GardenWidth)
                    {
                        currX = p1.Totavv;
                        currY += (int)(mostSorTav);
                        lineWidth = p1.Totavv * 2;
                        mostSorTav = 0;
                    }
                    if (p1.Sortavv * 2 > mostSorTav)
                    {
                        mostSorTav = p1.Sortavv * 2;
                    }
                    if (currY + p1.Sortavv * 2 > GardenHeight)
                    {
                        canvasError = true;
                        break;
                    }
                    placedPlants.Add(new PositionedPlant(p1, (int)currX, (int)currY + (int)p1.Sortavv));
                }
            }

            RedrawCanvas();
        }
    }

    struct PositionedPlant
    {
        public readonly Plant plant;
        public readonly int x;
        public readonly int y;

        public readonly long LefX => x - plant.Totavv/2;
        public readonly long TopY => y - plant.Sortavv/2;
        public readonly long RightX => x + plant.Totavv / 2;
        public readonly long BottomY => y + plant.Sortavv / 2;

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
