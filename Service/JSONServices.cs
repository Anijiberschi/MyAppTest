﻿using Microsoft.Maui.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.Service;

public class JSONServices
{
    internal async Task<List<Book>> GetBooks()
    {
        try
        {
            // D'abord essayer de récupérer depuis le serveur
            //var url = "http://localhost:32774/json?FileName=MyBooks.json";
            var url = "https://185.157.245.38:5000/json?FileName=MyBooks.json";

            List<Book> MyList = new();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            HttpClient _httpClient = new HttpClient(handler);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Succes to fetch the json log");
                var content = await response.Content.ReadAsStreamAsync();
                MyList = JsonSerializer.Deserialize<List<Book>>(content) ?? new List<Book>();
            }
            else
            {
                Console.WriteLine("Failed to fetch from server, loading from local file.");
                using var stream = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/books.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                MyList = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }

            return MyList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // En cas d'erreur, retourner une liste vide
            return new List<Book>();
        }
    }

    internal async Task SetBooks(List<Book> MyList)
    {
        //var url = "http://localhost:32774/json";
        var url = "https://185.157.245.38:5000/json";

        MemoryStream mystream = new();

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        HttpClient _httpClient = new HttpClient(handler);

        JsonSerializer.Serialize(mystream, MyList);

        mystream.Position = 0;

        var fileContent = new ByteArrayContent(mystream.ToArray());

        var content = new MultipartFormDataContent
        {
            { fileContent, "file", "MyBooks.json"}
        };

        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            // Notifier l'utilisateur du succès
            await Shell.Current.DisplayAlert("Succès", "Les données ont été sauvegardées avec succès sur le serveur", "OK");
        }
        else
        {
            // Gérer les erreurs
            await Shell.Current.DisplayAlert("Erreur", "Une erreur est survenue lors de la sauvegarde des données", "OK");
        }
    }
}