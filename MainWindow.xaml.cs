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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Root_Loaded");
            Root.Items.Clear();
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
                    amount
                };
                Root.Items.Add(item);
            }
        }

        private void Root_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentItemI = Root.SelectedIndex;
            dynamic currentItem = Root.SelectedItem;
            if (currentItem?.plant == null) return;
            MenuUpDown.Value = currentItem.amount;
            RefreshPlantList();
        }

        private void AddPlantButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPlantDialog();
            dialog.Closed += (s, e) => Root_Loaded(null, null);
            dialog.ShowDialog();
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private Dictionary<long, int> plantAmounts = new Dictionary<long, int>();

        private void MenuIntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var currentItemI = Root.SelectedIndex;
            dynamic currentItem = Root.SelectedItem;
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
            var currentItemI = Root.SelectedIndex;
            dynamic currentItem = Root.SelectedItem;
            if (currentItem?.plant == null) return;
            Plant selectedPlant = currentItem.plant;
            var goods = Database.GetNeighIds(selectedPlant.Id, true);
            var bads = Database.GetNeighIds(selectedPlant.Id, false);
            for (int i = 0; i < Root.Items.Count; i++)
            {
                var item = Root.Items[i];
                var plant = (Plant)((dynamic)item).plant;
                if (plant == null) continue;
                int amount = 0;
                if (plantAmounts.ContainsKey(plant.Id))
                {
                    amount = plantAmounts[plant.Id];
                }

                if (goods.Contains(plant.Id))
                {
                    Root.Items[i] = new
                    {
                        good = true,
                        bad = false,
                        plant,
                        amount
                    };
                }
                else if (bads.Contains(plant.Id))
                {
                    Root.Items[i] = new
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
                        Root.Items[i] = new
                        {
                            good = false,
                            bad = false,
                            plant,
                            amount
                        };
                    }
                }
            }
            Root.SelectedIndex = currentItemI;
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            Root_Loaded(null, null);
        }
    }
}
