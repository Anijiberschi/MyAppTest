using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model;
public class Book
{
    public string ISBN { get; set; } = string.Empty;  // Code barre / ISBN
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Stock { get; set; } = 0;
    public decimal Price { get; set; } = 0;
    public string CoverImage { get; set; } = string.Empty;
}