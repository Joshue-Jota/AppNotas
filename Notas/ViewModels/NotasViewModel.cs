using Notas.Database;
using Notas.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Notas.ViewModels
{
    public class NotasViewModel
    {
        public ObservableCollection<Nota> ListaNotas { get; set; }
        public Nota NotaSeleccionada { get; set; }
        public ICommand EliminarCommand { get; }
        public ICommand ActualizarCommand { get; }

        public string Titulo { get; set; }
        public string Contenido { get; set; }

        public ICommand GuardarCommand { get; }

        DatabaseNotas database;

        public NotasViewModel(DatabaseNotas db)
        {
            database = db;
            ListaNotas = new ObservableCollection<Nota>();

            GuardarCommand = new Command(async () => await GuardarNota());
            EliminarCommand = new Command<Nota>(async (nota) => await EliminarNota(nota));
            ActualizarCommand = new Command(async () => await ActualizarNota());
            CargarNotas();
        }

        async void CargarNotas()
        {
            var notas = await database.GetNotasAsync();

            foreach (var nota in notas)
                ListaNotas.Add(nota);
        }

        async Task GuardarNota()
        {
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
            if (NotaSeleccionada != null)
            {
                NotaSeleccionada.Titulo = Titulo;
                NotaSeleccionada.Contenido = Contenido;

                await database.UpdateNotaAsync(NotaSeleccionada);

                Titulo = "";
                Contenido = "";
                NotaSeleccionada = null;
            }
        }
        public void SeleccionarNota(Nota nota)
        {
            NotaSeleccionada = nota;
            Titulo = nota.Titulo;
            Contenido = nota.Contenido;
        }
    }
}
