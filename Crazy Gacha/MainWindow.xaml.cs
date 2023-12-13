using Crazy_Gacha.Clases;
using MySql.Data.MySqlClient;
using NAudio.Wave;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
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
        private List<Tienda> itemsClicks = new List<Tienda>();
        private int clicksUser = 1;
        private int idUsuario = 0;

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            if (sender is MediaPlayer mediaPlayer)
            {
                mediaPlayer.Position = TimeSpan.Zero;
                mediaPlayer.Play();
            }
        }


        public MainWindow(string nombre, int idUsu)
        {
            InitializeComponent();
            NumEgg.Content = numegg;
            tbNombre.Text = nombre;
            idUsuario = idUsu;
            Loaded += Window_Loaded;
            mediaPlayer.MediaEnded += new EventHandler(MediaPlayer_MediaEnded);
            mediaPlayer.Open(new Uri("Resources/Pou.mp3", UriKind.Relative));
            mediaPlayer.Play();
            CargarTienda();
        }

        private void Egg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            numegg -= clicksUser;
            NumEgg.Content = numegg;
            if (numegg < 0)
            {
                NumEgg.Content = 0;
                numegg = 0;
            }

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

                    string query = "SELECT linkAudio FROM premios WHERE id=1";
                    MySqlCommand command = new MySqlCommand(query, conn);

                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        // Reproducir el audio
                        string linkAudio = reader.GetString("linkAudio");
                        LlamarReproducirSonido(linkAudio);
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
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
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

                //string query = "SELECT * FROM premios WHERE rareza=@rareza ORDER BY RAND() LIMIT 1;";
                string query = "SELECT * FROM premios WHERE rareza=@rareza;";
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

                    string blobUrl = reader.GetString("linkImagen");

                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imageData = webClient.DownloadData(blobUrl);

                        // Convertir los datos descargados a BitmapImage
                        BitmapImage image = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(imageData))
                        {
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = stream;
                            image.EndInit();
                        }

                        // Asignar la imagen a tu objeto Premio
                        Premio.ImagenPremio = image;
                    }

                    /*byte[] audioData = (byte[])reader["audio"];
                    ReproducirSonido(audioData);*/

                    string audioUrl = reader.GetString("linkAudio");
                    LlamarReproducirSonido(audioUrl);

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

        private async void LlamarReproducirSonido(string audioUrl)
        {        
            await ReproducirSonidoDesdeURL(audioUrl);
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

        async Task ReproducirSonidoDesdeURL(string audioUrl)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    byte[] audioBytes = await httpClient.GetByteArrayAsync(audioUrl);

                    using (MemoryStream audioStream = new MemoryStream(audioBytes))
                    {
                        using (var waveOut = new WaveOutEvent()) // WaveOutEvent para reproducir el audio
                        {
                            using (var waveStream = new Mp3FileReader(audioStream))
                            {
                                waveOut.Init(waveStream);
                                waveOut.Play();
                                while (waveOut.PlaybackState == PlaybackState.Playing)
                                {
                                    await Task.Delay(100); // Espera hasta que termine de reproducirse
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al reproducir audio desde URL: {ex.Message}", "Error");
                }
            }
        }


        private void CargarTienda()
        {
            listBoxMouse.ItemsSource = null;
            itemsClicks.Clear();

            try
            {
                conn.Open();
                string query = "SELECT * FROM tienda WHERE tipoMejora='Clicks' AND ((idUsuario = @idUsuario) OR (idUsuario IS NULL AND NOT EXISTS (SELECT 1 FROM tienda WHERE idUsuario = @idUsuario)))";

                if (!string.IsNullOrEmpty(query))
                {
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);
                    MySqlDataReader reader = command.ExecuteReader();

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
                            string blobUrl = reader.GetString("linkImagenMejora");

                            using (WebClient webClient = new WebClient())
                            {
                                byte[] imageData = webClient.DownloadData(blobUrl);

                                // Convertir los datos descargados a BitmapImage
                                BitmapImage image = new BitmapImage();
                                using (MemoryStream stream = new MemoryStream(imageData))
                                {
                                    image.BeginInit();
                                    image.CacheOption = BitmapCacheOption.OnLoad;
                                    image.StreamSource = stream;
                                    image.EndInit();
                                    tiendaItem.ImagenMejora = image;
                                }
                            }

                            itemsClicks.Add(tiendaItem);
                        }
                    }

                    reader.Close();
                    conn.Close();
                }
                listBoxMouse.ItemsSource = itemsClicks;

                conn.Open();
                foreach (Tienda tiendaItem in listBoxMouse.Items)
                {
                    string queryDinero = "SELECT dinero FROM usuario WHERE id = @idUsuario";
                    MySqlCommand commandDinero = new MySqlCommand(queryDinero, conn);
                    commandDinero.Parameters.AddWithValue("@idUsuario", idUsuario);
                    int dineroDisponible = (int)commandDinero.ExecuteScalar();

                    // Verificar si el usuario tiene suficiente dinero para comprar el item tienda
                    if (tiendaItem.Precio > dineroDisponible)
                    {
                        // Cambiar el color del item a gris
                        tiendaItem.Background = Brushes.Gray;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en conexión a base de datos", ex.ToString());
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();

                string query = "SELECT linkImagen FROM premios WHERE id=1";
                MySqlCommand command = new MySqlCommand(query, conn);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    string blobUrl = reader.GetString("linkImagen");

                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imageData = webClient.DownloadData(blobUrl);

                        // Convertir los datos descargados a BitmapImage
                        BitmapImage image = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(imageData))
                        {
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = stream;
                            image.EndInit();
                            Egg.Source = image;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No.");
                }
                reader.Close();

                query = "SELECT cantidad FROM tienda WHERE nombreMejora='+1 click' AND idUsuario=@idUsu";
                command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@idUsu", idUsuario);
                clicksUser = (int)command.ExecuteScalar();
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
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                conn.Open();

                ListBoxItem listBoxItem = sender as ListBoxItem;

                if (listBoxItem != null)
                {
                    Tienda tiendaSeleccionada = listBoxItem.DataContext as Tienda;

                    if (tiendaSeleccionada != null)
                    {
                        string nombreMejora = tiendaSeleccionada.NombreMejora;
                        string query = "SELECT nombreMejora FROM tienda WHERE nombreMejora = @NombreMejora AND (idUsuario = @idUsuario OR idUsuario IS NULL)";

                        MySqlCommand command = new MySqlCommand(query, conn);
                        command.Parameters.AddWithValue("@NombreMejora", nombreMejora);
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);

                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string nomMejora = reader.GetString("nombreMejora");
                            reader.Close();

                            // Verificar si el usuario tiene suficiente dinero para comprar el item tienda
                            string queryDinero = "SELECT dinero FROM usuario WHERE id = @idUsuario";
                            MySqlCommand commandDinero = new MySqlCommand(queryDinero, conn);
                            commandDinero.Parameters.AddWithValue("@idUsuario", idUsuario);
                            int dineroDisponible = (int)commandDinero.ExecuteScalar();

                            if (tiendaSeleccionada.Precio > dineroDisponible)
                            {
                                // Cancelar la propagación del evento si el usuario no tiene suficiente dinero
                                e.Handled = true;
                            }
                            else
                            {
                                // Realizar la acción específica
                                RealizarMejora(nomMejora, idUsuario);
                                // Actualizar el dinero del usuario restando el precio de la tienda
                                string updateDineroQuery = "UPDATE usuario SET dinero = dinero - @Precio WHERE id = @idUsuario";
                                MySqlCommand updateDineroCommand = new MySqlCommand(updateDineroQuery, conn);
                                updateDineroCommand.Parameters.AddWithValue("@Precio", tiendaSeleccionada.Precio);
                                updateDineroCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                                updateDineroCommand.ExecuteNonQuery();
                                conn.Close();
                                CargarTienda();
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en conexión a base de datos", ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }



        public void RealizarMejora(string nomMejora, int idUsuario)
        {
            switch (nomMejora)
            {
                case "+1 click":
                    clicksUser += 1;
                    string selectQuery = "SELECT COUNT(*) FROM tienda WHERE nombreMejora=@nomMejora AND idUsuario = @idUsuario";
                    MySqlCommand selectCommand = new MySqlCommand(selectQuery, conn);
                    selectCommand.Parameters.AddWithValue("@nomMejora", nomMejora);
                    selectCommand.Parameters.AddWithValue("@idUsuario", idUsuario);

                    int count = Convert.ToInt32(selectCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        // Ya existe una entrada, realizar actualización
                        string updateQuery = "UPDATE tienda SET cantidad = cantidad + 1 WHERE nombreMejora=@nomMejora AND idUsuario = @idUsuario";
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn);
                        updateCommand.Parameters.AddWithValue("@nomMejora", nomMejora);
                        updateCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                        updateCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        // No existe una entrada, realizar inserción
                        string insertQuery = "INSERT INTO tienda (nombreMejora, cantidad, idUsuario, precio, tipoMejora, imagenMejora) " +
                        "SELECT nombreMejora, 1, @idUsuario, precio, tipoMejora, imagenMejora " +
                        "FROM tienda " +
                        "WHERE nombreMejora=@nomMejora LIMIT 1";

                        MySqlCommand insertCommand = new MySqlCommand(insertQuery, conn);
                        insertCommand.Parameters.AddWithValue("@nomMejora", nomMejora);
                        insertCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                        insertCommand.ExecuteNonQuery();
                    }
                    break;
            }
        }
    }
}
