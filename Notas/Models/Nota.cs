using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Notas.Models
{
    public class Nota : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _titulo;
        public string Titulo
        {
            get => _titulo;
            set { _titulo = value; OnPropertyChanged(); }
        }

        private string _contenido;
        public string Contenido
        {
            get => _contenido;
            set { _contenido = value; OnPropertyChanged(); }
        }
    }
}
