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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace garden_planner
{
    /// <summary>
    /// Interaction logic for AddPlantDialog.xaml
    /// </summary>
    public partial class AddPlantDialog : Window
    {
        private bool _isEditing = false;
        private long currentId = 0;
        public AddPlantDialog()
        {
            InitializeComponent();
  
            var plants = Database.GetAllPlantsOrdered().Select(x => x.Name).ToList();
            
                plants.ForEach(x => GoodNeighsCombo.Items.Add(x));
           
            
                plants.ForEach(x => BadNeighsCombo.Items.Add(x));
            
        }

        public AddPlantDialog(string name, long? sortav, long? totav, string? color, long id, List<string> goodNeighs, List<string> badNeighs)
        {
            InitializeComponent();
            NameTB.Text = name;
            SortavTB.Text = (sortav ?? 0).ToString();
            TotavTB.Text = (totav ?? 0).ToString();
            PlantColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(color);
            currentId = id;
            var plants = Database.GetAllPlantsOrdered().Select(x => x.Name).ToList();
            plants.ForEach(x => GoodNeighsCombo.Items.Add(x));
            plants.ForEach(x => BadNeighsCombo.Items.Add(x));
            MainAddPlantBtn.Content = "Növény módosítása";
            if (goodNeighs.Count > 0)
            {
                goodNeighs.ForEach(x => GoodNeighsListBox.Items.Add(x));
            }
            if (badNeighs.Count > 0)
            {
                badNeighs.ForEach(x => BadNeighsListBox.Items.Add(x));
            }
            _isEditing = true;
        }

        private void GoodNeighsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GoodNeighsBtn.IsEnabled = GoodNeighsCombo.SelectedIndex != -1;
        }

        private void BadNeighsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BadNeighsBtn.IsEnabled = BadNeighsCombo.SelectedIndex != -1;
        }

        private void GoodNeigsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GoodNeighsListBox.Items.Contains(GoodNeighsCombo.SelectedValue))
                return;
            GoodNeighsListBox.Items.Add(GoodNeighsCombo.SelectedValue);
            GoodNeighsCombo.SelectedIndex = -1;
        }

        private void BadNeigsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BadNeighsListBox.Items.Contains(BadNeighsCombo.SelectedValue))
                return;
            BadNeighsListBox.Items.Add(BadNeighsCombo.SelectedValue);
            BadNeighsCombo.SelectedIndex = -1;
        }

        private void MainAddPlantBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isEditing)
            {
                AddPlant();
            }
            else
            {
                EditPlant();
            }
            
        }

        private void AddPlant()
        {
            string? errMsg = null;
            long sortav = 0;
            long totav = 0;

            HashSet<string> gns = new HashSet<string>(GoodNeighsListBox.Items.OfType<string>());
            HashSet<string> bns = new HashSet<string>(BadNeighsListBox.Items.OfType<string>());

            if (string.IsNullOrEmpty(NameTB.Text))
            {
                errMsg = "A növény neve nem lehet üres.";
            }
            else if (string.IsNullOrEmpty(SortavTB.Text) || !long.TryParse(SortavTB.Text, out sortav) || sortav < 1 || sortav > 999)
            {
                errMsg = "Rossz sortávolság.";
            }
            else if (string.IsNullOrEmpty(TotavTB.Text) || !long.TryParse(TotavTB.Text, out totav) || totav < 1 || totav > 999)
            {
                errMsg = "Rossz tőtávolság.";
            }
            else if (gns.Intersect(bns).Any())
            {
                errMsg = "Egy adott növény nem lehet mindkét szomszéd listában.";
            }

            if (errMsg != null)
            {
                System.Windows.MessageBox.Show(errMsg, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Plant plant = new Plant(NameTB.Text, sortav, totav, PlantColorPicker.SelectedColorText);
            var gnIds = gns.Select(x => (long)Database.GetPlantIdFromName(x)!).ToList();
            var bnIds = bns.Select(x => (long)Database.GetPlantIdFromName(x)!).ToList();
            Database.AddPlantDefinition(ref plant, gnIds, bnIds);

            Close();
        }

        private void EditPlant()
        {
            string? errMsg = null;
            long sortav = 0;
            long totav = 0;

            HashSet<string> gns = new HashSet<string>(GoodNeighsListBox.Items.OfType<string>());
            HashSet<string> bns = new HashSet<string>(BadNeighsListBox.Items.OfType<string>());

            if (string.IsNullOrEmpty(NameTB.Text))
            {
                errMsg = "A növény neve nem lehet üres.";
            }
            else if (string.IsNullOrEmpty(SortavTB.Text) || !long.TryParse(SortavTB.Text, out sortav) || sortav < 1 || sortav > 999)
            {
                errMsg = "Rossz sortávolság.";
            }
            else if (string.IsNullOrEmpty(TotavTB.Text) || !long.TryParse(TotavTB.Text, out totav) || totav < 1 || totav > 999)
            {
                errMsg = "Rossz tőtávolság.";
            }
            else if (gns.Intersect(bns).Any())
            {
                errMsg = "Egy adott növény nem lehet mindkét szomszéd listában.";
            }

            if (errMsg != null)
            {
                System.Windows.MessageBox.Show(errMsg, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Plant plant = new Plant(NameTB.Text, sortav, totav, PlantColorPicker.SelectedColorText);
            var gnIds = gns.Select(x => (long)Database.GetPlantIdFromName(x)!).ToList();
            var bnIds = bns.Select(x => (long)Database.GetPlantIdFromName(x)!).ToList();
            Database.RemovePlantDefinitionById(currentId);
            Database.AddPlantDefinition(ref plant, gnIds, bnIds);

            Close();
        }

        private void BadNeighsDelete_Click(object sender, RoutedEventArgs e)
        {
            BadNeighsListBox.Items.Remove(BadNeighsListBox.SelectedItem);
        }
        private void GoodNeighsDelete_Click(object sender, RoutedEventArgs e)
        {
            GoodNeighsListBox.Items.Remove(GoodNeighsListBox.SelectedItem);
        }
    }
}
