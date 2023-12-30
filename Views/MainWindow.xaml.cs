using hci_tetris.Processing;
using hci_tetris.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hci_tetris.Repositories;

namespace hci_tetris.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRepository repository;
        private int score;
        private List<User> Users { get; set; }

        private readonly ImageSource[] pieceImages =
        [
            new BitmapImage(new Uri("../Assets/Seethrough-piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Cyan-piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Blue-piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Orange-piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Yellow-piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Green-piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Purple-piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Red-piece.png", UriKind.Relative))
        ];

        private readonly ImageSource[] figureImages =
        [
            new BitmapImage(new Uri("../Assets/Empty-Figure.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/I-Figure.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/J-Figure.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/L-Figure.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/O-Figure.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/S-Figure.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/T-Figure.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Z-Figure.png", UriKind.Relative))
        ];

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private PlayPhase playPhase = new();

        public MainWindow()
        {
            repository = new JsonRepository();
            if (repository.CheckIfExists(Thread.CurrentPrincipal.Identity.Name))
            {
                score = repository.Get(Thread.CurrentPrincipal.Identity.Name).Score;
            }
            else
            {
                repository.Add(Thread.CurrentPrincipal.Identity.Name, 0);
                score = 0;
            }
            
            InitializeComponent();
            imageControls = SetupGameCanvas(playPhase.Playfield);
        }

        private Image[,] SetupGameCanvas(Playfield playfield)
        {
            Image[,] imageControls = new Image[playfield.Rows, playfield.Columns];
            int cellSize = 25;

            for (int r = 0; r < playfield.Rows; r++)
            {
                for (int c = 0; c < playfield.Columns; c++)
                {
                    Image imageControl = new()
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawPlayfield(Playfield playfield)
        {
            for (int r = 0; r < playfield.Rows; r++)
            {
                for (int c = 0; c < playfield.Columns; c++)
                {
                    int id = playfield[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = pieceImages[id];
                }
            }
        }

        private void DrawFigure(Figure figure)
        {
            foreach (Position p in figure.PiecePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = pieceImages[figure.Id];
            }
        }

        private void DrawNextFigure(FigureList figureList)
        {
            Figure next = figureList.NextFigure;
            NextImage.Source = figureImages[next.Id];
        }

        private void DrawHeldFigure(Figure heldFigure)
        {
            if (heldFigure == null)
            {
                HoldImage.Source = figureImages[0];
            }
            else
            {
                HoldImage.Source = figureImages[heldFigure.Id];
            }
        }

        private void DrawGhostFigure(Figure figure)
        {
            int dropDistance = playPhase.FigureDropDistance();

            foreach (Position p in figure.PiecePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = pieceImages[figure.Id];
            }
        }

        private void Draw(PlayPhase playPhase)
        {
            DrawPlayfield(playPhase.Playfield);
            DrawGhostFigure(playPhase.CurrentFigure);
            DrawFigure(playPhase.CurrentFigure);
            DrawNextFigure(playPhase.FigureList);
            DrawHeldFigure(playPhase.HeldFigure);
            ScoreText.Text = $"Score: {playPhase.Score}";
        }

        private async Task GameLoop()
        {
            Draw(playPhase);

            while (!playPhase.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (playPhase.Score * delayDecrease));
                await Task.Delay(delay);
                playPhase.MoveFigureDown();
                Draw(playPhase);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {playPhase.Score}";
            if(playPhase.Score > score)
            {
                repository.Update(Thread.CurrentPrincipal.Identity.Name, playPhase.Score);
                score = playPhase.Score;
            }

            Users = repository.GetAll();
            Users.Sort((u1, u2) => u2.Score - u1.Score);
            ResultsDataGrid.ItemsSource = Users;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (playPhase.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    playPhase.MoveFigureLeft();
                    break;
                case Key.Right:
                    playPhase.MoveFigureRight();
                    break;
                case Key.Down:
                    playPhase.MoveFigureDown();
                    break;
                case Key.Up:
                    playPhase.RotateFigureClockwise();
                    break;
                case Key.C:
                    playPhase.RotateFigureCounterclockwise();
                    break;
                case Key.H:
                    playPhase.HoldFigure();
                    break;
                case Key.Space:
                    playPhase.DropFigure();
                    break;
                default:
                    return;
            }

            Draw(playPhase);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            playPhase = new PlayPhase();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}