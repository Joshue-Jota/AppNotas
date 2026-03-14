using Notas.Database;
using Notas.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Notas.ViewModels
{
    [QueryProperty(nameof(NotaId), "id")]
    public class DetalleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        readonly DatabaseNotas database;
        Nota _notaActual;
        bool _esNueva = true;

        // ── Clave para guardar estado temporal ──────────────────────
        const string KEY_TITULO = "temp_titulo";
        const string KEY_CONTENIDO = "temp_contenido";
        const string KEY_PIN = "temp_pin";
        const string KEY_ID = "temp_id";

        private int _notaId;
        public int NotaId
        {
            get => _notaId;
            set
            {
                _notaId = value;
                OnPropertyChanged();
                CargarNota(value);
            }
        }

        private string _titulo;
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                OnPropertyChanged();
                // Guarda en preferencias cada vez que cambia
                Preferences.Set(KEY_TITULO, value ?? "");
            }
        }

        private string _contenido;
        public string Contenido
        {
            get => _contenido;
            set
            {
                _contenido = value;
                OnPropertyChanged();
                Preferences.Set(KEY_CONTENIDO, value ?? "");
            }
        }

        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                _isPinned = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PinTexto));
                Preferences.Set(KEY_PIN, value);
            }
        }

        public string PinTexto => IsPinned ? "📌" : "☆";
        public string TituloPagina => _esNueva ? "Nueva Nota" : "Editar Nota";
        public string FechaHoy => DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");
        public string EstadoPin => "📌 Fijada";

        public ICommand GuardarCommand { get; }
        public ICommand TogglePinCommand { get; }

        public DetalleViewModel(DatabaseNotas db)
        {
            database = db;
            GuardarCommand = new Command(async () => await Guardar());
            TogglePinCommand = new Command(() => IsPinned = !IsPinned);
        }

        async void CargarNota(int id)
        {
            if (id <= 0)
            {
                // Es nota nueva — restaurar si hay estado guardado
                RestaurarEstadoTemporal();
                return;
            }

            _notaActual = await database.GetNotaByIdAsync(id);
            if (_notaActual != null)
            {
                _esNueva = false;
                OnPropertyChanged(nameof(TituloPagina));

                // Si hay estado temporal guardado para esta nota, usarlo
                int idGuardado = Preferences.Get(KEY_ID, -1);
                if (idGuardado == id)
                {
                    RestaurarEstadoTemporal();
                }
                else
                {
                    // Primera vez cargando esta nota
                    Titulo = _notaActual.Titulo;
                    Contenido = _notaActual.Contenido;
                    IsPinned = _notaActual.IsPinned;

                    // Guardar id en preferencias
                    Preferences.Set(KEY_ID, id);
                }
            }
        }

        void RestaurarEstadoTemporal()
        {
            // Restaura lo que había antes de la rotación
            var tituloGuardado = Preferences.Get(KEY_TITULO, "");
            var contenidoGuardado = Preferences.Get(KEY_CONTENIDO, "");
            var pinGuardado = Preferences.Get(KEY_PIN, false);

            // Asigna directo al campo para no sobreescribir preferences
            _titulo = tituloGuardado;
            _contenido = contenidoGuardado;
            _isPinned = pinGuardado;

            OnPropertyChanged(nameof(Titulo));
            OnPropertyChanged(nameof(Contenido));
            OnPropertyChanged(nameof(IsPinned));
            OnPropertyChanged(nameof(PinTexto));
        }

        public void LimpiarEstadoTemporal()
        {
            // Llamar cuando se guarda o se cancela
            Preferences.Remove(KEY_TITULO);
            Preferences.Remove(KEY_CONTENIDO);
            Preferences.Remove(KEY_PIN);
            Preferences.Remove(KEY_ID);
        }

        async Task Guardar()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
            {
                await Shell.Current.DisplayAlert(
                    "Campo requerido",
                    "El título no puede estar vacío.",
                    "OK");
                return;
            }

            if (_esNueva || _notaActual == null)
            {
                _notaActual = new Nota
                {
                    Titulo = Titulo,
                    Contenido = Contenido,
                    IsPinned = IsPinned,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await database.SaveNotaAsync(_notaActual);
            }
            else
            {
                _notaActual.Titulo = Titulo;
                _notaActual.Contenido = Contenido;
                _notaActual.IsPinned = IsPinned;
                _notaActual.UpdatedAt = DateTime.Now;
                await database.UpdateNotaAsync(_notaActual);
            }

            // Limpiar estado temporal al guardar exitosamente
            LimpiarEstadoTemporal();

            await Shell.Current.GoToAsync("..");
        }
    }
}