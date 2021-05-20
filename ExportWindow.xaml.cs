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
using GameLevelRedactor.Data;

namespace GameLevelRedactor
{
    public partial class ExportWindow : Window
    {
        private ObservableCollection<LevelData> levelsData = new();
        public ExportWindow()
        {
            InitializeComponent();

            InitButtons();

            levelsList.ItemsSource = levelsData;
        }
        private void InitButtons()
        {
            upItemButton.Click += (s, e) =>
            {
                if (levelsList.SelectedItem != null)
                {
                    LevelData selected = (LevelData)levelsList.SelectedItem;
                    int index = levelsList.SelectedIndex;
                    if (index > 0)
                    {
                        LevelData temp = levelsData[index - 1];
                        levelsData[index - 1] = selected;
                        levelsData[index] = temp;
                        levelsList.SelectedIndex = index - 1;
                    }
                }
            };
            downItemButton.Click += (s, e) =>
            {
                if (levelsList.SelectedItem != null)
                {
                    LevelData selected = (LevelData)levelsList.SelectedItem;
                    int index = levelsList.SelectedIndex;
                    if (index < levelsData.Count - 1)
                    {
                        LevelData temp = levelsData[index + 1];
                        levelsData[index + 1] = selected;
                        levelsData[index] = temp;
                        levelsList.SelectedIndex = index + 1;
                    }
                }
            };
            deleteItemButton.Click += (s, e) =>
            {
                if (levelsList.SelectedItem != null)
                    levelsData.Remove((LevelData)levelsList.SelectedItem);
            };
        }
        private void SendSetToServer(object sender, EventArgs e)
        {
            LevelsSetData lsd = new() { Tematic = tematicTextBox.Text, Levels = levelsData.ToList() };
            //Отправка на сервер
        }
        private void LevelsList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
            }
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
