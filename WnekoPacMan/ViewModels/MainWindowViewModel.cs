using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WnekoPacMan.ViewModels
{
    partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private int[] playerPosition = new int[] { 15, 15 };
        private int[] movementDirection = new int[] { 0, 1 };
        DispatcherTimer timer;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
            InitializeCommands();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            PlayerLeft += movementDirection[0];
            PlayerTop += movementDirection[1];
            for (int i = 0; i < 2; i++)
            {
                if (playerPosition[i] > 800) playerPosition[i] = 0;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int PlayerLeft
        {
            get => playerPosition[0] - 15;
            set
            {
                playerPosition[0] = value + 15;
                NotifyPropertyChanged();
            }
        }
        public int PlayerTop
        {
            get => playerPosition[1] - 15;
            set
            {
                playerPosition[1] = value + 15;
                NotifyPropertyChanged();
            }
        }

        public int MovementX
        {
            get => movementDirection[0];
            set => movementDirection[0] = value;
        }

        public int MovementY
        {
            get => movementDirection[1];
            set => movementDirection[1] = value;
        }

    }
}
