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

        public ObservableCollection<Nota> ListaNotas { get; set; } = new();

        private string _busqueda;
        public string Busqueda
        {
            get => _busqueda;
            set
            {
                _busqueda = value;
                OnPropertyChanged();
                FiltrarNotas();
            }
        }

        public ICommand EliminarCommand { get; }
        public ICommand PinCommand { get; }
        public ICommand NuevaNotaCommand { get; }
        public ICommand EditarCommand { get; }

        readonly DatabaseNotas database;
        List<Nota> _todasLasNotas = new();

        public NotasViewModel(DatabaseNotas db)
        {
            database = db;

            EliminarCommand = new Command<Nota>(async (nota) => await ConfirmarEliminar(nota));
            PinCommand = new Command<Nota>(async (nota) => await TogglePin(nota));
            NuevaNotaCommand = new Command(async () => await NuevaNota());
            EditarCommand = new Command<Nota>(async (nota) => await AbrirEdicion(nota));

            CargarNotas();
        }

        public async void CargarNotas()
        {
            _todasLasNotas = await database.GetNotasAsync();
            ListaNotas.Clear();
            foreach (var n in _todasLasNotas)
                ListaNotas.Add(n);
        }

        void FiltrarNotas()
        {
            var filtradas = string.IsNullOrWhiteSpace(Busqueda)
                ? _todasLasNotas
                : _todasLasNotas.Where(n =>
                    n.Titulo.Contains(Busqueda, StringComparison.OrdinalIgnoreCase) ||
                    (n.Contenido?.Contains(Busqueda, StringComparison.OrdinalIgnoreCase) ?? false))
                  .ToList();

            ListaNotas.Clear();
            foreach (var n in filtradas)
                ListaNotas.Add(n);
        }

        async Task ConfirmarEliminar(Nota nota)
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Eliminar nota",
                $"¿Deseas eliminar \"{nota.Titulo}\"?",
                "Eliminar", "Cancelar");

            if (confirmar)
            {
                await database.DeleteNotaAsync(nota);
                ListaNotas.Remove(nota);
                _todasLasNotas.Remove(nota);
            }
        }

        async Task TogglePin(Nota nota)
        {
            nota.IsPinned = !nota.IsPinned;
            nota.UpdatedAt = DateTime.Now;
            await database.UpdateNotaAsync(nota);
            CargarNotas();
        }

        async Task NuevaNota()
        {
            await Shell.Current.GoToAsync("detalle");
        }

        async Task AbrirEdicion(Nota nota)
        {
            await Shell.Current.GoToAsync($"detalle?id={nota.Id}");
        }
    }
}