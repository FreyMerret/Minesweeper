using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper;

public enum DifficultyLevel : int
{
    Easy = 1,
    Normal,
    Hard,
    Impossible
}

public class MyMinesweeper
{
    public Dictionary<int, Color> ColorOfNumberMinesAround = new Dictionary<int, Color>
    {
        {1, Colors.Blue},
        {2, Colors.Green},
        {3, Colors.Red},
        {4, Colors.Purple},
        {5, Colors.DarkRed},
        {6, Colors.Yellow},
        {7, Colors.Peru},
        {8, Colors.White}
    };


    public int width { get; private set; }
    public int height { get; private set; }
    public DifficultyLevel difficultyLevel { get; private set; }
    public int notOpenCellsLeft { get; set; }
    public int minesCount { get; private set; }
    public int[,] mines { get; private set; }

    public FrameworkElement[,] links { get; private set; }

    public MyMinesweeper(int width, int height, DifficultyLevel difficultyLevel)
    {
        this.width = width;
        this.height = height;
        this.difficultyLevel = difficultyLevel;
        this.links = new Button[width, height];
    }

    public void GenerateGame()
    {
        Random random = new Random(DateTime.Now.Second);
        int ThresholdValue = (int)difficultyLevel * 10;             //пороговое значение вероятности появления мины в клетке 
        int[,] onlyMines = new int[height, width];                  //поле на котором будут только мины. Нужно для последующего создания поля с минами и цифрами
        mines = new int[height, width];
        minesCount = 0;
        notOpenCellsLeft = height * width;
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                if (random.Next(0, 100) <= ThresholdValue)
                {
                    onlyMines[i, j] = -1;                         //ставим мину
                    mines[i, j] = -1;
                    minesCount++;
                }
            }

        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
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
                if (x == width - 1)
                    return -(onlyMines[0, width - 2] + onlyMines[1, width - 2] + onlyMines[1, width - 1]);
                else
                    return -(onlyMines[0, x - 1] + onlyMines[1, x - 1] + onlyMines[1, x] + onlyMines[1, x + 1] + onlyMines[0, x + 1]);
            }
            else
            if (y == height - 1)
            {
                if (x == 0)
                    return -(onlyMines[height - 1, 1] + onlyMines[height - 2, 1] + onlyMines[height - 2, 0]);
                else
                if (x == width - 1)
                    return -(onlyMines[height - 1, width - 2] + onlyMines[height - 2, width - 2] + onlyMines[height - 2, width - 1]);
                else
                    return -(onlyMines[height - 1, x - 1] + onlyMines[height - 2, x - 1] + onlyMines[height - 2, x] + onlyMines[height - 2, x + 1] + onlyMines[height - 1, x + 1]);
            }
            else
            {
                if (x == 0)
                    return -(onlyMines[y - 1, 0] + onlyMines[y - 1, 1] + onlyMines[y, 1] + onlyMines[y + 1, 1] + onlyMines[y + 1, 0]);
                else
                if (x == width - 1)
                    return -(onlyMines[y - 1, width - 1] + onlyMines[y - 1, width - 2] + onlyMines[y, width - 2] + onlyMines[y + 1, width - 2] + onlyMines[y + 1, width - 1]);
                else
                    return -(onlyMines[y - 1, x - 1] + onlyMines[y - 1, x] + onlyMines[y - 1, x + 1] + onlyMines[y, x + 1] + onlyMines[y + 1, x + 1] + onlyMines[y + 1, x] + onlyMines[y + 1, x - 1] + onlyMines[y, x - 1]);
            }
        }
    }
}

