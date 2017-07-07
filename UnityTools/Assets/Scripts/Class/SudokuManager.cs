//
//  SudokuManager.cs
//  UnityTools
//
//  Created by JYMain on 6/29/2017.
//

using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SudokuManager : MonoBehaviour
{
    private readonly int[,] Grid = new int[9, 9];
    private readonly int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8, 9};
    private readonly Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                Grid[i, j] = 0;
            }
        }
#if UNITY_EDITOR
        stopwatch.Reset();
        stopwatch.Start();
#endif
        var randomList = GetRandomNumber();
        for (var i = 0; i < 9; i++)
            Grid[0, i] = randomList[i];
        GridStepByStep(0, 0);
#if UNITY_EDITOR
        stopwatch.Stop();
        PrintGameGrid(Grid);
        Debug.Log(string.Format("StopWatch: {0}ms", stopwatch.ElapsedMilliseconds));
#endif
    }

    private bool GridStepByStep(int row, int col)
    {
        if (row >= 9 || col >= 9) return true;

        if (Grid[row, col] != 0)
        {
            if (col + 1 < 9) return GridStepByStep(row, col + 1);

            if (row + 1 < 9) return GridStepByStep(row + 1, 0);
            return true;
        }
        var random = GetRandomNumber();
        for (var i = 0; i < random.Length; i++)
        {
            if (IsAvailable(row, col, random[i]))
            {
                Grid[row, col] = random[i];
                if (col + 1 < 9)
                {
                    if (GridStepByStep(row, col + 1)) return true;
                    Grid[row, col] = 0;
                }
                else if (row + 1 < 9)
                {
                    if (GridStepByStep(row + 1, 0)) return true;
                    Grid[row, col] = 0;
                }
                else return true;
            }
        }
        return false;
    }

    /// <summary>
    ///     Check whether the conflict
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    private bool IsAvailable(int row, int col, int number)
    {
        var rowStart = (row/3)*3;
        var colStart = (col/3)*3;
        int cellrow = 0, cellcol = 0;
        for (var i = 0; i < 9; i++)
        {
            if (col != i && Grid[row, i] == number) return false;

            if (row != i && Grid[i, col] == number) return false;

            cellrow = rowStart + (i%3);
            cellcol = colStart + (i/3);
            if (Grid[cellrow, cellcol] == number && !(row == cellrow && col == cellrow)) return false;
        }
        return true;
    }

    /// <summary>
    ///     get random list nubmer
    /// </summary>
    /// <returns></returns>
    private int[] GetRandomNumber()
    {
        var randomNumber = new int[9];
        var list = numbers.ToList();
        for (var i = 0; i < 9; i++)
        {
            randomNumber[i] = list[Random.Range(0, list.Count)];
            list.Remove(randomNumber[i]);
        }
        return randomNumber;
    }

    private void PrintGameGrid(int[,] grid)
    {
        var fillString = "";
        for (var i = 0; i < 9; i++)
        {
            if (i != 0 && i%3 == 0)
            {
                for (var j = 0; j < 11; j++)
                {
                    fillString += "-\t";
                }
                fillString += "\n";
            }
            for (var j = 0; j < 9; j++)
            {
                if (j != 0 && j%3 == 0)
                    fillString += "|\t";
                fillString += grid[i, j] + "\t";
            }
            fillString += "\n";
        }

        Debug.Log(fillString);
    }
}