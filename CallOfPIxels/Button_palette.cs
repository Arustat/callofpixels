//Classe Button qui va régir le button

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Frame;

public class ColorPalette: Form
{
    private Button colorButton;
    public event Action<System.Drawing.Color> ColorChanged;
    public ColorPalette()
    {
        colorButton = new Button
        {
            BackColor = Color.White, //Couleur par défault button
            Width = 100,
            Height = 100,
            FlatStyle = FlatStyle.Flat, // Utilisation du style plat (sans ombres ni effets)
            FlatAppearance = 
            {
                BorderSize = 3, // Taille de la bordure
                BorderColor = Color.Black // Couleur de la bordure (noir)
            }
        };
        
        
        colorButton.Region = new Region(new Rectangle(0, 0, 100, 100)); // Cercle parfait
        colorButton.Location = new Point((this.ClientSize.Width - colorButton.Width) / 2, 
            this.ClientSize.Height - colorButton.Height - 10);
        
        //Ajout de l'événement au clic du bouton
        colorButton.Click += ButtonClick;
        
        //Ajoute le bouton à la fenêtre
        Controls.Add(colorButton);
    }

    private void ButtonClick(object sender, EventArgs e)
    {
        // Afficher la palette de couleurs
        using (ColorDialog colorDialog = new ColorDialog())
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Appliquer la couleur choisie au bouton
                colorButton.BackColor = colorDialog.Color;
                ColorChanged?.Invoke(colorDialog.Color);
            }
        }
    }
    
    
}