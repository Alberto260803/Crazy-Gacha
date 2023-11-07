using MySql.Data.MySqlClient;
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
    /// Lógica de interacción para Inicio.xaml
    /// </summary>
    public partial class Inicio : Window
    {
        public string nombre;
        public string pass;
        List<string> errorMessages = new List<string>();
        private Recursos recursos = new Recursos();
        public Inicio()
        {
            InitializeComponent();
        }

        private void btEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(recursos.ConectarBD());
                conn.Open();
                nombre = tbUsuario.Text;
                pass = tbPassword.Password;

                string query = "SELECT * FROM usuario WHERE nombre=@usuario AND password=@contra";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@usuario", nombre);
                command.Parameters.AddWithValue("@contra", pass);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    MainWindow mainWindow = new MainWindow(nombre);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    errorMessages.Add("Usuario o contraseña incorrectos");
                }
                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                errorMessages.Add("Error en conexión a base de datos");
                errorMessages.Add(ex.ToString());
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.ToString());
            }
        }

        private void ojo_cerrado_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tbPassword.Visibility == Visibility.Visible)
            {
                ojo_cerrado.Source = new BitmapImage(new Uri("Resources/ojo-visible.png", UriKind.Relative));
                tbVisiblePassword.Text = tbPassword.Password;
                tbPassword.Visibility = Visibility.Hidden;
                tbVisiblePassword.Visibility = Visibility.Visible;

                // Mover el cursor al final del TextBox
                tbVisiblePassword.Focus();
                tbVisiblePassword.CaretIndex = tbVisiblePassword.Text.Length;
            }
            else
            {
                ojo_cerrado.Source = new BitmapImage(new Uri("Resources/ojo-ocultar.png", UriKind.Relative));
                tbPassword.Password = tbVisiblePassword.Text;
                tbPassword.Visibility = Visibility.Visible;
                tbVisiblePassword.Visibility = Visibility.Hidden;
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var Registro = new Registro();
            Registro.Show();
            Registro.Top = this.Top;
            Registro.Left = this.Left;
            this.Close();
        }

        private void enterClick(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btEntrar_Click(sender, e);
            }
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            enterClick(sender, e);
        }

        private void tbUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            enterClick(sender, e);
        }

        private void tbVisiblePassword_KeyDown(object sender, KeyEventArgs e)
        {
            enterClick(sender, e);
        }
    }
}
