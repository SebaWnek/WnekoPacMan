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
        CommandHandler MoveUpCommand;
        CommandHandler MoveDownCommand;
        CommandHandler MoveLeftCommand;
        CommandHandler MoveRightCommand;
        
        private void InitializeCommands()
        {
            MoveUpCommand = new CommandHandler(MoveUp, (o) => true);
            MoveDownCommand = new CommandHandler(MoveDown, (o) => true);
            MoveLeftCommand = new CommandHandler(MoveLeft, (o) => true);
            MoveRightCommand = new CommandHandler(MoveRight, (o) => true);
        }

        private void MoveUp(object o)
        {
            MovementY = -1;
        }
        private void MoveDown(object o)
        {
            MovementY = 1;
        }
        private void MoveLeft(object o)
        {
            MovementX = -1;
        }
        private void MoveRight(object o)
        {
            MovementX = 1;
        }
    }
}
