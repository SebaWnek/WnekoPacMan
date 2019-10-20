using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WnekoPacMan.Models
{
    public enum AIModes
    {
        Normal,
        Scatter,
        Random,
    }
    class AI : Player
    {
        bool[] possibleDirrections = new bool[4]; //up, down, left, right
        int[] targedCell = new int[] { 0, 0 };
        int[] scatterModeTargetCell = new int[] { 0, 0 };
        AIModes mode = AIModes.Normal;

        Dictionary<Directions, int> directionToNumber = new Dictionary<Directions, int>
        {
            {Directions.Up, 0 },
            {Directions.Down, 1 },
            {Directions.Left, 2 },
            {Directions.Right, 3 },
        };
        Dictionary<int, Directions> numberToDirection = new Dictionary<int, Directions>
        {
            {0, Directions.Up },
            {1, Directions.Down },
            {2, Directions.Left },
            {3, Directions.Right }
        };
        Dictionary<Directions, Directions> oppositeDirections = new Dictionary<Directions, Directions>
        {
            {Directions.Up, Directions.Down },
            {Directions.Down, Directions.Up },
            {Directions.Left, Directions.Right },
            {Directions.Right, Directions.Left }
        };

        public AIModes Mode
        {
            get => mode;
            set
            {
                mode = value;
                TurnArround();
            }
        }

        private void TurnArround()
        {
            currentDirection = oppositeDirections[currentDirection];
        }

        public AI(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir) : base(gridSize, cellSize, game, cell, dir)
        {
            currentDirection = dir;
            movementDirection = movementDirections[dir];
        }

        public override void Move()
        {
            base.Move();
            GridCell[1] = (playerPosition[0]) / cellSize; //collumn
            GridCell[0] = (playerPosition[1]) / cellSize; //row
            int nextDirNumb;
            int count;
            if (CheckIfInTheMiddle())
            {
                possibleDirrections = game.CheckIntersection(gridCell);
                possibleDirrections[directionToNumber[oppositeDirections[currentDirection]]] = false;
                count = possibleDirrections.Count(t => t);
                if (count == 1)
                {
                    nextDirNumb = Array.IndexOf(possibleDirrections, true);
                    currentDirection = numberToDirection[nextDirNumb];
                }
                if (count > 1)
                {
                    currentDirection = ChooseNextDirection(possibleDirrections);
                }
                movementDirection = movementDirections[currentDirection];
                //Debug.WriteLine(targedCell[0] + ", " + targedCell[1]);
            }

        }

        private Directions ChooseNextDirection(bool[] possibleDirrections)
        {
            int[] tmpGridCell = new int[2];
            double tmpDistance;
            int selectedDirection = 0;
            double smallestDistance = int.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                if (possibleDirrections[i] == true)
                {
                    tmpGridCell[0] = gridCell[0] + gridDirections[numberToDirection[i]][0];
                    tmpGridCell[1] = gridCell[1] + gridDirections[numberToDirection[i]][1];
                    tmpDistance = CalculateDistance(tmpGridCell);
                    if (tmpDistance < smallestDistance)
                    {
                        smallestDistance = tmpDistance;
                        selectedDirection = i;
                    }
                }
            }
            return numberToDirection[selectedDirection];
        }

        private double CalculateDistance(int[] tmpGridCell)
        {
            int[] tmpTargedCell;
            switch (Mode)
            {
                case AIModes.Scatter:
                    tmpTargedCell = scatterModeTargetCell;
                    break;
                case AIModes.Random:
                    tmpTargedCell = new int[] { rnd.Next(0, gridSize[0]), rnd.Next(0, gridSize[1]) };
                    break;
                default:
                    tmpTargedCell = targedCell;
                    break;

            }
            double x = Math.Abs((double)tmpGridCell[0] - (double)tmpTargedCell[0]);
            double y = Math.Abs((double)tmpGridCell[1] - (double)tmpTargedCell[1]);
            return Math.Sqrt(x * x + y * y);
        }

        public void OnHumanPositionChanged(object sender, HumanPositionChangedEventArgs e)
        {
            targedCell = e.GridCell;
        }
    }
}
