using Microcharts;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class GraphViewModel : ObservableObject
{
    [ObservableProperty]
    public partial Chart MyObservableChart { get; set; } = new BarChart();

    public GraphViewModel()
    {
        RefreshPage();
    }

    internal void RefreshPage()
    {
        // Si aucun livre n'est disponible, créer un graphique par défaut
        if (Globals.MyBooks == null || Globals.MyBooks.Count == 0)
        {
            CreateDefaultChart();
            return;
        }

        // Créer un graphique basé sur les catégories de livres
        CreateCategoryStockChart();
    }

    private void CreateDefaultChart()
    {
        ChartEntry[] entries = new[]
        {
            new ChartEntry(0)
            {
                Label = "Aucune donnée",
                ValueLabel = "0",
                Color = SKColor.Parse("#2c3e50")
            }
        };

        Chart myChart = new BarChart
        {
            Entries = entries,
            LabelTextSize = 40,
            BackgroundColor = SKColors.Transparent
        };

        MyObservableChart = myChart;
    }

    private void CreateCategoryStockChart()
    {
        // Regrouper les livres par catégorie et calculer le stock total par catégorie
        var categoriesData = Globals.MyBooks
            .GroupBy(b => string.IsNullOrEmpty(b.Category) ? "Non catégorisé" : b.Category)
            .Select(g => new
            {
                Category = g.Key,
                TotalStock = g.Sum(b => b.Stock)
            })
            .OrderByDescending(x => x.TotalStock)
            .Take(10) // Limiter à 10 catégories pour la lisibilité
            .ToList();

        // Créer les entrées du graphique pour chaque catégorie
        List<ChartEntry> entries = new List<ChartEntry>();

        // Palette de couleurs variées pour le graphique
        string[] colorPalette = new[] {
            "#3498db", "#2ecc71", "#e74c3c", "#f39c12", "#9b59b6",
            "#1abc9c", "#d35400", "#34495e", "#16a085", "#c0392b"
        };

        for (int i = 0; i < categoriesData.Count; i++)
        {
            var data = categoriesData[i];
            entries.Add(new ChartEntry(data.TotalStock)
            {
                Label = data.Category,
                ValueLabel = data.TotalStock.ToString(),
                Color = SKColor.Parse(colorPalette[i % colorPalette.Length])
            });
        }

        // Créer le graphique avec les entrées
        Chart myChart = new BarChart
        {
            Entries = entries.ToArray(),
            LabelTextSize = 40,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            BackgroundColor = SKColors.Transparent
        };

        MyObservableChart = myChart;
    }
}