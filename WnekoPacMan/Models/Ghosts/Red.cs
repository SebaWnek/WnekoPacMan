using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WnekoPacMan.Models.Ghosts
{
    class Red : AI
    {
        public event EventHandler<PositionChangedEventArgs> RedPositionChanged;
        static Brush playerColor = Brushes.Red;
        public Red(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir, float speed) : base(gridSize, cellSize, game, cell, dir, playerColor, speed)
        {
            scatterModeTargetCell = new int[] { 0, gridSize[1] };
            name = "blinky";
        }

        protected virtual void OnPositionChanged()
        {
            PositionChangedEventArgs e = new PositionChangedEventArgs(gridCell);
            RedPositionChanged.Invoke(this, e);
        }

        public override void Move()
        {
            previousCell[0] = gridCell[0];
            previousCell[1] = gridCell[1];
            base.Move();
            if (gridCell[0] != previousCell[0] || gridCell[1] != previousCell[1])
            {
                OnPositionChanged();
            }
        }
    }
}
