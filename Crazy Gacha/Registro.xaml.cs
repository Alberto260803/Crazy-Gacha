using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Crazy_Gacha
{
    /// <summary>
    /// Lógica de interacción para Registro.xaml
    /// </summary>
    public partial class Registro : Window
    {
        public string nombre_usuario = "";
        public string pass = "";
        private List<string> errorMessages = new List<string>();
        private Recursos recursos = new Recursos();
        public Registro()
        {
            InitializeComponent();
        }

        private void btEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(recursos.ConectarBD());
                conn.Open();
                string correo = tbCorreo.Text;
                nombre_usuario = tbNombre.Text;
                
                if(tbPassword.Visibility == Visibility.Visible)
                {
                    pass = tbPassword.Password;
                }

                else
                {
                    pass = tbVisiblePassword.Text;
                }

                if (ValidateEmail(correo, conn) == true && ValidateUsername(nombre_usuario, conn) && ValidatePassword(pass) == true)
                {
                    string query = "INSERT INTO usuario (nombre,correo,password,dinero) VALUES (@nombre,@correo,@pass,0)";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.Parameters.AddWithValue("@nombre", nombre_usuario);
                    command.Parameters.AddWithValue("@correo",correo);
                    command.Parameters.AddWithValue("@pass",pass);

                    int result = command.ExecuteNonQuery();

                    if (result == 1)
                    {
                        MessageBox.Show("Datos guardados correctamente");
                        MainWindow mainWindow = new MainWindow(nombre_usuario);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        errorMessages.Add("Error al guardar los datos");
                    }
                    conn.Close();
                }

                if (errorMessages.Count > 0)
                {
                    string errorMessage = string.Join("\n", errorMessages);
                    MessageBox.Show(errorMessage);
                    errorMessages.Clear();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en conexión a base de datos");
                ex.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error.");
                ex.ToString();
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
            var Inicio = new Inicio();
            Inicio.Show();
            Inicio.Top = this.Top;
            Inicio.Left = this.Left;
            this.Close();
        }

        private bool ValidateEmail(string correo, MySqlConnection connection)
        {
            bool validado = true;

            // Valida el formato del email
            if (!Regex.IsMatch(correo, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                errorMessages.Add("Formato de correo electrónico inválido.");
                validado = false;
            }

            // Valida la longitud del email
            if (correo.Length < 5 || correo.Length > 100)
            {
                errorMessages.Add("El correo electrónico debe tener entre 5 y 100 caracteres.");
                validado = false;
            }

            string emailCheckQuery = "SELECT * FROM usuario WHERE correo = @correo";
            MySqlCommand emailCheckCommand = new MySqlCommand(emailCheckQuery, connection);
            emailCheckCommand.Parameters.AddWithValue("@correo", correo);

            try
            {
                MySqlDataReader reader = emailCheckCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    errorMessages.Add("El correo electrónico ya existe en la base de datos.");
                    validado = false;
                }


                reader.Close();
            }
            catch (MySqlException)
            {
                errorMessages.Add("Error en la conexión a la base de datos.");
                validado = false;
            }
            return validado;
        }

        private bool ValidatePassword(string password)
        {
            bool validado = true;
            // Valida la longitud de la contraseña
            if (password.Length < 8)
            {
                errorMessages.Add("La contraseña debe tener al menos 8 caracteres");
                validado = false;
            }

            // Valida la complejidad de la contraseña
            if (!Regex.IsMatch(password, @"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).*$"))
            {
                errorMessages.Add("La contraseña debe contener al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.");
                validado = false;
            }

            return validado;
        }

        private bool ValidateUsername(string nombre, MySqlConnection connection)
        {
            bool validado = true;
            string emailCheckQuery = "SELECT * FROM usuario WHERE nombre = @nombre";
            MySqlCommand emailCheckCommand = new MySqlCommand(emailCheckQuery, connection);
            emailCheckCommand.Parameters.AddWithValue("@nombre", nombre);

            try
            {
                MySqlDataReader reader = emailCheckCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    errorMessages.Add("El nombre de usuario ya existe. Prueba con " + nombre + "23456 o con otra combinación.");
                    validado = false;
                }

                reader.Close();
            }
            catch (MySqlException)
            {
                errorMessages.Add("Error en la conexión a la base de datos.");
                validado = false;
            }

            return validado;
        }

        private void enterClick(object sender,KeyEventArgs e)
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

        private void tbCorreo_KeyDown(object sender, KeyEventArgs e)
        {
            enterClick(sender, e);
        }

        private void tbNombre_KeyDown(object sender, KeyEventArgs e)
        {
            enterClick(sender,e);
        }

        private void tbVisiblePassword_KeyDown(object sender, KeyEventArgs e)
        {
            enterClick(sender,e);
        }
    }
}
