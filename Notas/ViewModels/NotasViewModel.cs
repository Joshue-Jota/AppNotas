using Notas.Database;
using Notas.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Notas.ViewModels
{
    public class NotasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ── Propiedades con notificación ──────────────────────────────

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

        private Nota _notaSeleccionada;
        public Nota NotaSeleccionada
        {
            get => _notaSeleccionada;
            set { _notaSeleccionada = value; OnPropertyChanged(); }
        }

        private bool _modoEdicion;
        public bool ModoEdicion
        {
            get => _modoEdicion;
            set
            {
                _modoEdicion = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModoGuardar));
            }
        }

        public bool ModoGuardar => !ModoEdicion;

        // ── Lista y comandos ──────────────────────────────────────────

        public ObservableCollection<Nota> ListaNotas { get; set; }

        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand ActualizarCommand { get; }

        DatabaseNotas database;

        // ── Constructor ───────────────────────────────────────────────

        public NotasViewModel(DatabaseNotas db)
        {
            database = db;
            ListaNotas = new ObservableCollection<Nota>();

            GuardarCommand = new Command(async () => await GuardarNota());
            EliminarCommand = new Command<Nota>(async (nota) => await EliminarNota(nota));
            EditarCommand = new Command<Nota>(SeleccionarNota);
            ActualizarCommand = new Command(async () => await ActualizarNota());

            CargarNotas();
        }

        // ── Métodos ───────────────────────────────────────────────────

        async void CargarNotas()
        {
            var notas = await database.GetNotasAsync();
            foreach (var nota in notas)
                ListaNotas.Add(nota);
        }

        async Task GuardarNota()
        {
            if (string.IsNullOrWhiteSpace(Titulo)) return;

            var nuevaNota = new Nota
            {
                Titulo = Titulo,
                Contenido = Contenido
            };

            await database.SaveNotaAsync(nuevaNota);
            ListaNotas.Add(nuevaNota);

            Titulo = "";
            Contenido = "";
        }

        async Task EliminarNota(Nota nota)
        {
            await database.DeleteNotaAsync(nota);
            ListaNotas.Remove(nota);
        }

        async Task ActualizarNota()
        {
            if (NotaSeleccionada == null) return;

            NotaSeleccionada.Titulo = Titulo;
            NotaSeleccionada.Contenido = Contenido;

            await database.UpdateNotaAsync(NotaSeleccionada);

            // Refresca el item visualmente en la lista
            var index = ListaNotas.IndexOf(NotaSeleccionada);
            if (index >= 0)
            {
                ListaNotas.RemoveAt(index);
                ListaNotas.Insert(index, NotaSeleccionada);
            }

            Titulo = "";
            Contenido = "";
            NotaSeleccionada = null;
            ModoEdicion = false;
        }

        void SeleccionarNota(Nota nota)
        {
            NotaSeleccionada = nota;
            Titulo = nota.Titulo;
            Contenido = nota.Contenido;
            ModoEdicion = true;
        }
    }
}