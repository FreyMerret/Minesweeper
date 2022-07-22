using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyGame;

public enum DifficultyLevel : int   //цифра указывает процентное количество мин на поле
{
    Easy = 10,
    Normal = 15,
    Hard = 20
}

public class MyMinesweeper
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
    public int[,] mines { get; private set; }
    public FrameworkElement[,] links { get; private set; }








    public MyMinesweeper(DifficultyLevel difficultyLevel)
    {
        this.width = this.height = this.difficultyLevel = (int)difficultyLevel;
        this.links = new Button[width, height];
    }

    public void GenerateGame(out List<Label> labels, out List<Button> buttons)
    {
        Random random = new Random(DateTime.Now.Second);
        minesCount = (int)Math.Round(width * height * ((double)difficultyLevel / 100));
        int[,] onlyMines = new int[width, height];                  //поле на котором будут только мины. Нужно для последующего создания поля с минами и цифрами
        mines = new int[width, height];
        notOpenCellsLeft = width * height;
        for (int i = 0; i < minesCount; i++)                        //устанавливаем мины
        {
            do                                                      //пока не найдем пустую клетку для постановки мины, выбираем рандомные клетки
            {
                int randomX = random.Next(0, width);
                int randomY = random.Next(0, height);

                if (onlyMines[randomX, randomY] != -1)
                {
                    onlyMines[randomX, randomY] = -1;               //ставим мину
                    mines[randomX, randomY] = -1;
                    break;
                }
            }
            while (true);
        }

        for (int i = 0; i < width; i++)                             //считаем количество мин вокруг
            for (int j = 0; j < height; j++)
            {
                if (mines[i, j] != -1)
                    mines[i, j] = CheckMinesAround(i, j);
            }

        int CheckMinesAround(int y, int x)                          //функция для подсчета мин вокруг. Вынес ее, чтобы было удобно сворачивать
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

        labels = new List<Label>();
        buttons = new List<Button>();

        for (int i = 0; i < width; i++)                             //формируем два списка: с кнопками и с метками, которые лежат под кнопками
            for (int j = 0; j < height; j++)
            {
                if (mines[i, j] != 0)                               //если вокруг данной клетки есть мины или на самой клетке мина
                {
                    Label label = new Label()
                    {
                        FontWeight = FontWeights.UltraBlack,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    if (mines[i, j] == -1)                          //если на клетке мина, то рисуем мину
                    {
                        label.Content = "*";
                        label.Foreground = new SolidColorBrush(Colors.DarkRed);
                    }
                    else                                            //иначе ставим число мин вокруг
                    {
                        label.Content = mines[i, j];
                        label.Foreground = new SolidColorBrush(MyMinesweeper.ColorOfNumberMinesAround[mines[i, j]]);
                    }

                    Grid.SetColumn(label, i);
                    Grid.SetRow(label, j);
                    labels.Add(label);
                }

                var button = new Button();
                button.PreviewMouseLeftButtonUp += (source, e) => MyClick(source, e);
                button.PreviewMouseRightButtonUp += (source, e) => RightButtonClick(source, e);
                Grid.SetColumn(button, i);
                Grid.SetRow(button, j);
                buttons.Add(button);
                links[i, j] = button;
            }

    }

    private void RightButtonClick(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        if (button != null)                     //установка флажка
        {
            if (button.Content != "F")
                button.Content = "F";
            else
                button.Content = "";
        }
    }

    private void MyClick(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        if (button != null)
            if (button.Content != "F")
                MyClick(Grid.GetColumn(button), Grid.GetRow(button));
    }

    private void MyClick(int i, int j)
    {
        if (links[i, j].Visibility == Visibility.Visible)   //открытие ячейки, если она не не открыта. Это сделано так, чтобы при попадении на ячейку, вокруг которой нет мин была возможность открыть ячейки вокруг не утонув в рекурсии
        {
            links[i, j].Visibility = Visibility.Hidden;
            if (mines[i, j] == -1)
            {
                GameOver();
                return;
            }
            if (mines[i, j] == 0)
                ShowAdjacentCells(i, j);                    //открываем соседние ячейки, если вокруг текущей нет мин
            notOpenCellsLeft--;
            if (notOpenCellsLeft == minesCount)
                Win();
        }
    }

    private void GameOver()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                links[x, y].IsEnabled = false;
                if (mines[x, y] == -1)
                    links[x, y].Visibility = Visibility.Hidden; //показываем мины
            }
    }

    private void Win()
    {
        MessageBox.Show("Вы победили!!!");
        GameOver();
    }

    private void ShowAdjacentCells(int i, int j)    //открываем соседние клетки, если вокруг клетки i,j нет мин
    {
        if (i == 0)                                 //если это первая строка
        {
            if (j == 0)                             //и первый столбец, то это левый верхний угол и нужно открыть не все клетки вокруг, чтобы не поймать ошибку. Далее аналогично
            {
                MyClick(i + 1, j);
                MyClick(i, j + 1);
                MyClick(i + 1, j + 1);
            }
            else if (j == height - 1)
            {
                MyClick(i, j - 1);
                MyClick(i + 1, j - 1);
                MyClick(i + 1, j);
            }
            else
            {
                MyClick(i, j + 1);
                MyClick(i + 1, j + 1);
                MyClick(i + 1, j);
                MyClick(i + 1, j - 1);
                MyClick(i, j - 1);
            }
        }
        else if (i == width - 1)        //если последняя строка
        {
            if (j == 0)
            {
                MyClick(i - 1, j);
                MyClick(i - 1, j + 1);
                MyClick(i, j + 1);
            }
            else if (j == height - 1)
            {
                MyClick(i, j - 1);
                MyClick(i - 1, j - 1);
                MyClick(i - 1, j);
            }
            else
            {
                MyClick(i, j - 1);
                MyClick(i - 1, j - 1);
                MyClick(i - 1, j);
                MyClick(i - 1, j + 1);
                MyClick(i, j + 1);
            }
        }
        else
        {
            if (j == 0)
            {
                MyClick(i - 1, j);
                MyClick(i - 1, j + 1);
                MyClick(i, j + 1);
                MyClick(i + 1, j + 1);
                MyClick(i + 1, j);
            }
            else if (j == height - 1)
            {
                MyClick(i - 1, j);
                MyClick(i - 1, j - 1);
                MyClick(i, j - 1);
                MyClick(i + 1, j - 1);
                MyClick(i + 1, j);
            }
            else
            {
                MyClick(i + 1, j);
                MyClick(i + 1, j + 1);
                MyClick(i, j + 1);
                MyClick(i - 1, j + 1);
                MyClick(i - 1, j);
                MyClick(i - 1, j - 1);
                MyClick(i, j - 1);
                MyClick(i + 1, j - 1);
            }
        }
    }
}

