using System.Data;
using Dapper;
using MySqlConnector;
namespace Frame; 
public class SqlServices
{
    // Chaîne de connexion fixe à la base de données 
    private readonly string _connectionString = "Server=192.168.1.97;Port=3306;Database=callofpixels;Uid=sqlcommuser;Pwd=GHU7L8jxrs4RBjsB;";
    
    // Déclaration de la connexion à la base de données
    private MySqlConnection connection;

    // Constructeur pour initialiser la connexion
    public SqlServices()
    {
        connection = new MySqlConnection(_connectionString);
    }

    // Méthode pour établir la connexion
    public void OpenConnection()
    {
        try
        {
            Console.WriteLine("Connecting...");
            connection.Open();
            Console.WriteLine("Connected\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to the database: {ex.Message}");
        }
    }

    public async Task<bool> ConnectAsync()
    {
        try
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            // Vérification de la connectivité 
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT 1";
                await command.ExecuteScalarAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while connecting: {ex.Message}");
            return false;
        }
    }


    
    // Ajout de Pixel
    public void AddPixel(Pixel pixel)
    {
        Console.WriteLine("Adding Pixel function\n");
        
        // SQL pour ajouter un pixel
        var sql = "INSERT INTO jeu (Name, Cos, Color, Date) VALUES (@Name, @Cos, @Color, NOW())";
        connection.Execute(sql, new { pixel.Name, pixel.Cos, pixel.Color });
        Console.WriteLine($"{pixel.Name}'s Pixel with {pixel.Color} color added in position {pixel.Cos} on {DateTime.Now}\n");
    }
    
    
    // Update Pixel
    public void UpdatePixel(Pixel pixel)
    {
        Console.WriteLine("Updating Pixel function\n");
        using (var connection = new MySqlConnection(_connectionString))
        {
            Console.WriteLine("(update) Connecting...");
            connection.Open();
            Console.WriteLine("Connected\n");

            // Requête pour vérifier si un pixel existe déjà aux coordonnées spécifiées
            var searchSql = "SELECT COUNT(*) FROM jeu WHERE Cos = @Cos";
            int pixelCount = connection.ExecuteScalar<int>(searchSql, new { pixel.Cos });

            if (pixelCount > 0)
            {
                // Si un pixel existe, update
                var updateSql = "UPDATE jeu SET Name = @Name, Color = @Color, Date = NOW() WHERE Cos = @Cos";
                connection.Execute(updateSql, new { pixel.Name, pixel.Cos, pixel.Color });

                Console.WriteLine($"{pixel.Name}'s Pixel updated with {pixel.Color} color at position {pixel.Cos}\n");
            }
            else
            {
                // Si aucun pixel n'existe aux coordonnées spécifiées, on l'ajoute à la DB
                
                Console.WriteLine("Not an existing pixel, creating a new one\n");
              
                
                var sql = "INSERT INTO jeu (Name, Cos, Color, Date) VALUES (@Name, @Cos, @Color, NOW())";
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
                var searchSql = "SELECT COUNT(*) FROM jeu WHERE Cos = @Cos";
                int pixelCount = connection.ExecuteScalar<int>(searchSql, new { pixel.Cos });

                if (pixelCount > 0)
                {
                    // Si un pixel existe, update
                    var updateSql = "UPDATE jeu SET Name = @Name, Color = @Color, Date = NOW() WHERE Cos = @Cos";
                    connection.Execute(updateSql, new { pixel.Name, pixel.Cos, pixel.Color });

                    Console.WriteLine($"{pixel.Name}'s Pixel updated with {pixel.Color} color at position {pixel.Cos}\n");
                }
                else
                {
                    // Si aucun pixel n'existe aux coordonnées spécifiées, on l'ajoute à la DB
                
                    Console.WriteLine("Not an existing pixel, creating a new one\n");
                
                    var sql = "INSERT INTO jeu (Name, Cos, Color, Date) VALUES (@Name, @Cos, @Color, NOW())";
                    connection.Execute(sql, new { pixel.Name, pixel.Cos, pixel.Color });
                
                    Console.WriteLine($"{pixel.Name}'s Pixel with {pixel.Color} color added in position {pixel.Cos} on {DateTime.Now}\n");
                }  
            }
        }
    }
    
    public List<Pixel> RetrievePixelList()
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            Console.WriteLine("Connecting...");
            connection.Open();
            Console.WriteLine("Connected\n");

            // Requête pour récupérer tous les pixels de la table
            var sql = "SELECT Id, Name, Cos, Color FROM jeu";

            // Exécuter la requête et mapper les résultats à la classe Pixel
            var pixels = connection.Query<Pixel>(sql).ToList();

            Console.WriteLine("Pixels retrieved successfully!");

            // Retourner la liste des pixels
            return pixels;
        }
    }
}
