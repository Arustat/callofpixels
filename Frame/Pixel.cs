using System.Windows.Media;
using System.Windows.Shapes;

namespace Frame
{
    public class Pixel
    {
        public Rectangle Rect { get; set; } // Rectangle représentant le pixel
        public SolidColorBrush Color { get; set; } // Couleur du pixel
        public int X { get; set; } // Coordonnée X du pixel
        public int Y { get; set; } // Coordonnée Y du pixel

        // Constructeur de la classe Pixel
        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
            Color = new SolidColorBrush(Colors.White); // Par défaut, la couleur est blanche
            Rect = new Rectangle
            {
                Width = 10,
                Height = 10,
                Fill = Color,
                Stroke = new SolidColorBrush(Colors.Gray),
                StrokeThickness = 0.5
            };
        }
    }
}