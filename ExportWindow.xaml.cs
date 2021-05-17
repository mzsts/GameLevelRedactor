using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using GameLevelRedactor.Data;

namespace GameLevelRedactor
{
    public partial class ExportWindow : Window
    {
        private ObservableCollection<LevelData> levelsData;
        public ExportWindow()
        {
            InitializeComponent();

            levelsData = new();
            levelsList.ItemsSource = levelsData;
        }

        private void LevelsList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
            }
        }
        private void LevelsList_DragLeave(object sender, DragEventArgs e)
        {
        }
        private void LevelsList_Drop(object sender, DragEventArgs e)
        {
            List<string> paths = new();

            foreach (string obj in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (Directory.Exists(obj))
                    paths.AddRange(Directory.GetFiles(obj, "*.json", SearchOption.TopDirectoryOnly));
                else
                    paths.Add(obj);
            }

            foreach (string path in paths)
            {
                try
                {
                    levelsData.Add(Parser.GetLevelData(File.ReadAllText(path)));
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
