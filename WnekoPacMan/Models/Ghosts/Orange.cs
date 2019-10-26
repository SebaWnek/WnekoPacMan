using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WnekoPacMan.Models.Ghosts
{
    class Orange : AI
    {
        double distance = 0;
        static Brush playerColor = Brushes.Orange;
        public Orange(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir, float speed) : base(gridSize, cellSize, game, cell, dir, playerColor, speed)
        {
            scatterModeTargetCell = new int[] { gridSize[0], 0 };
            name = "clyde";
        }

        protected override int[] ChooseCell()
        {
            distance = CalculateDistance(gridCell, playerCell);
            if (distance < 8) return scatterModeTargetCell;
            else return playerCell;
        }
    }
}
