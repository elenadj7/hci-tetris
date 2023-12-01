﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hci_tetris.Models
{
    public class OFigure : Figure
    {
        public override int Id => 4;

        protected override Position[][] Pieces => new Position[][]
        {
            [new(0,0), new(0,1), new(1,0), new(1,1)]
        };

        protected override Position StartOffset => new(0, 4);
    }
}
