﻿using System;
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

        Directions nextDirection;
        public Human(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir) : base(gridSize, cellSize, game, cell, dir, playerColor)
        {
        }

        public Directions NextDirection { get => nextDirection; set => nextDirection = value; }

        public override void Move()
        {
            base.Move();
            newGridCell[1] = (playerPosition[0]) / cellSize; //column
            newGridCell[0] = (playerPosition[1]) / cellSize; //row
            if(gridCell[0] != newGridCell[0] || gridCell[1] != newGridCell[1])
            {
                OnHumanPositionChanged();
            }
            gridCell[0] = newGridCell[0];
            gridCell[1] = newGridCell[1];
            if (CheckIfInTheMiddle())
            {
                CellType currentCell = game.CheckGridCellType(gridCell);
                if(currentCell == CellType.dot || currentCell == CellType.energizer || currentCell == CellType.fruit)
                {
                    score += game.Eat(gridCell[0], gridCell[1]);
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

        protected virtual void OnHumanPositionChanged()
        {
            HumanPositionChangedEventArgs e = new HumanPositionChangedEventArgs(currentDirection, gridCell);
            HumanPositionChanged.Invoke(this, e);
        }
    }
}