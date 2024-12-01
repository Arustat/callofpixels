using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Frame
{
    public partial class MainWindow : Window
    {
        private ScaleTransform _scaleTransform = new ScaleTransform(1, 1); // Pour gérer le zoom
        private Brush _currentColor = Brushes.White; // La couleur actuelle des pixels
        private string _pseudo; // Nouveau champ pour le pseudo
        private List<Point> _pixelClickedList = new List<Point>(); // Liste pour stocker les pixels cliqués
        private System.Timers.Timer _timer; // Timer pour envoyer la liste des pixels toutes les 3 secondes
        private SqlServices _sqlServices; // Instance de SqlServices
        private string _currentColorFormatted = "#FFFFFF"; // Variable pour la couleur actuelle au format hex

        public MainWindow(string pseudo)
        {
            InitializeComponent();
            _pseudo = pseudo; // Enregistrer le pseudo
            TxtPseudo.Text = _pseudo;
            

            // Initialement zoom
            _scaleTransform.ScaleX = 0.24912826983452546;
            _scaleTransform.ScaleY = 0.24912826983452546;

            // Appliquer le ScaleTransform à la grille pour permettre le zoom
            GrilleCanvas.LayoutTransform = _scaleTransform;

            // Générer la grille de pixels
            Grille(100, 200, 20); // Grille de 100x200 avec des pixels de taille 20x20

            // Initialisation de la classe SqlServices pour l'interaction avec la base de données
            _sqlServices = new SqlServices();

            var colorPalette = new ColorPalette();

            // S'abonner à l'événement ColorChanged pour récupérer la couleur
            colorPalette.ColorChanged += OnColorChanged;

            // Ajouter le bouton Windows Forms au conteneur WindowsFormsHost
            ColorButtonHost.Child = colorPalette.Controls[0];

            // Ajouter l'événement MouseWheel pour le zoom
            this.MouseWheel += MainWindow_MouseWheel;

            // Ajouter l'événement de double-clic pour zoomer sur un point spécifique
            GrilleCanvas.PreviewMouseDown += GrilleCanvasDoubleClick;

            // Initialisation du timer pour envoyer les pixels toutes les 3 secondes
            _timer = new System.Timers.Timer(1500); // Définir le timer pour toutes les 3 secondes
            _timer.Elapsed += SendPixelList;
            _timer.AutoReset = true;
            _timer.Start();
        }

        // Méthode pour recevoir la nouvelle couleur et mettre à jour _currentColor
        private void OnColorChanged(System.Drawing.Color newColor)
        {
            // Convertir la couleur choisie de System.Drawing.Color à System.Windows.Media.Color
            var wpfColor = System.Windows.Media.Color.FromArgb(
                newColor.A, newColor.R, newColor.G, newColor.B);

            // Convertir en SolidColorBrush et mettre à jour _currentColor
            _currentColor = new SolidColorBrush(wpfColor);
            _currentColorFormatted = $"#{wpfColor.R:X2}{wpfColor.G:X2}{wpfColor.B:X2}"; // Format hex
            Console.WriteLine($"Couleur sélectionnée : {_currentColorFormatted}"); // Afficher la couleur dans la console
        }

        // Méthode pour générer la grille de pixels
        private void Grille(int rows, int cols, double pixelSize)
        {
            // Effacer la grille précédente
            GrilleCanvas.Children.Clear();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Créer un rectangle pour chaque pixel
                    Rectangle pixel = new Rectangle
                    {
                        Width = pixelSize,
                        Height = pixelSize,
                        Fill = Brushes.LightGray, // Couleur par défaut du pixel
                        Stroke = Brushes.DarkGray, // Bordure des pixels
                        StrokeThickness = 0.1
                    };

                    // Positionner chaque pixel sur le canvas
                    Canvas.SetLeft(pixel, col * pixelSize);
                    Canvas.SetTop(pixel, row * pixelSize);

                    // Ajouter le pixel au Canvas
                    GrilleCanvas.Children.Add(pixel);

                    // Ajouter un événement MouseDown pour changer la couleur du pixel lorsqu'on clique dessus
                    pixel.MouseDown += Pixel_MouseDown;
                }
            }

            // Ajuster les dimensions du Canvas en fonction de la taille de la grille
            GrilleCanvas.Width = cols * pixelSize;
            GrilleCanvas.Height = rows * pixelSize;
        }

        // Gestion du zoom lors du défilement de la souris
        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9; // Facteur de zoom

            // Limite minimale de zoom (20%)
            double minZoom = 0.2;

            // Limite maximale de zoom (200%)
            double maxZoom = 5.0;

            // Appliquer le zoom seulement si la nouvelle échelle est dans les limites
            if (_scaleTransform.ScaleX * zoomFactor >= minZoom && _scaleTransform.ScaleX * zoomFactor <= maxZoom)
            {
                _scaleTransform.ScaleX *= zoomFactor;
                _scaleTransform.ScaleY *= zoomFactor;
            }
        }

        // Gestion du clic sur un pixel pour changer sa couleur
        private void Pixel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle pixel)
            {
                // Changer la couleur du pixel au clic
                pixel.Fill = _currentColor;

                // Récupérer les coordonnées du clic sans tenir compte du zoom
                var position = e.GetPosition(GrilleCanvas);

                // Calculer la colonne et la ligne
                double pixelSize = pixel.Width; // Taille du pixel
                int col = (int)(position.X / pixelSize); // Calcul de la colonne
                int row = (int)(position.Y / pixelSize); // Calcul de la ligne

                // Ajouter les coordonnées de la colonne et de la ligne à la liste
                _pixelClickedList.Add(new Point(col, row)); // Stocker la colonne et la ligne dans la liste
            }
        }

        // Fonction appelée par le timer toutes les 3 secondes
        private void SendPixelList(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Convertir les points (col, row) en Pixels pour l'envoi
            List<Pixel> pixelList = _pixelClickedList
                .Select(p => new Pixel
                {
                    Name = _pseudo,
                    Cos = $"{p.X},{p.Y}", // Cos contient maintenant les coordonnées col,row
                    Color = _currentColorFormatted, // Utiliser la couleur formatée
                    Date = DateTime.Now
                })
                .ToList();

            // Envoyer à la base de données
            foreach (var pixel in pixelList)
            {
                Console.WriteLine($"Pixel: Name={pixel.Name}, Cos={pixel.Cos}, Color={pixel.Color}, Date={pixel.Date}");
            }

            _sqlServices.ListUpdate(pixelList);

            // Vider la liste des pixels après l'envoi
            _pixelClickedList.Clear(); 

            // Récupérer la liste des pixels déjà présents dans la base de données
            List<Pixel> retrievedPixels = _sqlServices.RetrievePixelList();

            // Mettre à jour la grille avec les pixels récupérés
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var pixel in retrievedPixels)
                {
                    // Transformer les coordonnées de "Cos" (ex: "10,20") en deux valeurs X et Y
                    var coords = pixel.Cos.Split(',');
                    int col = int.Parse(coords[0]);
                    int row = int.Parse(coords[1]);

                    // Trouver le rectangle correspondant dans la grille
                    foreach (var child in GrilleCanvas.Children)
                    {
                        if (child is Rectangle rect)
                        {
                            var left = Canvas.GetLeft(rect);
                            var top = Canvas.GetTop(rect);

                            // Comparer les coordonnées pour voir si on doit changer la couleur du pixel
                            if (Math.Abs(left - col * rect.Width) < 0.1 && Math.Abs(top - row * rect.Height) < 0.1)
                            {
                                rect.Fill = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(pixel.Color));
                                break;
                            }
                        }
                    }
                }
            });
        }

        // Méthode pour gérer le double-clic pour le zoom
        private void GrilleCanvasDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Calculer la position du double-clic
                var position = e.GetPosition(GrilleCanvas);

                // Calculer les coordonnées du pixel
                double pixelSize = 20;
                int col = (int)(position.X / pixelSize);
                int row = (int)(position.Y / pixelSize);

                // Afficher un message de debug pour la position du double-clic
                Console.WriteLine($"Double-clic sur le pixel ({col}, {row})");
            }
        }
    }
}
