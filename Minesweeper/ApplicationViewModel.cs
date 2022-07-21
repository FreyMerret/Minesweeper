using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private MyMinesweeper minesweeper;

        public MyMinesweeper Minesweeper
        {
            get { return minesweeper; }
            set
            {
                minesweeper = value;
                OnPropertyChanged("Minesweeper");
            }
        }

        public ApplicationViewModel()
        {            
        }

        private RelayCommand сhooseDifficulty;
        public RelayCommand ChooseDifficulty
        {
            get
            {
                return сhooseDifficulty ??
                    (сhooseDifficulty = new RelayCommand(obj =>
                    {
                        DifficultyLevel difficulty = (DifficultyLevel)obj;
                        Minesweeper = new MyMinesweeper(difficulty);
                    }));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
