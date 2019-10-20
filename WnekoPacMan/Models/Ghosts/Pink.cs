using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WnekoPacMan.Models.Ghosts
{
    class Pink : AI
    {
        static Brush playerColor = Brushes.Pink;
        public Pink(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir) : base(gridSize, cellSize, game, cell, dir, playerColor)
        {
            scatterModeTargetCell = new int[] { 0, 0 };
            name = "pinky";
        }

        protected override int[] ChooseCell()
        {
            int[] result = playerCell;
            result[0] += 4 * gridDirections[playerDirection][0];
            result[1] += 4 * gridDirections[playerDirection][1];
            return result;
        }
    }
}
