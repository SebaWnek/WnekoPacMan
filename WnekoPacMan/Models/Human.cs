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
        static Brush playerColor = Brushes.Yellow;
        int score = 0;
        bool shouldMove = true;
        int stopCounter = 0;

        Directions nextDirection;
        public Human(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir) : base(gridSize, cellSize, game, cell, dir, playerColor)
        {
        }

        public Directions NextDirection { get => nextDirection; set => nextDirection = value; }

        public override void Move()
        {
            if (shouldMove)
            {
                base.Move();

                newGridCell[1] = (playerPosition[0]) / cellSize; //column
                newGridCell[0] = (playerPosition[1]) / cellSize; //row
                if (gridCell[0] != newGridCell[0] || gridCell[1] != newGridCell[1])
                {
                    OnHumanPositionChanged();
                }
                gridCell[0] = newGridCell[0];
                gridCell[1] = newGridCell[1];
                if (CheckIfInTheMiddle())
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

                    if (currentCell == CellType.dot || currentCell == CellType.energizer || currentCell == CellType.fruit)
                    {
                        
                    }
                    int[] nextCellStraigth = GetNextCell(currentDirection);
                    CellType nextInLine = game.CheckGridCellType(nextCellStraigth);
                    int[] nextCellTurn = GetNextCell(NextDirection);
                    CellType nextAfterTurn = game.CheckGridCellType(nextCellTurn);
                    if (currentDirection != NextDirection)
                    {
                        if (nextAfterTurn != CellType.border && nextAfterTurn != CellType.wall)
                        {
                            currentDirection = NextDirection;
                            movementDirection = movementDirections[currentDirection];
                            OnHumanPositionChanged();
                            return;
                        }
                    }
                    if (nextInLine == CellType.wall)
                    {
                        movementDirection = movementDirections[Directions.Stop];
                    }

                }
            }
            else
            {
                stopCounter--;
                if (stopCounter == 0) shouldMove = true;
            }


        }

        protected virtual void OnHumanPositionChanged()
        {
            HumanPositionChangedEventArgs e = new HumanPositionChangedEventArgs(currentDirection, gridCell);
            HumanPositionChanged.Invoke(this, e);
        }
    }
}
