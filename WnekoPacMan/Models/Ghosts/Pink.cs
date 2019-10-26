using System.Windows.Media;

namespace WnekoPacMan.Models.Ghosts
{
    class Pink : AI
    {
        static Brush playerColor = Brushes.Pink;
        public Pink(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir, float speed) : base(gridSize, cellSize, game, cell, dir, playerColor, speed)
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
