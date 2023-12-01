using hci_tetris.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hci_tetris.Processing
{
    public class FigureList
    {
        private readonly Figure[] figures =
        [
            new IFigure(),
            new JFigure(),
            new LFigure(),
            new OFigure(),
            new SFigure(),
            new TFigure(),
            new ZFigure()
        ];

        private readonly Random random = new();

        public Figure NextFigure { get; private set; }

        public FigureList()
        {
            NextFigure = RandomFigure();
        }

        private Figure RandomFigure()
        {
            return figures[random.Next(figures.Length)];
        }

        public Figure GetAndUpdate()
        {
            Figure figure = NextFigure;

            do
            {
                NextFigure = RandomFigure();
            }
            while (figure.Id == NextFigure.Id);

            return figure;
        }
    }
}
