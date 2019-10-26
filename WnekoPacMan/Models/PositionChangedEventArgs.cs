using System;

namespace WnekoPacMan.Models
{
    public class HumanPositionChangedEventArgs : PositionChangedEventArgs
    {
        public Directions Direction { get; set; }

        public HumanPositionChangedEventArgs(Directions dir, int[] cell) : base(cell)
        {
            Direction = dir;
        }
    }

    public class PositionChangedEventArgs : EventArgs
    {
        public int[] GridCell { get; set; }

        public PositionChangedEventArgs(int[] cell)
        {
            GridCell = cell;
        }
    }
}
