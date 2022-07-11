using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    public MyMinesweeper minesweeper;

    private void StartNewGame_Click(object sender, RoutedEventArgs e)
    {
        PlayingField.Children.Clear();
        StartNewGame();
    }

    private void StartNewGame()
    {
        if (EasyRB.IsChecked == true)
            minesweeper = new MyMinesweeper(10, 10, DifficultyLevel.Easy);
        else if (NormalRB.IsChecked == true)
            minesweeper = new MyMinesweeper(15, 15, DifficultyLevel.Normal);
        else if (HardRB.IsChecked == true)
            minesweeper = new MyMinesweeper(20, 20, DifficultyLevel.Hard);
        else if (ImpossibleRB.IsChecked == true)
            minesweeper = new MyMinesweeper(25, 25, DifficultyLevel.Impossible);

        minesweeper.GenerateGame();

        GeneratePlayingField(minesweeper.width, minesweeper.height);

        for (int i = 0; i < minesweeper.width; i++)
            for (int j = 0; j < minesweeper.height; j++)
            {
                if (minesweeper.mines[i, j] != 0)                               //если вокруг данной клетки есть мины или на самой клетке мина
                {
                    Label label = new Label();
                    label.FontWeight = FontWeights.UltraBlack;
                    if (minesweeper.mines[i, j] == -1)                          //если на клетке мина, то рисуем мину
                    {
                        label.Content = "*";
                        label.Foreground = new SolidColorBrush(Colors.DarkRed);
                    }
                    else                                                        //иначе ставим число мин вокруг
                    {
                        label.Content = minesweeper.mines[i, j];
                        label.Foreground = new SolidColorBrush(minesweeper.ColorOfNumberMinesAround[minesweeper.mines[i, j]]);
                    }
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.HorizontalAlignment = HorizontalAlignment.Center;

                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j);
                    PlayingField.Children.Add(label);
                }

                var b = new Button();
                int ii = i;//чертовы замыкания
                int jj = j;
                b.Click += (source, e) => MyClick(ii, jj);
                //b.Background = Brushes.Gray;

                Grid.SetRow(b, i);
                Grid.SetColumn(b, j);
                PlayingField.Children.Add(b);

                minesweeper.links[i, j] = b;
            }
    }

    private void GeneratePlayingField(int width, int height)
    {
        this.Height = height * 30 + 100;
        this.Width = width * 30 + 30;
        PlayingField.Children.Clear();
        PlayingField.RowDefinitions.Clear();
        PlayingField.ColumnDefinitions.Clear();

        for (int i = 0; i < height; i++)
        {
            var r = new RowDefinition();
            r.Height = new GridLength(1, GridUnitType.Star);
            PlayingField.RowDefinitions.Add(r);
        }

        for (int i = 0; i < width; i++)
        {
            var c = new ColumnDefinition();
            c.Width = new GridLength(1, GridUnitType.Star);
            PlayingField.ColumnDefinitions.Add(c);
        }

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.Gray;

                Grid.SetRow(border, i);
                Grid.SetColumn(border, j);
                PlayingField.Children.Add(border);
            }
    }

    private void MyClick(int i, int j)
    {
        if (minesweeper.links[i, j].Visibility == Visibility.Visible)
        {
            minesweeper.links[i, j].Visibility = Visibility.Hidden;
            if (minesweeper.mines[i, j] == -1)
            {
                GameOver();
                return;
            }
            if (minesweeper.mines[i, j] == 0)
                ShowAdjacentCells(i, j);
            minesweeper.notOpenCellsLeft--;
            if (minesweeper.notOpenCellsLeft == minesweeper.minesCount)
                Win();
        }
    }

    private void GameOver()
    {
        for (int x = 0; x < minesweeper.width; x++)
            for (int y = 0; y < minesweeper.height; y++)
            {
                minesweeper.links[x, y].IsEnabled = false;
                if (minesweeper.mines[x, y] == -1)
                    minesweeper.links[x, y].Visibility = Visibility.Hidden;
            }
    }

    private void Win()
    {
        new Thread(() => MessageBox.Show("Вы победили!!!")).Start();
        GameOver();
    }

    private void ShowAdjacentCells(int i, int j)    //открываем соседние клетки
    {
        if (i == 0)                                 //если это первая строка
        {
            if (j == 0)                             //и первый столбец, то это левый верхний угол. Далее аналогично
            {
                MyClick(i + 1, j);
                MyClick(i, j + 1);
                MyClick(i + 1, j + 1);
            }
            else if (j == minesweeper.height - 1)
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
        else if (i == minesweeper.width - 1)        //если последняя строка
        {
            if (j == 0)
            {
                MyClick(i - 1, j);
                MyClick(i - 1, j + 1);
                MyClick(i, j + 1);
            }
            else if (j == minesweeper.height - 1)
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
            else if (j == minesweeper.height - 1)
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
