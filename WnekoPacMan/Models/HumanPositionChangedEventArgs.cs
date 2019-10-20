using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WnekoPacMan.Models;

namespace WnekoPacMan.Models
{
    public class HumanPositionChangedEventArgs : EventArgs
    {
        public Directions Direction { get; set; }
        public int[] GridCell { get; set; }

        public HumanPositionChangedEventArgs(Directions dir, int[] cell)
        {
            Direction = dir;
            GridCell = cell;
        }
    }
}
