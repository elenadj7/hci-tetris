using hci_tetris.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hci_tetris.Processing
{
    public class PlayPhase
    {
        private Figure currentFigure;

        public Figure CurrentFigure
        {
            get => currentFigure;
            private set
            {
                currentFigure = value;
                currentFigure.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentFigure.Move(1, 0);

                    if (!FigureFits())
                    {
                        currentFigure.Move(-1, 0);
                    }
                }
            }
        }

        public Playfield Playfield { get; }
        public FigureList FigureList { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public Figure HeldFigure { get; private set; }
        public bool CanHold { get; private set; }

        public PlayPhase()
        {
            Playfield = new Playfield(22, 10);
            FigureList = new FigureList();
            CurrentFigure = FigureList.GetAndUpdate();
            CanHold = true;
        }

        private bool FigureFits()
        {
            foreach (Position p in CurrentFigure.PiecePositions())
            {
                if (!Playfield.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }

            return true;
        }

        public void HoldFigure()
        {
            if (!CanHold)
            {
                return;
            }

            if (HeldFigure == null)
            {
                HeldFigure = CurrentFigure;
                CurrentFigure = FigureList.GetAndUpdate();
            }
            else
            {
                (HeldFigure, CurrentFigure) = (CurrentFigure, HeldFigure);
            }

            CanHold = false;
        }

        public void RotateFigureClockwise()
        {
            CurrentFigure.RotateClockwise();

            if (!FigureFits())
            {
                CurrentFigure.RotateCounterclockwise();
            }
        }

        public void RotateFigureCounterclockwise()
        {
            CurrentFigure.RotateCounterclockwise();

            if (!FigureFits())
            {
                CurrentFigure.RotateClockwise();
            }
        }

        public void MoveFigureLeft()
        {
            CurrentFigure.Move(0, -1);

            if (!FigureFits())
            {
                CurrentFigure.Move(0, 1);
            }
        }

        public void MoveFigureRight()
        {
            CurrentFigure.Move(0, 1);

            if (!FigureFits())
            {
                CurrentFigure.Move(0, -1);
            }
        }

        private bool IsGameOver()
        {
            return !(Playfield.IsRowEmpty(0) && Playfield.IsRowEmpty(1));
        }

        private void PlaceFigure()
        {
            foreach (Position p in CurrentFigure.PiecePositions())
            {
                Playfield[p.Row, p.Column] = CurrentFigure.Id;
            }

            Score += Playfield.ClearFullRows();

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentFigure = FigureList.GetAndUpdate();
                CanHold = true;
            }
        }

        public void MoveFigureDown()
        {
            CurrentFigure.Move(1, 0);

            if (!FigureFits())
            {
                CurrentFigure.Move(-1, 0);
                PlaceFigure();
            }
        }

        private int PieceDropDistance(Position p)
        {
            int drop = 0;

            while (Playfield.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int FigureDropDistance()
        {
            int drop = Playfield.Rows;

            foreach (Position p in CurrentFigure.PiecePositions())
            {
                drop = Math.Min(drop, PieceDropDistance(p));
            }

            return drop;
        }

        public void DropFigure()
        {
            CurrentFigure.Move(FigureDropDistance(), 0);
            PlaceFigure();
        }
    }
}
