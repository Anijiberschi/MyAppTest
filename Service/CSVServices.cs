using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Service;

public class CSVServices
{
    public async Task<List<Book>> LoadData()
    {
        List<Book> list = [];

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Sélectionnez un fichier CSV"
        });

        if (result != null)
        {
            var lines = await File.ReadAllLinesAsync(result.FullPath, Encoding.UTF8);

            // Filtrer les lignes vides
            var nonEmptyLines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            if (nonEmptyLines.Length == 0)
            {
                await Shell.Current.DisplayAlert("Erreur", "Le fichier CSV est vide", "OK");
                return list;
            }

            var headers = nonEmptyLines[0].Split(';');
            var properties = typeof(Book).GetProperties();

            for (int i = 1; i < nonEmptyLines.Length; i++)
            {
                string line = nonEmptyLines[i];

                // Ignorer les lignes vides ou qui ne contiennent que des délimiteurs
                if (string.IsNullOrWhiteSpace(line) || line.Trim().Replace(";", "") == "")
                    continue;

                var values = line.Split(';');

                // Vérifier si la ligne contient au moins une valeur non vide
                bool hasData = values.Any(v => !string.IsNullOrWhiteSpace(v));
                if (!hasData)
                    continue;

                Book obj = new();

                for (int j = 0; j < headers.Length; j++)
                {
                    var property = properties.FirstOrDefault(p => p.Name.Equals(headers[j], StringComparison.OrdinalIgnoreCase));

                    if (property != null && j < values.Length && !string.IsNullOrEmpty(values[j]))
                    {
                        try
                        {
                            object value = Convert.ChangeType(values[j], property.PropertyType);
                            property.SetValue(obj, value);
                        }
                        catch (Exception)
                        {
                            // Gérer l'erreur de conversion
                            if (property.PropertyType == typeof(int))
                                property.SetValue(obj, 0);
                            else if (property.PropertyType == typeof(decimal))
                                property.SetValue(obj, 0m);
                            else
                                property.SetValue(obj, string.Empty);
                        }
                    }
                }

                // Vérifier que l'objet livre a au moins un ISBN avant de l'ajouter
                if (!string.IsNullOrWhiteSpace(obj.ISBN))
                {
                    list.Add(obj);
                }
            }
        }
        return list;
    }

    public async Task PrintData<T>(List<T> data)
    {
        var csv = new StringBuilder();
        var properties = typeof(T).GetProperties();
        csv.AppendLine(string.Join(";", properties.Select(p => p.Name)));

        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
            csv.AppendLine(string.Join(";", values));
        }
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));
        var fileSaverResult = await FileSaver.Default.SaveAsync("Books.csv", stream);

        if (fileSaverResult.IsSuccessful)
        {
            await Shell.Current.DisplayAlert("Succès", "Le fichier CSV a été sauvegardé avec succès", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Erreur", "Une erreur est survenue lors de la sauvegarde du fichier CSV", "OK");
        }
    }
}