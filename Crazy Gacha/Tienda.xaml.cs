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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Crazy_Gacha
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class Tienda : UserControl
    {
        public Tienda()
        {
            InitializeComponent();
        }

        public string NombreMejora
        {
            get { return lblNombreMejora.Content.ToString(); }
            set
            {
                lblNombreMejora.Content = value;
            }
        }

        public float Precio
        {
            get { return float.Parse(lblPrecioMejora.Content.ToString());}
            set { lblPrecioMejora.Content = value.ToString(); }
        }

        public int Cantidad
        {
            get { return int.Parse(lblCantidad.Content.ToString()); }
            set { lblCantidad.Content = value.ToString(); }
        }

        public ImageSource ImagenMejora
        {
            get { return imgMejora.Source; }
            set
            {
                if(imgMejora != null)
                {
                    imgMejora.Source = value;
                }
            }
        }

        public ImageSource ImagenMoneda
        {
            get { return imgMoneda.Source; }
            set
            {
                imgMoneda.Source = new BitmapImage(new Uri("moneda.png", UriKind.Relative));
            }
        }
    }
}
