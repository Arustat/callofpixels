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
        
        public MainWindow()
        {
            InitializeComponent();
            
            GrilleCanvas.LayoutTransform = _scaleTransform;

            // Générer la grille de pixels
            Grille(100, 200, 2);
            
            this.MouseWheel += MainWindow_MouseWheel;
        }

        private void Grille(int rows, int cols, double pixelSize)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    //En c# le square n'existe pas alors j'utilise le Rectangle 
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
                }
            }
            
            GrilleCanvas.Width = cols * pixelSize;
            GrilleCanvas.Height = rows * pixelSize;
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            _scaleTransform.ScaleX *= zoomFactor;
            _scaleTransform.ScaleY *= zoomFactor;
        }

        
    }
}