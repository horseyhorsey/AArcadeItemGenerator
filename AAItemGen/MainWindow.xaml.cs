using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Winforms = System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace AAItemGen
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        AAItems itemGenerator = new AAItems();

        public MainWindow()
        {
            InitializeComponent();

           _TextboxHSPath.Text = Properties.Settings.Default.hsPath;
           _TextboxAAPath.Text = Properties.Settings.Default.aaPath;

            //AAItems itemGenerator = new AAItems();
            //AAItems gameItem = new AAItems
            //{
            //    Title = "robocop",
            //    Type = "MAME",
            //    FileLocation = @"I:\HyperSpin\Media\Amstrad CPC\Video\3D Pool (Europe).png",
            //    App = "RocketLaunch"
            //};
            //itemGenerator.gamesList  = new List<AAItems>();
            //gameItem.gamesList.Add(gameItem);

            //textBlock.Text = itemGenerator.createJsonText(gameItem.gamesList);

            buildSystemsFromDatabase();
        }

        private void buildSystemsFromDatabase()
        {
            if (_TextboxHSPath.Text != string.Empty)
            {
                string menuXml = _TextboxHSPath.Text + "\\Databases\\Main Menu\\Main Menu.xml";
                if (File.Exists(menuXml))
                {
                    Menu menu = new Menu();
                    List<Menu> menuList = menu.getMainMenuItemsFromXml(menuXml);

                    foreach (Menu item in menuList)
                    {
                        _comboxBoxSystems.Items.Add(item.name);
                    }

                    _comboxBoxSystems.SelectedIndex = 0;
                }
                else
                    MessageBox.Show("Main Menu xml cannot be found at " + menuXml);

            }
        }

        #region Folder picker buttons
        private void _buttonRoms_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog fb = new Winforms.FolderBrowserDialog();
            Winforms.DialogResult result = fb.ShowDialog();
            if (result == Winforms.DialogResult.OK)
            {
                _TextboxRoms.Text = fb.SelectedPath;
            }
        }
        private void _buttonSnaps_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog fb = new Winforms.FolderBrowserDialog();
            Winforms.DialogResult result = fb.ShowDialog();
            if (result == Winforms.DialogResult.OK)
            {
                _TextboxSnaps.Text = fb.SelectedPath;
            }
        }
        private void _buttonMaquee_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog fb = new Winforms.FolderBrowserDialog();
            Winforms.DialogResult result = fb.ShowDialog();
            if (result == Winforms.DialogResult.OK)
            {
                _TextboxMarquee.Text = fb.SelectedPath;
            }
        }
        #endregion

        private void _buttonModels_Click(object sender, RoutedEventArgs e)
        {
            Winforms.OpenFileDialog od = new Winforms.OpenFileDialog();
            od.DefaultExt = ".mdl";
            od.InitialDirectory = _TextboxAAPath.Text;
            Winforms.DialogResult result = od.ShowDialog();

            if (result == Winforms.DialogResult.OK)
            {
                //string modelPath = _TextboxModels.Text;
                //Regex 
                //_TextboxModels.Text = od.FileName;
            }

        }

        /// <summary>
        /// Save paths to applications config so it can be used when re launching app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _buttonSavePaths_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.hsPath = _TextboxHSPath.Text;
            Properties.Settings.Default.aaPath = _TextboxAAPath.Text;

            Properties.Settings.Default.Save();

            _comboxBoxSystems.Items.Clear();
            buildSystemsFromDatabase();
            
        }

        /// <summary>
        /// Generate items from a hyperspin database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _buttonGenFromHS_Click(object sender, RoutedEventArgs e)
        {
            string hsPath = _TextboxHSPath.Text;
            string xml = System.IO.Path.Combine(hsPath, "Databases", _comboxBoxSystems.SelectedItem.ToString(), _comboxBoxSystems.SelectedItem.ToString() + ".xml");

            if (File.Exists(xml))
            {
                itemGenerator.generateItemsFromHS(xml, _checkClones.IsChecked.Value, _TextboxRoms.Text, _TextboxType.Text, _TextboxRomExt.Text,
                    _TextboxApp.Text, _TextboxModels.Text, _comboxBoxSystems.SelectedItem.ToString(), _TextboxAAPath.Text, _TextboxSnaps.Text, _TextboxMarquee.Text);
                _canvas.IsEnabled = false;
            }
            else
                MessageBox.Show("Cannot find system xml at =" + xml);

            _canvas.IsEnabled = true;
        }

        private void _genRomsButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                _canvas.IsEnabled = false;
                itemGenerator.generateItemsFromFolder(_TextboxRoms.Text, _TextboxType.Text, _TextboxRomExt.Text, _TextboxApp.Text, _TextboxModels.Text, _TextboxType.Text,_TextboxAAPath.Text, _TextboxSnaps.Text, _TextboxMarquee.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed, check text fields");
                _canvas.IsEnabled = true;
            }

            _canvas.IsEnabled = true;
        }    



    }
}
