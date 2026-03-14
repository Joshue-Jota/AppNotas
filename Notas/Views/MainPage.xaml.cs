namespace Notas.Views;
using Notas.ViewModels;
using Notas.Database;


public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "notas.db3");

        var database = new DatabaseNotas(dbPath);

        BindingContext = new NotasViewModel(database);
    }
}