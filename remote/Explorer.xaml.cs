using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using remote.Annotations;
using Path = System.IO.Path;

namespace remote
{
    /// <summary>
    /// Interaction logic for Explorer.xaml
    /// </summary>
    public partial class Explorer : INotifyPropertyChanged
    {
        private string _currentPath;
        private ObservableCollection<string> _files = new ObservableCollection<string>();
        private int _selectedIndex;
        public static string currentDirectory = null;
        public static string currentfile = null;
        public Explorer()
        {
            _currentPath = currentDirectory ?? ConfigurationManager.AppSettings["defaultPath"];
            if (currentfile != null)
            {
                SelectedIndex = new List<string>(Directory.GetFiles(_currentPath)).IndexOf(currentfile) + 1;
            }
            Open(CurrentPath);

            InitializeComponent();
        }

        void Open(string currentPath)
        {
            bool isDirectory = false;
            try
            {
                var directories = Directory.GetDirectories(currentPath);
                var files = Directory.GetFiles(currentPath);
                isDirectory = true;
                Files.Clear();
                Files.Add("..");
                foreach (var dir in directories)
                {
                    var folders = dir.Split(Path.DirectorySeparatorChar);
                    Files.Add(folders.Last());
                }
                foreach (var file in files)
                {
                    var folders = file.Split(Path.DirectorySeparatorChar);

                    Files.Add(folders.Last());
                }
                currentDirectory = currentPath;

            }
            catch (Exception)
            {

            }
            if (!isDirectory)
            {
                var extension = Path.GetExtension(currentPath);
                if (ConfigurationManager.AppSettings["extensions"].Contains(extension))
                {
                    var player = ConfigurationManager.AppSettings["playerName"];

                    Process p = Process.GetProcessesByName(player).FirstOrDefault();
                    if (p != null)
                    {
                        p.Kill();
                    }

                    Process.Start(currentPath);

                    Actions.SendKey("f");

                    this.Close();
                }
                currentfile = currentPath;
            }

            CurrentPath = currentPath;
        }

        public ObservableCollection<string> Files
        {
            get { return _files; }
            set
            {
                if (Equals(value, _files)) return;
                _files = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value == _selectedIndex) return;
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                if (value == _currentPath) return;
                _currentPath = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void MoveUp()
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
        }

        public void MoveDown()
        {
            if (SelectedIndex < Files.Count)
                SelectedIndex++;

        }

        public void OpenSelected()
        {
            if (SelectedIndex < 0)
                return;
            string path;
            if (Files[SelectedIndex] == "..")
            {
                var folders = CurrentPath.Split(Path.DirectorySeparatorChar);
                var list = new List<string>(folders);
                if (list.Count > 1)
                    list.RemoveAt(list.Count - 1);
                path = Path.Combine(list.ToArray());
            }
            else
            {
                path = Path.Combine(CurrentPath, Files[SelectedIndex]);
            }
            Open(path);
        }

        private void ScrollIntoView(object sender, SelectionChangedEventArgs e)
        {
            if (Files.Count > SelectedIndex && SelectedIndex > -1)
                ListView.ScrollIntoView(Files[SelectedIndex]);
        }
    }
}
