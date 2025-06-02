using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

[QueryProperty(nameof(ISBN), "selectedBook")]
public partial class DetailsViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? ISBN { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial string? Author { get; set; }

    [ObservableProperty]
    public partial string? Category { get; set; }

    [ObservableProperty]
    public partial int Stock { get; set; }

    [ObservableProperty]
    public partial decimal Price { get; set; }

    [ObservableProperty]
    public partial string? CoverImage { get; set; }

    [ObservableProperty]
    public partial string? SerialBufferContent { get; set; }

    [ObservableProperty]
    public partial bool EmulatorON_OFF { get; set; } = false;

    readonly DeviceOrientationService MyScanner;

    IDispatcherTimer emulator = Application.Current.Dispatcher.CreateTimer();

    public DetailsViewModel(DeviceOrientationService myScanner)
    {
        this.MyScanner = myScanner;
        MyScanner.OpenPort();
        myScanner.SerialBuffer.Changed += OnSerialDataReception;

        emulator.Interval = TimeSpan.FromSeconds(1);
        emulator.Tick += (s, e) => AddCode();
    }

    partial void OnEmulatorON_OFFChanged(bool value)
    {
        if (value) emulator.Start();
        else emulator.Stop();
    }

    private void AddCode()
    {
        // Simulation d'un code ISBN - Pourrait être amélioré pour générer des ISBN valides
        MyScanner.SerialBuffer.Enqueue("978" + new Random().Next(1000000000, 2147483647).ToString());
    }

    private void OnSerialDataReception(object sender, EventArgs arg)
    {
        DeviceOrientationService.QueueBuffer MyLocalBuffer = (DeviceOrientationService.QueueBuffer)sender;

        if (MyLocalBuffer.Count > 0)
        {
            string scannedCode = MyLocalBuffer.Dequeue().ToString();
            SerialBufferContent = scannedCode;

            // Si on est en mode ajout (ISBN est vide), on assigne le code scanné à l'ISBN
            if (string.IsNullOrEmpty(ISBN))
            {
                ISBN = scannedCode;
            }

            OnPropertyChanged(nameof(SerialBufferContent));
        }
    }

    internal void RefreshPage()
    {
        foreach (var item in Globals.MyBooks)
        {
            if (ISBN == item.ISBN)
            {
                Title = item.Title;
                Author = item.Author;
                Category = item.Category;
                Stock = item.Stock;
                Price = item.Price;
                CoverImage = item.CoverImage;
                break;
            }
        }
    }

    internal void ClosePage()
    {
        MyScanner.SerialBuffer.Changed -= OnSerialDataReception;
        MyScanner.ClosePort();
    }

    [RelayCommand]
    internal void ChangeObjectParameters()
    {
        var existingBook = Globals.MyBooks.FirstOrDefault(b => b.ISBN == ISBN);

        if (existingBook != null)
        {
            // Mettre à jour un livre existant
            existingBook.Title = Title ?? string.Empty;
            existingBook.Author = Author ?? string.Empty;
            existingBook.Category = Category ?? string.Empty;
            existingBook.Stock = Stock;
            existingBook.Price = Price;
            existingBook.CoverImage = CoverImage ?? string.Empty;

            Shell.Current.DisplayAlert("Succès", "Le livre a été mis à jour avec succès", "OK");
        }
        else
        {
            // Ajouter un nouveau livre
            var newBook = new Book
            {
                ISBN = ISBN ?? string.Empty,
                Title = Title ?? string.Empty,
                Author = Author ?? string.Empty,
                Category = Category ?? string.Empty,
                Stock = Stock,
                Price = Price,
                CoverImage = CoverImage ?? string.Empty
            };

            Globals.MyBooks.Add(newBook);
            Shell.Current.DisplayAlert("Succès", "Le livre a été ajouté avec succès", "OK");
        }
    }
}