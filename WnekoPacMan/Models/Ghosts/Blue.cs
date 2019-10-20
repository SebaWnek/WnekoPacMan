using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WnekoPacMan.Models.Ghosts
{
    class Blue : AI
    {
        static Brush playerColor = Brushes.Aqua;
        int[] redCell = new int[2];
        int[] playerForwardCell = new int[2];
        int[] vector = new int[2];
        public Blue(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir) : base(gridSize, cellSize, game, cell, dir, playerColor)
        {
            scatterModeTargetCell = gridSize;
            name = "inky";
        }

        public void OnRedPositionChanged(object o, PositionChangedEventArgs e)
        {
            redCell = e.GridCell;
        }

        protected override int[] ChooseCell()
        {
            playerForwardCell[0] = playerCell[0] + 2 * gridDirections[playerDirection][0];
            playerForwardCell[1] = playerCell[1] + 2 * gridDirections[playerDirection][1];
            vector[0] = playerForwardCell[0] - redCell[0];
            vector[1] = playerForwardCell[1] - redCell[1];
            redCell[0] += 2 * vector[0];
            redCell[1] += 2 * vector[1];
            return redCell;
        }
    }
}
