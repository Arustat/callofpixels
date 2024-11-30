using System;
using System.Collections.Generic;
using MySqlConnector;
using Dapper;

namespace SQL
{
    public class ServicePixel
    {
        private readonly string _chaineConnexion;

        // Constructeur pour initialiser la chaîne de connexion à la base de données
        public ServicePixel(string chaineConnexion)
        {
            _chaineConnexion = chaineConnexion;
        }

        // Fonction pour ajouter un nouveau pixel dans la base de données
        public bool AjouterPixel(Pixel nouveauPixel)
        {
            Console.WriteLine("Ajout d'un nouveau pixel...");

            // Vérification si le nouveauPixel n'est pas null
            if (nouveauPixel == null)
            {
                Console.WriteLine("Erreur : le pixel à ajouter est null.");
                return false;
            }

            // Insertion du pixel dans la base de données
            using (var connexion = new MySqlConnection(_chaineConnexion))
            {
                try
                {
                    connexion.Open();
                    const string sql =
                        "INSERT INTO jeu (Id, Name, Cos, Color, Date) VALUES (@Id, @Name, @Cos, @Color, @Date)";

                    // Exécution de l'insertion
                    var lignesInserees = connexion.Execute(sql, new { nouveauPixel.Id, nouveauPixel.Name, nouveauPixel.Cos, nouveauPixel.Color, nouveauPixel.Date });

                    // Affichage du nombre de lignes insérées
                    if (lignesInserees > 0)
                    {
                        Console.WriteLine($"Le pixel a été ajouté avec succès : Id = {nouveauPixel.Id}, Name = {nouveauPixel.Name}, Cos = {nouveauPixel.Cos}, Color = {nouveauPixel.Color}, Date = {nouveauPixel.Date}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Échec de l'ajout du pixel.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'ajout du pixel : {ex.Message}");
                    return false;
                }
            }
        }
    }
    // Classe principale pour exécuter l'application
    internal class Program
    {
        private static void Main()
        {
            // Chaîne de connexion à la base de données MySQL
            var chaineConnexion =
                "Server=192.168.1.97;Port=3306;Database=callofpixels;Uid=sqlcommuser;Pwd=GHU7L8jxrs4RBjsB;";

            // Création d'une instance du service de gestion des pixels
            var servicePixel = new ServicePixel(chaineConnexion);

            // Création d'un nouveau pixel à ajouter
            Pixel monNouveauPixel = new Pixel
            {
                Id = 3,
                Name = "Pixel3",
                Cos = "3,3",
                Color = "#00FF00",
                Date = DateTime.Now
            };

            // Ajout du nouveau pixel
            var ajoutReussi = servicePixel.AjouterPixel(monNouveauPixel);
            Console.WriteLine($"Ajout du pixel réussi : {ajoutReussi}");
        }
    }
}
