using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

/// <summary>
/// Предоставляет статические методы для работы с матрицами нейронных сетей.
/// </summary>
public static class NMatrix
{
    private static Random random = new Random();

    /// <summary>
    /// Создаёт двумерный массив со случайными числами диапазон которых тем меньше чем больше width.
    /// </summary>
    /// <param name="height">Высота массива.</param>
    /// <param name="width">Ширина массива.</param>
    public static double[,] ArrayRandomNormal(int height, int width)
    {
        double[,] result = new double[height, width];
        double x = 1 / Math.Sqrt(width);

        for (int i = 0; i < height; i++)
            for (int n = 0; n < width; n++)
                result[i, n] = random.NextDouble() * (x * 2) - x;

        return result;
    }

    /// <summary>
    /// Создаёт одномерный массив со случайными числами диапазон которых тем меньше чем больше length.
    /// </summary>
    /// <param name="length">Длина массива.</param>
    public static double[] ArrayRandomNormal(int length)
    {
        double[] result = new double[length];
        double x = 1 / Math.Sqrt(length);

        for (int i = 0; i < length; i++)
            result[i] = random.NextDouble() * (x * 2) - x;

        return result;
    }

    /// <summary>
    /// Создаёт двумерный массив со случайными числами в диапазоне 0.0...1.0.
    /// </summary>
    /// <param name="height">Высота массива.</param>
    /// <param name="width">Ширина массива.</param>
    public static double[,] ArrayRandom(int height, int width)
    {
        double[,] result = new double[height, width];

        for (int i = 0; i < height; i++)
            for (int n = 0; n < width; n++)
                result[i, n] = random.NextDouble();

        return result;
    }

    /// <summary>
    /// Создаёт одномерный массив со случайными числами в диапазоне 0.0...1.0.
    /// </summary>
    /// <param name="length">Длина массива.</param>
    public static double[] ArrayRandom(int length)
    {
        double[] result = new double[length];

        for (int i = 0; i < length; i++)
            result[i] = random.NextDouble();

        return result;
    }

    /// <summary>
    /// Применяет ко всем элементам <see cref="Matrix"/> сигмоидальную функцию активации.
    /// </summary>
    /// <param name="matrix">Матрица к которой нужно применить сигмоидальную функцию активации.</param>
    public static Matrix SigmoidActivationFunction(this Matrix matrix)
    {
        var result = new Matrix(matrix.Height, matrix.Width);

        for (int i = 0; i < result.Height; i++)
            for (int j = 0; j < result.Width; j++)
                result[i, j] = 1 / (1 + Math.Exp(matrix[i, j] * (-1)));
        return result;
    }

    /// <summary>
    /// Применяет ко всем элементам <see cref="Matrix"/> логарифмическую функцию активации.
    /// </summary>
    /// <param name="matrix">Матрица к которой нужно применить логарифмическую функцию активации.</param>
    public static Matrix LogActivationFunction(this Matrix matrix)
    {
        var result = new Matrix(matrix.Height, matrix.Width);

        for (int i = 0; i < result.Height; i++)
            for (int j = 0; j < result.Width; j++)
                result[i, j] = Math.Log(matrix[i, j] / (1 - matrix[i, j]));
        return result;
    }

    /// <summary>
    /// Выполняет матричное умножение <see cref="Matrix"/>.
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="matrix2"></param>
    public static Matrix Dot(Matrix matrix, Matrix matrix2)
    {
        if (matrix.Width != matrix2.Height)
            throw new ArgumentException("Данные матрицы не могу выполнить матричное умножение");

        var result = new Matrix(matrix.Height, matrix2.Width);
        Parallel.For(0, matrix2.Width, i =>
        {
            for (int j = 0; j < matrix.Height; j++)
                for (int k = 0; k < matrix.Width; k++)
                    result[j, i] += matrix[j, k] * matrix2[k, i];
        });

        return result;
    }

    /// <summary>
    /// Перевести тип данных массива в тип данных <see cref="double"/>.
    /// </summary>
    public static double[] ToDouble<T>(this T[] array)
    {
        double[] result = new double[array.Length];
        Parallel.For(0, array.Length, i =>
        {
            result[i] = double.Parse(array[i].ToString());
        });
        return result;
    }

    /// <summary>
    /// Вырезать массив.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startIndex">Начальный индекс.</param>
    public static T[] Cut<T>(this T[] array, int startIndex)
    {
        T[] result = new T[array.Length - startIndex];
        for (int n = startIndex, i = 0; n < array.Length; n++, i++)
            result[i] = array[n];

        return result;
    }

    /// <summary>
    /// Вырезать массив.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startIndex">Начальный индекс.</param>
    /// <param name="endIndex">Конечный индекс.</param>
    public static T[] Cut<T>(this T[] array, int startIndex, int endIndex)
    {
        endIndex++;
        T[] result = new T[endIndex - startIndex];
        for (int n = startIndex, i = 0; n < endIndex; n++, i++)
            result[i] = array[n];

        return result;
    }

    /// <summary>
    /// Сохраняет одномерный массив в файл.
    /// </summary>
    /// <param name="array"/>
    /// <param name="path">Путь сохранения одномерного массива в файл.</param>
    public static void Save<T>(T[] array, string path)
    {
        using (FileStream FS = new FileStream(path, FileMode.OpenOrCreate))
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            binFormatter.Serialize(FS, array);
        }
    }

    /// <summary>
    /// Сохраняет двумерный массив в файл.
    /// </summary>
    /// <param name="array"/>
    /// <param name="path">Путь сохранения двумерного массива в файл.</param>
    public static void Save<T>(T[,] array, string path)
    {
        using (FileStream FS = new FileStream(path, FileMode.OpenOrCreate))
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            binFormatter.Serialize(FS, array);
        }
    }

    /// <summary>
    /// Сохраняет <see cref="Matrix"/> в файл как массив.
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="path">Путь сохранения <see cref="Matrix"/> в файл.</param>
    public static void Save(Matrix matrix, string path)
    {
        //using (FileStream FS = new FileStream(path, FileMode.OpenOrCreate))
        //{
        //    BinaryFormatter binFormatter = new BinaryFormatter();
        //    binFormatter.Serialize(FS, matrix);
        //}
        using (FileStream FS = new FileStream(path, FileMode.OpenOrCreate))
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            binFormatter.Serialize(FS, matrix);
        }
    }

    /// <summary>
    /// Загружает массив из файла.
    /// </summary>
    /// <param name="path">Путь загрузки массива из файла.</param>
    public static object Load(string path)
    {
        using (FileStream FS = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            return binFormatter.Deserialize(FS);
        }
    }

    /// <summary>
    /// Изменяет одномерный массив в двумерный с заданными высотой и шириной.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="height">Высота массива.</param>
    /// <param name="width">Ширина массива.</param>
    public static T[,] Reshape<T>(this T[] array, int height, int width)
    {
        T[,] result = new T[height, width];

        for (int i = 0, k = 0; i < height; i++)
            for (int j = 0; j < width; j++, k++)
                result[i, j] = array[k];

        return result;
    }

    /// <summary>
    /// Изменяет двумерный массив в одномерный.
    /// </summary>
    public static T[] Reshape<T>(this T[,] array)
    {
        int y = array.GetLength(0), x = array.GetLength(1);
        T[] result = new T[y * x];

        for (int i = 0, k = 0; i < y; i++)
            for (int j = 0; j < x; j++, k++)
                result[k] = array[i, j];

        return result;
    }

    /// <summary>
    /// Перевести одномерный массив в <see cref="string"/>.
    /// </summary>
    public static string TranslateToString<T>(this T[] array)
    {
        int x = array.Length;
        string translateToString = "[";
        for (int n = 0; n < x; n++)
            translateToString += $"{array[n]}\t";
        translateToString = translateToString.TrimEnd('\t') + "]";
        return translateToString;
    }

    /// <summary>
    /// Перевести двумерный массив в <see cref="string"/>.
    /// </summary>
    public static string TranslateToString<T>(this T[,] array)
    {
        int y = array.GetLength(0), x = array.GetLength(1);

        string translateToString = "[\n  ";
        for (int i = 0; i < y; i++)
        {
            translateToString += "[";
            for (int n = 0; n < x; n++)
                translateToString += $"{array[i, n]}\t";
            translateToString = translateToString.TrimEnd('\t') + "]\n  ";
        }
        translateToString = translateToString.TrimEnd(' ', ' ') + "]";
        return translateToString;
    }
}