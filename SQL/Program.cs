using Dapper;
using MySql.Data.MySqlClient; // Utiliser MySqlConnection

public class SqlServices
{
    // Chaîne de connexion fixe à la base de données 
    private readonly string _connectionString = "Server=localhost;Port=3306;Database=callofpixel;Uid=root;";

    // Fonction d'ajout de Pixel
    public void AddPixel(string name, string cos, string color)
    {
        using (var connection = new MySqlConnection(_connectionString)) // Utilisation de MySqlConnection
        {
            Console.WriteLine("Connecting...");
            connection.Open();
            Console.WriteLine("Connected");

            var sql = "INSERT INTO jeu1 (Name, Cos, Color) VALUES (@Name, @Cos, @Color)";
            connection.Execute(sql, new { Name = name, Cos = cos, Color = color });

            Console.WriteLine($"{name}'s Pixel with {color} color added in position {cos}");
        }
    }
}

class SQLProgram
{
    static void Main(string[] args)
    {
        Console.WriteLine("CallofPixels SQL Communication Service");
        Console.WriteLine("================================\n");
        
        // Créer une instance de SqlServices pour la connexion à la DB
        
        var pixelService = new SqlServices();
        
        pixelService.AddPixel("RougeRaph", "15,41", "#3ed63c");
        pixelService.AddPixel("May", "53,124", "#ff6e4f");
    }
}