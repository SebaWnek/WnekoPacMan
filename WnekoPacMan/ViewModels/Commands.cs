using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WnekoPacMan.Commands;

namespace WnekoPacMan.ViewModels
{
    partial class MainWindowViewModel
    {
        CommandHandler moveUpCommand;
        CommandHandler moveDownCommand;
        CommandHandler moveLeftCommand;
        CommandHandler moveRightCommand;

        public CommandHandler MoveUpCommand { get => moveUpCommand; set => moveUpCommand = value; }
        public CommandHandler MoveDownCommand { get => moveDownCommand; set => moveDownCommand = value; }
        public CommandHandler MoveLeftCommand { get => moveLeftCommand; set => moveLeftCommand = value; }
        public CommandHandler MoveRightCommand { get => moveRightCommand; set => moveRightCommand = value; }

        private void InitializeCommands()
        {
            MoveUpCommand = new CommandHandler(MoveUp, (o) => true);
            MoveDownCommand = new CommandHandler(MoveDown, (o) => true);
            MoveLeftCommand = new CommandHandler(MoveLeft, (o) => true);
            MoveRightCommand = new CommandHandler(MoveRight, (o) => true);
        }

        private void MoveUp(object o)
        {
            game.Human.NextDirection = Models.Directions.Up;
        }
        private void MoveDown(object o)
        {
            game.Human.NextDirection = Models.Directions.Down;
        }
        private void MoveLeft(object o)
        {
            game.Human.NextDirection = Models.Directions.Left;
        }
        private void MoveRight(object o)
        {
            game.Human.NextDirection = Models.Directions.Right;
        }
    }
}
