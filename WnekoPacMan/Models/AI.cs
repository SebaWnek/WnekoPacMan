﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WnekoPacMan.Models
{
    public enum AIModes
    {
        Normal,
        Scatter,
        Random,
    }
    abstract class AI : Player, INotifyPropertyChanged
    {
        bool[] possibleDirrections = new bool[4]; //up, down, left, right
        protected int[] targetCell = new int[] { 0, 0 };
        protected int[] playerCell = new int[] { 0, 0 };
        protected Directions playerDirection;
        protected int[] scatterModeTargetCell;
        AIModes mode = AIModes.Normal;
        protected string name;
        protected Rectangle targetMark;
        protected bool showTarget = true;
        Brush basecolor;

        static readonly Dictionary<Directions, int> directionToNumber = new Dictionary<Directions, int>
        {
            {Directions.Up, 0 },
            {Directions.Down, 1 },
            {Directions.Left, 2 },
            {Directions.Right, 3 },
        };
        static readonly Dictionary<int, Directions> numberToDirection = new Dictionary<int, Directions>
        {
            {0, Directions.Up },
            {1, Directions.Down },
            {2, Directions.Left },
            {3, Directions.Right }
        };

        public AIModes Mode
        {
            get => mode;
            set
            {
                mode = value;
            }
        }

        public virtual void ChangeMode(AIModes newMode, SpeedModes newSpeed)
        {
            if (mode != AIModes.Random) TurnArround();
            mode = newMode;
            speedMode = newSpeed;
            ChangeSpeed(newSpeed);
        }


        private void TurnArround()
        {
            currentDirection = oppositeDirections[currentDirection];
        }

        public AI(int[] gridSize, int cellSize, Game game, int[] cell, Directions dir, Brush color, float speed) : base(gridSize, cellSize, game, cell, dir, color, speed)
        {
            if (showTarget)
            {
                mode = AIModes.Scatter;
                targetMark = new Rectangle();
                basecolor = color;
                targetMark.Fill = color;
                targetMark.Width = cellSize;
                targetMark.Height = cellSize;
                targetMark.Opacity = 0.5;
                Binding columnProperty = new Binding("TargetCellColumn");
                Binding rowProperty = new Binding("TargetCellRow");
                columnProperty.Source = this;
                rowProperty.Source = this;
                targetMark.SetBinding(Grid.RowProperty, rowProperty);
                targetMark.SetBinding(Grid.ColumnProperty, columnProperty);
            }
        }

        public void ChangeColor(int color)
        {
            switch (color)
            {
                case 1:
                    playerEllipse.Fill = Brushes.Blue;
                    break;
                case 0:
                    playerEllipse.Fill = Brushes.White;
                    break;
                default:
                    playerEllipse.Fill = basecolor;
                    break;
            }
        }

        public override void Move()
        {
            base.Move();
            CheckIfCaugth();
            int nextDirNumb;
            int count;
            if (CheckIfInTheMiddle())
            {
                if (game.CheckGridCellType(gridCell) == CellType.tunnel)
                {
                    speedModifier = Speeds[SpeedModes.Tunnel];
                }
                else
                {
                    speedModifier = Speeds[speedMode];
                }
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
                    SelectTargetCell();
                    currentDirection = ChooseNextDirection(possibleDirrections);
                }
                Array.Copy(movementDirections[currentDirection], movementDirection, 2);
                if (currentDirection == Directions.Down || currentDirection == Directions.Up)
                {
                    PlayerLeft = gridCell[1] * cellSize;
                }
                else
                {
                    PlayerTop = gridCell[0] * cellSize;
                }
                //Debug.WriteLine(targedCell[0] + ", " + targedCell[1]);
            }

        }

        private void CheckIfCaugth()
        {
            if (gridCell[0] == playerCell[0] && gridCell[1] == playerCell[1] && mode == AIModes.Random)
            {
                GetEaten();
            }
            else if (gridCell[0] == playerCell[0] && gridCell[1] == playerCell[1] && mode != AIModes.Random)
            {
                //MessageBox.Show("You're dead!");
            }
        }

        private async void GetEaten()
        {
            playerEllipse.Visibility = Visibility.Hidden;
            isStopped = true;
            await Task.Delay(1000);
            playerEllipse.Visibility = Visibility.Visible;
            isStopped = false;
            MoveToStartPosition();
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
                    tmpDistance = CalculateDistance(tmpGridCell, targetCell);
                    if (tmpDistance < smallestDistance)
                    {
                        smallestDistance = tmpDistance;
                        selectedDirection = i;
                    }
                }
            }
            return numberToDirection[selectedDirection];
        }

        protected double CalculateDistance(int[] A, int[] B)
        {
            double x = Math.Abs((double)A[0] - (double)B[0]);
            double y = Math.Abs((double)A[1] - (double)B[1]);
            return Math.Sqrt(x * x + y * y);
        }

        protected virtual void SelectTargetCell()
        {
            switch (Mode)
            {
                case AIModes.Scatter:
                    targetCell = scatterModeTargetCell;
                    break;
                case AIModes.Random:
                    targetCell = new int[] { rnd.Next(0, gridSize[0]), rnd.Next(0, gridSize[1]) };
                    break;
                default:
                    targetCell = ChooseCell();
                    Debug.WriteLine(name + ": " + targetCell[0] + ", " + targetCell[1]);
                    break;
            }
            NotifyPropertyChanged("TargetCellColumn");
            NotifyPropertyChanged("TargetCellRow");
        }

        public int TargetCellRow
        {
            get => targetCell[0];
        }
        public int TargetCellColumn
        {
            get => targetCell[1];
        }

        public UIElement GetTargetGraphics()
        {
            return targetMark;
        }

        protected virtual int[] ChooseCell()
        {
            return playerCell;
        }

        public void OnHumanPositionChanged(object sender, HumanPositionChangedEventArgs e)
        {
            playerCell = e.GridCell;
            playerDirection = e.Direction;
        }
    }
}
