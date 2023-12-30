using hci_tetris.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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

namespace hci_tetris.Views
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private readonly IRepository repository;

        public StartWindow()
        {
            repository = new JsonRepository();
            InitializeComponent();
        }

        public void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;

            if (string.IsNullOrWhiteSpace(username) )
            {
                ErrorBlock.Text = "* Username must not be empty";
            }
            else
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);
                MainWindow mainWindow = new();
                mainWindow.Show();
                this.Close();
            }
        }
    }
}
