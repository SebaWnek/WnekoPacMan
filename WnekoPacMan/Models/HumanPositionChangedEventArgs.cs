using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WnekoPacMan.Models;

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
