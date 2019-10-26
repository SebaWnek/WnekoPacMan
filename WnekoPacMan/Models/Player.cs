using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WnekoPacMan.Models
{
    public enum SpeedModes
    {
        Human,
        Normal,
        Tunnel,
        Fright,
        FrightHuman,
        Elroy1,
        Elroy2
    }

    public enum Directions { Up, Down, Left, Right, Stop };
    public class Player : INotifyPropertyChanged
    {
        protected Dictionary<SpeedModes, float> Speeds = new Dictionary<SpeedModes, float>
        {
            { SpeedModes.Human, 0.8f},
            { SpeedModes.Normal, 0.75f},
            { SpeedModes.Tunnel, 0.4f},
            { SpeedModes.Fright, 0.5f},
            { SpeedModes.FrightHuman, 0.9f},
            { SpeedModes.Elroy1, 0.8f},
            { SpeedModes.Elroy2, 0.85f},
        };

        public static readonly Random rnd = new Random();
        protected static readonly Dictionary<Directions, int[]> movementDirections = new Dictionary<Directions, int[]>
        {
            {Directions.Up, new int[] { 0, -1 } },
            {Directions.Down, new int[] { 0, 1 }  },
            {Directions.Left, new int[] { -1, 0 } },
            {Directions.Right, new int[] { 1, 0 } },
            {Directions.Stop, new int[] {0,0} }
        };
        protected static readonly Dictionary<Directions, int[]> gridDirections = new Dictionary<Directions, int[]>
        {
            {Directions.Up, new int[] { -1, 0 } },
            {Directions.Down, new int[] { 1, 0 }  },
            {Directions.Left, new int[] { 0, -1 } },
            {Directions.Right, new int[] { 0, 1 } },
            {Directions.Stop, new int[] {0,0 } }
        };

        protected static readonly Dictionary<Directions, Directions> oppositeDirections = new Dictionary<Directions, Directions>
        {
            {Directions.Up, Directions.Down },
            {Directions.Down, Directions.Up },
            {Directions.Left, Directions.Right },
            {Directions.Right, Directions.Left },
            {Directions.Stop, Directions.Stop }
        };

        protected float[] playerPosition; // position of middle
        protected int[] movementDirection = new int[] { 0, 0 };
        protected int[] firstGridCell = new int[2];
        protected Ellipse playerEllipse;
        protected float baseSpeed = 2.5f;
        protected float speedModifier = 0.75f;
        protected SpeedModes speedMode = SpeedModes.Normal;

        protected int[] gridCell = new int[2];
        protected int[] previousCell = new int[2];
        protected Directions currentDirection;
        protected int[] gridSize;
        protected int cellSize;
        protected Game game;

        protected Player(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir, Brush color, float speed)
        {
            speedModifier = speed;
            currentDirection = dir;
            Array.Copy(movementDirections[currentDirection], movementDirection, 2);
            firstGridCell = cell;
            playerEllipse = new Ellipse();
            playerEllipse.Fill = color;
            playerEllipse.Height = cellSize;
            playerEllipse.Width = cellSize;
            playerPosition = new float[] { firstGridCell[1] * cellSize + cellSize / 2, firstGridCell[0] * cellSize + cellSize / 2 };
            Binding topBinding = new Binding("PlayerTop");
            Binding leftBinding = new Binding("PlayerLeft");
            topBinding.Source = this;
            leftBinding.Source = this;
            playerEllipse.SetBinding(Canvas.LeftProperty, leftBinding);
            playerEllipse.SetBinding(Canvas.TopProperty, topBinding);
            this.gridSize = gridSize;
            this.cellSize = cellSize;
            this.game = game;
        }


        protected bool CheckIfInTheMiddle()
        {
            return (Math.Abs(playerPosition[0] - (gridCell[1] * cellSize + cellSize / 2)) <= baseSpeed / 2) && (Math.Abs(playerPosition[1] - (gridCell[0] * cellSize + cellSize / 2)) <= baseSpeed / 2);
        }

        protected void CalculateCell()
        {
            GridCell[1] = (int)(playerPosition[0]) / cellSize; //column
            GridCell[0] = (int)(playerPosition[1]) / cellSize; //row
        }

        public virtual void ChangeSpeed(SpeedModes mode)
        {
            speedMode = mode;
            speedModifier = Speeds[mode];
        }

        public float PlayerLeft
        {
            get => playerPosition[0] - cellSize / 2;
            set
            {
                playerPosition[0] = value + cellSize / 2;
                if (game.SkipCounter == 0)
                {
                    NotifyPropertyChanged();
                }
            }
        }
        public float PlayerTop
        {
            get => playerPosition[1] - cellSize / 2;
            set
            {
                playerPosition[1] = value + cellSize / 2;
                if (game.SkipCounter == 0)
                {
                    NotifyPropertyChanged();
                }
            }
        }

        public int[] GridCell { get => gridCell; set => gridCell = value; }
        protected float Speed { get => baseSpeed * speedModifier; set => speedModifier = value; }

        public virtual void Move()
        {
            CalculateCell();
            PlayerLeft += Speed * movementDirection[0];
            PlayerTop += Speed * movementDirection[1];
            if (playerPosition[0] <= 0 || playerPosition[1] <= 0 || playerPosition[0] >= gridSize[1] * cellSize || playerPosition[1] >= gridSize[0] * cellSize)
            {
                JumpOverBorder();
            }
        }

        protected int[] GetNextCell(Directions dirrection)
        {
            int[] result = new int[2];
            result[0] = gridCell[0] + gridDirections[dirrection][0];
            result[1] = gridCell[1] + gridDirections[dirrection][1];
            return result;
        }

        protected int[,] GetNextCells()
        {
            throw new NotImplementedException();
        }

        protected void JumpOverBorder()
        {
            if (playerPosition[0] <= 0) playerPosition[0] = gridSize[1] * cellSize;
            else if (playerPosition[1] <= 0) playerPosition[1] = gridSize[0] * cellSize;
            else if (playerPosition[0] >= gridSize[1] * cellSize) playerPosition[0] = 0;
            else if (playerPosition[1] >= gridSize[0] * cellSize) playerPosition[1] = 0;
        }

        public UIElement GetPlayerGraphics()
        {
            return playerEllipse;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
