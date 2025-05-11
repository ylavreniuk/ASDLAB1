using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        int[] sizes = { 10, 100, 1000, 10000 };
        int[] dataSmallest = GenerateRandomArray(sizes[0], 0, 1000);
        int[] dataSmall = GenerateRandomArray(sizes[1], 0, 1000);
        int[] dataBig = GenerateRandomArray(sizes[2], 0, 1000);
        int[] dataLargest = GenerateRandomArray(sizes[3], 0, 10000);

        int[] dataAlmostSorted = (int[])dataLargest.Clone();
        Array.Sort(dataAlmostSorted);
        Array.Reverse(dataAlmostSorted, (int)(dataAlmostSorted.Length * 0.9), dataAlmostSorted.Length - (int)(dataAlmostSorted.Length * 0.9));

        int[] dataReversed = (int[])dataLargest.Clone();
        Array.Reverse(dataReversed);

        Console.WriteLine("Невідсортовані масиви:");
        PrintArray(dataSmallest, "Малий масив");
        PrintArray(dataSmall, "Середній масив");
        PrintArray(dataBig, "Великий масив");
        PrintArray(dataLargest, "Найбільший масив");

        // Сортування
        CompareSortingAlgorithms(new[] { dataSmallest, dataSmall, dataBig, dataLargest, dataAlmostSorted, dataReversed });

        // Демонстрація пошуку
        Console.WriteLine("\nПОШУК:");
        var searchTable = new List<string>();
        searchTable.Add("Розмір;Метод;Шукане;Результат;Час (мс)");

        int[] searchTargets = { 50, 999, 10000 }; // кілька елементів для демонстрації

        int[][] arraysForSearch = { dataSmallest, dataSmall, dataBig, dataLargest };
        foreach (var arr in arraysForSearch)
        {
            int[] sorted = (int[])arr.Clone();
            Array.Sort(sorted);
            foreach (int target in searchTargets)
            {
                // Лінійний
                searchTable.Add(FormatSearchResult(arr.Length, "Лінійний", target, LinearSearch(arr, target)));

                // Бінарний
                searchTable.Add(FormatSearchResult(sorted.Length, "Бінарний", target, BinarySearch(sorted, target)));

                // Інтерполяційний
                searchTable.Add(FormatSearchResult(sorted.Length, "Інтерполяційний", target, InterpolationSearch(sorted, target)));
            }
        }

        Console.WriteLine("\nТаблиця результатів пошуку:");
        foreach (var line in searchTable)
            Console.WriteLine(line);
    }

    static string FormatSearchResult(int size, string method, int key, (int index, double timeMs) result)
    {
        return $"{size};{method};{key};{(result.index >= 0 ? $"Знайдено (індекс {result.index})" : "Немає")};{result.timeMs:F6}";
    }

    static (int index, double timeMs) LinearSearch(int[] arr, int key)
    {
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < arr.Length; i++)
            if (arr[i] == key)
                return (i, sw.Elapsed.TotalMilliseconds);
        return (-1, sw.Elapsed.TotalMilliseconds);
    }

    static (int index, double timeMs) BinarySearch(int[] arr, int key)
    {
        var sw = Stopwatch.StartNew();
        int left = 0, right = arr.Length - 1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            if (arr[mid] == key)
                return (mid, sw.Elapsed.TotalMilliseconds);
            if (arr[mid] < key) left = mid + 1;
            else right = mid - 1;
        }
        return (-1, sw.Elapsed.TotalMilliseconds);
    }

    static (int index, double timeMs) InterpolationSearch(int[] arr, int key)
    {
        var sw = Stopwatch.StartNew();
        int low = 0, high = arr.Length - 1;
        while (low <= high && key >= arr[low] && key <= arr[high])
        {
            if (arr[low] == arr[high]) break;
            int pos = low + ((key - arr[low]) * (high - low)) / (arr[high] - arr[low]);
            if (pos < 0 || pos >= arr.Length) break;
            if (arr[pos] == key) return (pos, sw.Elapsed.TotalMilliseconds);
            if (arr[pos] < key) low = pos + 1;
            else high = pos - 1;
        }
        return (-1, sw.Elapsed.TotalMilliseconds);
    }

    static int[] GenerateRandomArray(int size, int minValue, int maxValue)
    {
        Random random = new Random();
        return Enumerable.Range(0, size).Select(_ => random.Next(minValue, maxValue)).ToArray();
    }

    static void PrintArray(int[] array, string name)
    {
        Console.WriteLine($"{name}: {string.Join(", ", array.Take(10))}...");
    }

    static void CompareSortingAlgorithms(int[][] datasets)
    {
        var sortingAlgorithms = new (string, Func<int[], int[]>)[]
        {
            ("Insertion Sort", InsertionSort),
            ("Shell Sort", ShellSort)
        };

        Console.WriteLine("\nПорівняння алгоритмів сортування:");
        foreach (var (name, sortFunc) in sortingAlgorithms)
        {
            Console.WriteLine($"\n{name}:");
            foreach (var data in datasets)
            {
                int[] copy = (int[])data.Clone();
                var watch = Stopwatch.StartNew();
                sortFunc(copy);
                watch.Stop();
                Console.WriteLine($"Масив {copy.Length} елементів -> {watch.Elapsed.TotalMilliseconds:F6} мс");
            }
        }
    }

    static int[] InsertionSort(int[] arr)
    {
        for (int i = 1; i < arr.Length; i++)
        {
            int key = arr[i];
            int j = i - 1;
            while (j >= 0 && arr[j] > key)
            {
                arr[j + 1] = arr[j];
                j--;
            }
            arr[j + 1] = key;
        }
        return arr;
    }

    static int[] ShellSort(int[] arr)
    {
        int n = arr.Length;
        for (int gap = n / 2; gap > 0; gap /= 2)
        {
            for (int i = gap; i < n; i++)
            {
                int temp = arr[i];
                int j;
                for (j = i; j >= gap && arr[j - gap] > temp; j -= gap)
                {
                    arr[j] = arr[j - gap];
                }
                arr[j] = temp;
            }
        }
        return arr;
    }
}

