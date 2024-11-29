using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace Frame
{
    public partial class Start : Window
    {
        private DispatcherTimer _timer;
        private double _progressValue = 0;

        public Start()
        {
            InitializeComponent();

            // Initialiser le timer avant de démarrer
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100); // Intervalle pour mettre à jour la barre
            _timer.Tick += Timer_Tick;

            // Démarrer l'animation de la barre de progression une fois le timer prêt
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Incrémenter la valeur de la barre de progression
            _progressValue += 5;
            LoadingBar.Value = _progressValue;

            // Une fois que la barre est remplie (100), changer l'image de fond
            if (_progressValue >= 100)
            {
                _timer.Stop(); // Arrêter le Timer

                // Appeler la fonction pour changer l'image de fond
                Backhround_image("pack://application:,,,/img/back2.png"); // Remplacer par le chemin de ton image
                Loagind.Visibility = Visibility.Hidden;
                LoadingBar.Visibility = Visibility.Hidden; // Cacher la barre de progression
                TextPseudo.Visibility = Visibility.Visible;
                PseudoBox.Visibility = Visibility.Visible; // Afficher le champ pseudo
                StartButton.Visibility = Visibility.Visible; // Afficher le bouton de démarrage
            }
        }

        // Méthode pour changer l'image de fond
        private void Backhround_image(string imagePath)
        {
            // Créer un objet ImageBrush avec la nouvelle image
            ImageBrush newBackground = new ImageBrush();
            newBackground.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

            // Appliquer l'ImageBrush comme arrière-plan de la fenêtre
            this.Background = newBackground;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier si un pseudo a été saisi
            string pseudo = PseudoBox.Text;
            if (string.IsNullOrWhiteSpace(pseudo))
            {
                MessageBox.Show("Please enter a pseudo!");
                return;
            }

            // Ouvrir la fenêtre du jeu avec le pseudo de l'utilisateur
            MainWindow gameWindow = new MainWindow(pseudo);
            gameWindow.Show();
            this.Close();
        }
    }
}
