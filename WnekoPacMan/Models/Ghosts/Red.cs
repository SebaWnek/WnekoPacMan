using System;
using System.Windows.Media;

namespace WnekoPacMan.Models.Ghosts
{
    class Red : AI
    {
        public event EventHandler<PositionChangedEventArgs> RedPositionChanged;
        static Brush playerColor = Brushes.Red;
        protected bool isElroy1 = false;
        protected bool isElroy2 = false;
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

        protected override void SelectTargetCell()
        {
            base.SelectTargetCell();
            if ((isElroy1 || isElroy2) && Mode == AIModes.Scatter)
            {
                targetCell = ChooseCell();
                NotifyPropertyChanged("TargetCellColumn");
                NotifyPropertyChanged("TargetCellRow");
            }
        }

        public override void ChangeSpeed(SpeedModes mode)
        {
            base.ChangeSpeed(mode);
            if (mode == SpeedModes.Normal)
            {
                if (isElroy1)
                {
                    speedMode = SpeedModes.Elroy1;
                    speedModifier = Speeds[SpeedModes.Elroy1];
                }
                else if (isElroy2)
                {
                    speedMode = SpeedModes.Elroy2;
                    speedModifier = Speeds[SpeedModes.Elroy2];
                }
            }
        }

        public void BecomeElroy(int i)
        {
            if (i == 0)
            {
                isElroy1 = false;
                isElroy2 = false;
            }
            else if (i == 1)
            {
                isElroy1 = true;
                isElroy2 = false;
            }
            else if (i == 2)
            {
                isElroy1 = false;
                isElroy2 = true;
            }
            ChangeSpeed(speedMode);
        }
    }
}
