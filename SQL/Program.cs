using Dapper;
using MySql.Data.MySqlClient; // Utiliser MySqlConnection
namespace SQL; 
public class SqlServices
{
    // Chaîne de connexion fixe à la base de données 
    private readonly string _connectionString = "Server=localhost;Database=callofpixel;Uid=root;";

    // Ajout de Pixel
    public void AddPixel(Pixel pixel)
    {
        Console.WriteLine("Adding Pixel function\n");
        using (var connection = new MySqlConnection(_connectionString))
        {
            //Connection à la DB
            Console.WriteLine("Connecting...");
            connection.Open();
            Console.WriteLine("Connected\n");

            // SQL pour ajouter un pixel
            var sql = "INSERT INTO jeu1 (Name, Cos, Color, Date) VALUES (@Name, @Cos, @Color, NOW())";
            connection.Execute(sql, new { pixel.Name, pixel.Cos, pixel.Color });

            Console.WriteLine($"{pixel.Name}'s Pixel with {pixel.Color} color added in position {pixel.Cos} on {DateTime.Now}\n");
        }
    }
    
    
    // Update Pixel
    public void UpdatePixel(Pixel pixel)
    {
        Console.WriteLine("Updating Pixel function\n");
        using (var connection = new MySqlConnection(_connectionString))
        {
            Console.WriteLine("Connecting...");
            connection.Open();
            Console.WriteLine("Connected\n");

            // Requête pour vérifier si un pixel existe déjà aux coordonnées spécifiées
            var searchSql = "SELECT COUNT(*) FROM jeu1 WHERE Cos = @Cos";
            int pixelCount = connection.ExecuteScalar<int>(searchSql, new { pixel.Cos });

            if (pixelCount > 0)
            {
                // Si un pixel existe, update
                var updateSql = "UPDATE jeu1 SET Name = @Name, Color = @Color, Date = NOW() WHERE Cos = @Cos";
                connection.Execute(updateSql, new { pixel.Name, pixel.Cos, pixel.Color });

                Console.WriteLine($"{pixel.Name}'s Pixel updated with {pixel.Color} color at position {pixel.Cos}\n");
            }
            else
            {
                // Si aucun pixel n'existe aux coordonnées spécifiées, on l'ajoute à la DB
                
                Console.WriteLine("Not an existing pixel, creating a new one\n");
                
                var sql = "INSERT INTO jeu1 (Name, Cos, Color, Date) VALUES (@Name, @Cos, @Color, NOW())";
                connection.Execute(sql, new { pixel.Name, pixel.Cos, pixel.Color });
                
                Console.WriteLine($"{pixel.Name}'s Pixel with {pixel.Color} color added in position {pixel.Cos} on {DateTime.Now}\n");
            }
        }
    }

    //Update d'une liste de Pixels
    public void ListUpdate(List<Pixel> pixels)
    {
        Console.WriteLine("Updating Pixel list function\n");
        using (var connection = new MySqlConnection(_connectionString))
        {
            Console.WriteLine("Connecting...");
            connection.Open();
            Console.WriteLine("Connected\n");

            foreach (var pixel in pixels)
            {
                // Requête pour vérifier si un pixel existe déjà aux coordonnées spécifiées
                var searchSql = "SELECT COUNT(*) FROM jeu1 WHERE Cos = @Cos";
                int pixelCount = connection.ExecuteScalar<int>(searchSql, new { pixel.Cos });

                if (pixelCount > 0)
                {
                    // Si un pixel existe, update
                    var updateSql = "UPDATE jeu1 SET Name = @Name, Color = @Color, Date = NOW() WHERE Cos = @Cos";
                    connection.Execute(updateSql, new { pixel.Name, pixel.Cos, pixel.Color });

                    Console.WriteLine($"{pixel.Name}'s Pixel updated with {pixel.Color} color at position {pixel.Cos}\n");
                }
                else
                {
                    // Si aucun pixel n'existe aux coordonnées spécifiées, on l'ajoute à la DB
                
                    Console.WriteLine("Not an existing pixel, creating a new one\n");
                
                    var sql = "INSERT INTO jeu1 (Name, Cos, Color, Date) VALUES (@Name, @Cos, @Color, NOW())";
                    connection.Execute(sql, new { pixel.Name, pixel.Cos, pixel.Color });
                
                    Console.WriteLine($"{pixel.Name}'s Pixel with {pixel.Color} color added in position {pixel.Cos} on {DateTime.Now}\n");
                }  
            }
        }
    }

    
}

class SQLProgram
{
    static void Main(string[] args)
    {
        Console.WriteLine("CallofPixels SQL Communication Services");
        Console.WriteLine("======================================\n");

        var pixelService = new SqlServices();

        List<Pixel> pixels = new List<Pixel>
        {
            new Pixel { Name = "May", Cos = "15,41", Color = "#3ed63c" },
            new Pixel { Name = "Raff", Cos = "53,124", Color = "#ff6e4f" },
            new Pixel { Name = "AH", Cos = "200,500", Color = "#123abc" }
        };
        
        pixelService.ListUpdate(pixels);

    }
}
