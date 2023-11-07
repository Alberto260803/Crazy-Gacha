using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Crazy_Gacha
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Premios : Window
    {
        public Premios()
        {
            InitializeComponent();
        }

        public ImageSource ImagenPremio
        {
            get { return imgPremio.Source; }
            set
            {
                if (imgPremio != null)
                {
                    imgPremio.Source = value;
                }
            }
        }

        public string Rareza
        {
            get
            {
                return lblRareza.Text.ToString();
            }

            set
            {
                switch (value)
                {
                    case "Común": lblRareza.Foreground = Brushes.Black; break;
                    case "Rara": lblRareza.Foreground = Brushes.Green; break;
                    case "Especial": lblRareza.Foreground = Brushes.Aquamarine; break;
                    case "Épica": lblRareza.Foreground= Brushes.Purple; break;
                    case "Legendaria": lblRareza.Foreground = Brushes.Gold; break;
                    default: lblRareza.Foreground = Brushes.Black; break;
                }
                lblRareza.Text = value.ToUpper();       
            }
        }

        public string Nombre
        {
            get
            {
                return lblNombre.Text.ToString();
            }

            set
            {
                lblNombre.Text = value;  
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var ventanaPrincipal = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        }
    }
}
