using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WnekoPacMan.Models
{
    class Human : Player
    {
        public event EventHandler<HumanPositionChangedEventArgs> HumanPositionChanged;
        int[] newGridCell = new int[2];
        int[] previousGridCell = new int[2];
        static Brush playerColor = Brushes.Yellow;
        int score = 0;
        bool shouldMove = true;
        int stopCounter = 0;
        bool canTurn = false;
        List<Directions> possibleDirections = new List<Directions>();
        bool isTurning = false;
        float[] middle = new float[2];
        float[] relativeToMiddle = new float[2];
        int dir;
        int opDir;

        int[] nextCell;
        CellType nextCellType;


        Directions nextDirection;
        public Human(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir, float speed) : base(gridSize, cellSize, game, cell, dir, playerColor, speed)
        {
            nextDirection = dir;
            speedMode = SpeedModes.Human;
            speedModifier = Speeds[speedMode];
        }

        public Directions NextDirection { get => nextDirection; set => nextDirection = value; }

        public override void Move()
        {
            if (shouldMove)
            {
                base.Move();
                newGridCell[1] = (int)(playerPosition[0]) / cellSize; //column
                newGridCell[0] = (int)(playerPosition[1]) / cellSize; //row
                previousGridCell[0] = gridCell[0];
                previousGridCell[1] = gridCell[1];
                gridCell[0] = newGridCell[0];
                gridCell[1] = newGridCell[1];
                if (gridCell[0] != previousGridCell[0] || gridCell[1] != previousGridCell[1])
                {
                    OnHumanPositionChanged();
                    CheckIfCanTurn();
                    TryEat();
                }
                if (canTurn)
                {
                    Turn();
                }

                if (!isTurning && CheckIfInTheMiddle())
                {
                    nextCell = GetNextCell(currentDirection);
                    nextCellType = game.CheckGridCellType(nextCell); 
                    if (currentDirection == nextDirection && nextCellType == CellType.wall)
                    {
                        currentDirection = Directions.Stop;
                        Array.Copy(movementDirections[currentDirection], movementDirection, 2);
                    }
                    if (currentDirection == Directions.Stop && nextDirection != Directions.Stop)
                    {
                        nextCell = GetNextCell(nextDirection);
                        nextCellType = game.CheckGridCellType(nextCell);
                        if(nextCellType != CellType.wall)
                        {
                            currentDirection = nextDirection;
                            Array.Copy(movementDirections[currentDirection], movementDirection, 2);
                            dir = (currentDirection == Directions.Left || currentDirection == Directions.Right) ? 1 : 0; //direction to be centered
                            CenterPlayer();
                        }
                    }
                }
                if (NextDirection == oppositeDirections[currentDirection])
                {
                    currentDirection = NextDirection;
                    Array.Copy(movementDirections[currentDirection], movementDirection, 2);
                }
            }
            else
            {
                stopCounter--;
                if (stopCounter == 0) shouldMove = true;
            }


        }

        private void Turn()
        {
            CalculateMiddle();
            CalculateRelativeToMiddle();
            //if(currentDirection == NextDirection) DoNothing! Just move forward
            if (currentDirection != NextDirection && !isTurning && possibleDirections.Contains(nextDirection)) //Means just entered that grid cell or key was pressed
            {
                dir = (currentDirection == Directions.Left || currentDirection == Directions.Right) ? 0 : 1;
                opDir = dir == 0 ? 1 : 0;
                bool beforeMiddle = (relativeToMiddle[dir] < 0 && movementDirections[currentDirection][dir] > 0) ||
                                    (relativeToMiddle[dir] > 0 && movementDirections[currentDirection][dir] < 0);
                isTurning = true;
                movementDirection[dir] *= beforeMiddle ? 1 : -1;
                movementDirection[opDir] = movementDirections[nextDirection][opDir];
                CheckIfFinishedTurning();
            }
            else if (isTurning) //means is already turning, so can't change direction again
            {
                CheckIfFinishedTurning();
            }
        }

        private void CheckIfFinishedTurning()
        {
            bool finishedTurn = Math.Abs(relativeToMiddle[dir]) <= baseSpeed / 2;
            if (finishedTurn)
            {
                currentDirection = nextDirection;
                Array.Copy(movementDirections[currentDirection], movementDirection, 2);
                isTurning = false;
                CenterPlayer();
            }
        }

        private void CenterPlayer()
        {
            if (dir == 0) //put in the middle
            {
                PlayerLeft = gridCell[1] * cellSize;
            }
            else
            {
                PlayerTop = gridCell[0] * cellSize;
            }
        }

        private void CalculateRelativeToMiddle()
        {
            relativeToMiddle[0] = playerPosition[0] - middle[0];
            relativeToMiddle[1] = playerPosition[1] - middle[1];
        }

        private void CalculateMiddle()
        {
            middle[0] = gridCell[1] * cellSize + cellSize / 2;
            middle[1] = gridCell[0] * cellSize + cellSize / 2;
        }

        private void TryEat()
        {
            CellType currentCell = game.CheckGridCellType(gridCell);
            switch (currentCell)
            {
                case CellType.dot:
                    score += game.Eat(gridCell[0], gridCell[1]);
                    shouldMove = false;
                    stopCounter = 1;
                    break;
                case CellType.energizer:
                    score += game.Eat(gridCell[0], gridCell[1]);
                    shouldMove = false;
                    stopCounter = 3;
                    break;
                case CellType.fruit:
                    score += game.Eat(gridCell[0], gridCell[1]);
                    break;
            }
        }

        private void CheckIfCanTurn()
        {
            Directions dir1;
            Directions dir2;
            if(currentDirection == Directions.Down || currentDirection == Directions.Up)
            {
                dir1 = Directions.Left;
                dir2 = Directions.Right;
            }
            else
            {
                dir1 = Directions.Down;
                dir2 = Directions.Up;
            }
            CellType dir1Cell = game.CheckGridCellType(GetNextCell(dir1));
            CellType dir2Cell = game.CheckGridCellType(GetNextCell(dir2));
            if(dir1Cell != CellType.wall)
            {
                possibleDirections.Add(dir1);
                canTurn = true;
            }
            if(dir2Cell != CellType.wall)
            {
                possibleDirections.Add(dir2);
                canTurn = true;
            }
            if (dir1Cell == CellType.wall && dir2Cell == CellType.wall)
            {
                possibleDirections.Clear();
                canTurn = false;
            }
        }

        protected virtual void OnHumanPositionChanged()
        {
            HumanPositionChangedEventArgs e = new HumanPositionChangedEventArgs(currentDirection, gridCell);
            HumanPositionChanged.Invoke(this, e);
        }
    }
}
