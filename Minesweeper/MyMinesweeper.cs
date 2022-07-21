using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper;

public enum DifficultyLevel : int
{
    Easy = 10,
    Normal = 15,
    Hard = 25,
    Impossible = 30
}

public class MyMinesweeper : INotifyPropertyChanged
{
    public static Dictionary<int, Color> ColorOfNumberMinesAround = new Dictionary<int, Color>
    {
        {1, Colors.Blue},
        {2, Colors.Green},
        {3, Colors.Red},
        {4, Colors.Purple},
        {5, Colors.DarkRed},
        {6, Colors.YellowGreen},
        {7, Colors.Peru},
        {8, Colors.White}
    };


    public int width { get; private set; }
    public int height { get; private set; }
    public int difficultyLevel { get; private set; }
    public int notOpenCellsLeft { get; set; }
    public int minesCount { get; private set; }
    
    public int MinesCount
    {
        get { return minesCount; }
        private set
        {
            minesCount = value;
            OnPropertyChanged("MinesCount");
        }
    }
    public int[,] mines { get; private set; }

    public FrameworkElement[,] links { get; private set; }

    public MyMinesweeper(DifficultyLevel difficultyLevel)
    {
        this.width = this.height =  this.difficultyLevel = (int)difficultyLevel;
        this.links = new Button[width, height];
        MinesCount = (int)difficultyLevel;
    }

    public MyMinesweeper(int width, int height, int difficultyLevel)
    {
        if (width < 10 || height < 10)
            throw new Exception("Too small playing field");
        if (width > 30 || height > 30)
            throw new Exception("Too big playing field");
        if (difficultyLevel < 5)
            throw new Exception("Too small mines percent");
        if (difficultyLevel > 95)
            throw new Exception("Too big mines percent");

        this.width = width;
        this.height = height;
        this.difficultyLevel = difficultyLevel;
        this.links = new Button[width, height];
    }

    public void GenerateGame()
    {
        Random random = new Random(DateTime.Now.Second);
        int ThresholdValue = difficultyLevel;                       //пороговое значение вероятности появления мины в клетке 
        int[,] onlyMines = new int[width, height];                  //поле на котором будут только мины. Нужно для последующего создания поля с минами и цифрами
        mines = new int[width, height];
        minesCount = 0;
        notOpenCellsLeft = width * height;
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (random.Next(0, 100) <= ThresholdValue)
                {
                    onlyMines[i, j] = -1;                         //ставим мину
                    mines[i, j] = -1;
                    minesCount++;
                }
            }

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (mines[i, j] != -1)
                    mines[i, j] = CheckMinesAround(i, j);
            }

        int CheckMinesAround(int y, int x)
        {
            if (y == 0)
            {
                if (x == 0)
                    return -(onlyMines[0, 1] + onlyMines[1, 1] + onlyMines[1, 0]);
                else
                if (x == height - 1)
                    return -(onlyMines[0, height - 2] + onlyMines[1, height - 2] + onlyMines[1, height - 1]);
                else
                    return -(onlyMines[0, x - 1] + onlyMines[1, x - 1] + onlyMines[1, x] + onlyMines[1, x + 1] + onlyMines[0, x + 1]);
            }
            else
            if (y == width - 1)
            {
                if (x == 0)
                    return -(onlyMines[width - 1, 1] + onlyMines[width - 2, 1] + onlyMines[width - 2, 0]);
                else
                if (x == height - 1)
                    return -(onlyMines[width - 1, height - 2] + onlyMines[width - 2, height - 2] + onlyMines[width - 2, height - 1]);
                else
                    return -(onlyMines[width - 1, x - 1] + onlyMines[width - 2, x - 1] + onlyMines[width - 2, x] + onlyMines[width - 2, x + 1] + onlyMines[width - 1, x + 1]);
            }
            else
            {
                if (x == 0)
                    return -(onlyMines[y - 1, 0] + onlyMines[y - 1, 1] + onlyMines[y, 1] + onlyMines[y + 1, 1] + onlyMines[y + 1, 0]);
                else
                if (x == height - 1)
                    return -(onlyMines[y - 1, height - 1] + onlyMines[y - 1, height - 2] + onlyMines[y, height - 2] + onlyMines[y + 1, height - 2] + onlyMines[y + 1, height - 1]);
                else
                    return -(onlyMines[y - 1, x - 1] + onlyMines[y - 1, x] + onlyMines[y - 1, x + 1] + onlyMines[y, x + 1] + onlyMines[y + 1, x + 1] + onlyMines[y + 1, x] + onlyMines[y + 1, x - 1] + onlyMines[y, x - 1]);
            }
        }

    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}

