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
        private ScaleTransform _scaleTransform = new ScaleTransform(1, 1);
        private Brush _currentColor = Brushes.White;
        private string _pseudo;
        private List<Point> _pixelClickedList = new List<Point>();
        private System.Timers.Timer _timer;
        private SqlServices _sqlServices;
        private string _currentColorFormatted = "#FFFFFF";

        // Constructeur principal qui initialise la fenêtre, le pseudo et les composants
        public MainWindow(string pseudo)
        {
            InitializeComponent();
            _pseudo = pseudo;
            TxtPseudo.Text = _pseudo;
            

            _scaleTransform.ScaleX = 0.24912826983452546;
            _scaleTransform.ScaleY = 0.24912826983452546;
            GrilleCanvas.LayoutTransform = _scaleTransform;

            Grille(100, 200, 20);

            _sqlServices = new SqlServices();
            var colorPalette = new ColorPalette();
            colorPalette.ColorChanged += OnColorChanged;
            ColorButtonHost.Child = colorPalette.Controls[0];
            this.MouseWheel += MainWindow_MouseWheel;
            GrilleCanvas.PreviewMouseDown += GrilleCanvasDoubleClick;

            _timer = new System.Timers.Timer(1500);
            _timer.Elapsed += SendPixelList;
            _timer.AutoReset = true;
            _timer.Start();
        }

        // Gestion du changement de couleur sélectionnée dans la palette
        private void OnColorChanged(System.Drawing.Color newColor)
        {
            var wpfColor = System.Windows.Media.Color.FromArgb(
                newColor.A, newColor.R, newColor.G, newColor.B);
            _currentColor = new SolidColorBrush(wpfColor);
            _currentColorFormatted = $"#{wpfColor.R:X2}{wpfColor.G:X2}{wpfColor.B:X2}";
        }

        // Génère la grille de pixels en fonction des paramètres donnés
        private void Grille(int rows, int cols, double pixelSize)
        {
            GrilleCanvas.Children.Clear();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Rectangle pixel = new Rectangle
                    {
                        Width = pixelSize,
                        Height = pixelSize,
                        Fill = Brushes.LightGray,
                        Stroke = Brushes.DarkGray,
                        StrokeThickness = 0.1
                    };

                    Canvas.SetLeft(pixel, col * pixelSize);
                    Canvas.SetTop(pixel, row * pixelSize);
                    GrilleCanvas.Children.Add(pixel);

                    pixel.MouseDown += Pixel_MouseDown;
                }
            }

            GrilleCanvas.Width = cols * pixelSize;
            GrilleCanvas.Height = rows * pixelSize;
        }

        // Gère le zoom de la grille avec la molette de la souris
        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            double minZoom = 0.2;
            double maxZoom = 5.0;

            if (_scaleTransform.ScaleX * zoomFactor >= minZoom && _scaleTransform.ScaleX * zoomFactor <= maxZoom)
            {
                _scaleTransform.ScaleX *= zoomFactor;
                _scaleTransform.ScaleY *= zoomFactor;
            }
        }

        // Gère le clic sur un pixel et l'ajoute à la liste des pixels cliqués
        private void Pixel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle pixel)
            {
                pixel.Fill = _currentColor;
                var position = e.GetPosition(GrilleCanvas);
                double pixelSize = pixel.Width;
                int col = (int)(position.X / pixelSize);
                int row = (int)(position.Y / pixelSize);
                _pixelClickedList.Add(new Point(col, row));
            }
        }

        // Envoie la liste des pixels cliqués à la base de données toutes les 3 secondes
        private void SendPixelList(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<Pixel> pixelList = _pixelClickedList
                .Select(p => new Pixel
                {
                    Name = _pseudo,
                    Cos = $"{p.X},{p.Y}",
                    Color = _currentColorFormatted,
                    Date = DateTime.Now
                })
                .ToList();

            _sqlServices.ListUpdate(pixelList);
            _pixelClickedList.Clear();

            List<Pixel> retrievedPixels = _sqlServices.RetrievePixelList();
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var pixel in retrievedPixels)
                {
                    var coords = pixel.Cos.Split(',');
                    int col = int.Parse(coords[0]);
                    int row = int.Parse(coords[1]);

                    foreach (var child in GrilleCanvas.Children)
                    {
                        if (child is Rectangle rect)
                        {
                            var left = Canvas.GetLeft(rect);
                            var top = Canvas.GetTop(rect);

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

        // Gère le double-clic sur la grille (uniquement pour afficher les coordonnées pour le moment)
        private void GrilleCanvasDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var position = e.GetPosition(GrilleCanvas);
                double pixelSize = 20;
                int col = (int)(position.X / pixelSize);
                int row = (int)(position.Y / pixelSize);
                Console.WriteLine($"Double-clic sur le pixel ({col}, {row})");
            }
        }
    }
}
