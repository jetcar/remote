﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using remote.Annotations;

namespace remote
{
    public class ButtonCommands : INotifyPropertyChanged
    {

        public ButtonCommands()
        {
        }
        public ButtonCommands(string name,string image,Action method)
        {
            this.Name = name;
            ImagePath = image;
            this.Method = method;
            RemoveCommand = new Command<string>(Remove_Click);
        }

        private void Remove_Click(string obj)
        {
            Commands.Remove(obj);
        }

        public string Name { get; set; }
        private ObservableCollection<string> _commands = new ObservableCollection<string>();
        private ImageSource _picture;
        private Command<string> _removeCommand;

        [XmlIgnore]
        public Action Method { get; set; }


        [XmlIgnore]
        public ImageSource Picture
        {
            get { return _picture; }
            set
            {
                if (Equals(value, _picture)) return;
                _picture = value;
                OnPropertyChanged();
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                Picture = new BitmapImage(new Uri("/images/" + value, UriKind.RelativeOrAbsolute));
            }
        }



        public ObservableCollection<string> Commands
        {
            get { return _commands; }
            set
            {
                if (Equals(value, _commands)) return;
                _commands = value;
                OnPropertyChanged();
            }
        }
        [XmlIgnore]
        public Command<string> RemoveCommand
        {
            get { return _removeCommand; }
            set
            {
                if (Equals(value, _removeCommand)) return;
                _removeCommand = value;
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
    }
}
