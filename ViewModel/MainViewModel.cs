using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Book> MyObservableList { get; } = [];
    JSONServices MyJSONService;
    CSVServices MyCSVServices;

    public MainViewModel(JSONServices MyJSONService, CSVServices MyCSVServices)
    {
        this.MyJSONService = MyJSONService;
        this.MyCSVServices = MyCSVServices;
    }

    [RelayCommand]
    internal async Task GoToDetails(string isbn)
    {
        IsBusy = true;

        await Shell.Current.GoToAsync("DetailsView", true, new Dictionary<string, object>
        {
            {"selectedBook", isbn}
        });

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task GoToGraph()
    {
        IsBusy = true;

        await Shell.Current.GoToAsync("GraphView", true);

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task PrintToCSV()
    {
        IsBusy = true;

        await MyCSVServices.PrintData(Globals.MyBooks);

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task LoadFromCSV()
    {
        IsBusy = true;

        var loadedBooks = await MyCSVServices.LoadData();
        if (loadedBooks.Count > 0)
        {
            Globals.MyBooks = loadedBooks;
            await RefreshPage();
            await Shell.Current.DisplayAlert("Succès", $"{loadedBooks.Count} livres ont été chargés depuis le fichier CSV", "OK");
        }

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task UploadJSON()
    {
        IsBusy = true;

        await MyJSONService.SetBooks(Globals.MyBooks);

        IsBusy = false;
    }

    internal async Task RefreshPage()
    {
        MyObservableList.Clear();

        if (Globals.MyBooks.Count == 0)
        {
            try
            {
                Globals.MyBooks = await MyJSONService.GetBooks();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erreur", $"Impossible de charger les données: {ex.Message}", "OK");
            }
        }

        foreach (var item in Globals.MyBooks)
        {
            MyObservableList.Add(item);
        }
    }
}