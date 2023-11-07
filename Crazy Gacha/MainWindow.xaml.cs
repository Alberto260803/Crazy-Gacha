using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Crazy_Gacha
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int numegg = 5;
        private MediaPlayer? player;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer? timer;
        private string nombre = "";
        private static Recursos recursos = new Recursos();
        private MySqlConnection conn = new MySqlConnection(recursos.ConectarBD());
        private List<Tienda> itemsClicks = null;

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            if (sender is MediaPlayer mediaPlayer)
            {
                mediaPlayer.Position = TimeSpan.Zero;
                mediaPlayer.Play();
            }
        }


        public MainWindow(string nombre)
        {
            InitializeComponent();
            NumEgg.Content = numegg;
            tbNombre.Text = nombre;
            Loaded += Window_Loaded;
            mediaPlayer.MediaEnded += new EventHandler(MediaPlayer_MediaEnded);
            mediaPlayer.Open(new Uri("Resources/Pou.mp3", UriKind.Relative));
            mediaPlayer.Play();
        }

        private void Egg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            numegg -= 1;
            NumEgg.Content = numegg;
            if (numegg < 0)
                NumEgg.Content = 0;

            if (numegg == 0)
            {
                mediaPlayer.Pause();             
                var storyboard = new Storyboard();
                var rotateAnimation = new DoubleAnimation(0, 15, TimeSpan.FromSeconds(0.1));
                rotateAnimation.AutoReverse = true;
                rotateAnimation.RepeatBehavior = new RepeatBehavior(3);
                Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(Image.RenderTransform).(RotateTransform.Angle)"));
                storyboard.Children.Add(rotateAnimation);
                Egg.BeginStoryboard(storyboard);

                try
                {
                    conn.Open();

                    string query = "SELECT audio FROM premios WHERE id=1";
                    MySqlCommand command = new MySqlCommand(query, conn);

                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        // Reproducir el audio
                        byte[] audioData = (byte[])reader["audio"];
                        ReproducirSonido(audioData);
                    }
                    else
                    {
                        MessageBox.Show("No.");
                    }
                    reader.Close();
                    conn.Close();

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error en conexión a base de datos", ex.ToString());
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                // Configurar el timer para abrir la ventana después de la animación
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(0.6); // Ajusta este valor según la duración de tu animación
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    AbrirVentanaPremio();
                };
                timer.Start();
            }
        }

        private void AbrirVentanaPremio()
        {
            string prob;

            Random random = new Random();
            int probabilidad = random.Next(1, 101); // Genera un número aleatorio entre 1 y 100

            if (probabilidad <= 60) // Común
            {
                prob = "Común";
            }
            else if (probabilidad <= 85) // Rara
            {
                prob = "Rara";
            }
            else if (probabilidad <= 95) // Especial
            {
                prob = "Especial";
            }
            else if (probabilidad <= 99) // Épica
            {
                prob = "Épica";
            }
            else // Legendaria
            {
                prob = "Legendaria";
            }

            try
            {
                conn.Open();

                string query = "SELECT * FROM premios WHERE rareza=@rareza ORDER BY RAND() LIMIT 1;";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@rareza", prob);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    var Premio = new Premios();
                    string rareza = reader.GetString("rareza");
                    string nombre = reader.GetString("nombre");
                    Premio.Rareza = rareza;
                    Premio.Nombre = nombre;
                    byte[] bytes = (byte[])reader["imagen"];
                    BitmapImage image = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(bytes))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                    }
                    Premio.ImagenPremio = image;
                    byte[] audioData = (byte[])reader["audio"];
                    ReproducirSonido(audioData);

                    Premio.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    Premio.Closed += (sender, args) =>
                    {
                        // Detener la reproducción del sonido cuando se cierre la ventana
                        player?.Stop();
                        mediaPlayer.Play();
                    };

                    Premio.Show();
                }
                else
                {
                    MessageBox.Show("No.");
                }
                reader.Close();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en conexión a base de datos", ex.ToString());
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            numegg = 5;
            NumEgg.Content = numegg;
        }


        private void ReproducirSonido(byte[] audioData)
        {
            try
            {
                string tempFilePath = System.IO.Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, audioData);

                // Cambiar la extensión del archivo temporal a .mp3
                string mp3FilePath = System.IO.Path.ChangeExtension(tempFilePath, ".mp3");
                File.Move(tempFilePath, mp3FilePath);

                // Crear un reproductor MediaPlayer y reproducir el archivo temporal
                player = new MediaPlayer();
                player.Open(new Uri(mp3FilePath));
                player.Play();

                // Esperar hasta que se complete la reproducción si es necesario
                player.MediaEnded += (sender, e) =>
                {
                    player.Close();
                    File.Delete(mp3FilePath); // Eliminar el archivo temporal
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reproducir audio: {ex.Message}", "Error");
            }
        }


        private void CargarTienda()
        {
            listBoxMouse.ItemsSource = null;
            listBoxMouse.Items.Clear();

            try
            {
                conn.Open();
                string query = "SELECT * FROM tienda WHERE tipoMejora='Clicks'";

                if (!string.IsNullOrEmpty(query))
                {
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    itemsClicks = new List<Tienda>();

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            Tienda tiendaItem = new Tienda
                            {
                                NombreMejora = reader.GetString("nombreMejora"),
                                Precio = reader.GetFloat("precio"),
                                Cantidad = reader.GetInt32("cantidad")
                            };

                            // Cargar la imagen desde la base de datos y asignarla a ImagenMejora
                            byte[] bytes = (byte[])reader["imagenMejora"];
                            using (MemoryStream stream = new MemoryStream(bytes))
                            {
                                BitmapImage image = new BitmapImage();
                                image.BeginInit();
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.StreamSource = stream;
                                image.EndInit();
                                tiendaItem.ImagenMejora = image;
                            }

                            listBoxMouse.Items.Add(tiendaItem);
                        }
                    }
                }

                listBoxMouse.ItemsSource = itemsClicks;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en conexión a base de datos", ex.ToString());
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();

                string query = "SELECT imagen FROM premios WHERE id=1";
                MySqlCommand command = new MySqlCommand(query, conn);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    BitmapImage image = new BitmapImage();
                    byte[] bytes = (byte[])reader["imagen"];
                    using (MemoryStream stream = new MemoryStream(bytes))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                    }
                    Egg.Source = image;
                }
                else
                {
                    MessageBox.Show("No.");
                }
                reader.Close();
                conn.Close();
                CargarTienda();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en conexión a base de datos", ex.ToString());
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}
