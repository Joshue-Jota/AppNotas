using Notas.Database;
using Notas.ViewModels;

namespace Notas.Views
{
    public partial class DetallePage : ContentPage
    {
        public DetallePage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "notas.db3");
            BindingContext = new DetalleViewModel(new DatabaseNotas(dbPath));
        }

        // Si el usuario presiona "atrás" sin guardar, limpia el estado
        protected override bool OnBackButtonPressed()
        {
            if (BindingContext is DetalleViewModel vm)
                vm.LimpiarEstadoTemporal();

            return base.OnBackButtonPressed();
        }
    }
}