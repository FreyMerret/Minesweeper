using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyGame;

public partial class MainWindow : Window
{
    private MyMinesweeper minesweeper = new MyMinesweeper(DifficultyLevel.Easy);

    public MainWindow()
    {
        InitializeComponent();
        StartNewGame();
    }    

    private void StartNewGame_Click(object sender, RoutedEventArgs e)
    {              
        StartNewGame();
    }

    private void StartNewGame()
    {
        GeneratePlayingField(minesweeper.width, minesweeper.height);
        List<Label> labels;
        List<Button> buttons;
        minesweeper.GenerateGame(out labels, out buttons);        

        foreach(var label in labels)
            PlayingField.Children.Add(label);
        foreach (var button in buttons)
            PlayingField.Children.Add(button);
    }

    private void ChangeDifficulty_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        if (menuItem != null)
            minesweeper = new MyMinesweeper((DifficultyLevel)menuItem.Tag);
        StartNewGame();
    }

    private void GeneratePlayingField(int width, int height)
    {
        this.Height = height * 30 + 110;
        this.Width = width * 30 + 30;
        PlayingField.Children.Clear();
        PlayingField.RowDefinitions.Clear();
        PlayingField.ColumnDefinitions.Clear();

        for (int i = 0; i < height; i++)
            PlayingField.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        
        for (int i = 0; i < width; i++)
            PlayingField.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                Border border = new Border()
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Gray
                };

                Grid.SetColumn(border, i);
                Grid.SetRow(border, j);
                PlayingField.Children.Add(border);
            }
    }
}
