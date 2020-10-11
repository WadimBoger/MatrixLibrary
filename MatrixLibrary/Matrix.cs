using System;
using System.Threading.Tasks;

/// <summary>
/// Предоставляет класс матрицы
/// </summary>
[Serializable]
public class Matrix
{
    private double[,] data;

    /// <summary>
    /// Высота матрицы.
    /// </summary>
    public int Height { get => data.GetLength(0); }

    /// <summary>
    /// Ширина матрицы.
    /// </summary>
    public int Width { get => data.GetLength(1); }

    /// <summary>
    /// Минимальное значение в <see cref="Matrix"/>.
    /// </summary>
    public double Min
    {
        get
        {
            double min = data[0, 0];
            ProcessFunctionOverData((i, j) => { if (data[i, j] < min) min = data[i, j]; });
            return min;
        }
    }

    /// <summary>
    /// Максимальное значение в <see cref="Matrix"/>.
    /// </summary>
    public double Max
    {
        get
        {
            double max = data[0, 0];
            ProcessFunctionOverData((i, j) => { if (data[i, j] > max) max = data[i, j]; });
            return max;
        }
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Matrix"/>.
    /// </summary>
    /// <param name="height">Высота матрицы</param>
    /// <param name="width">Ширина матрицы</param>
    public Matrix(int height, int width)
    {
        data = new double[height, width];
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Matrix"/> из готового двумерного массива.
    /// </summary>
    /// <param name="array">Двумерный массив.</param>
    public Matrix(double[,] array)
    {
        data = array;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Matrix"/> из готового одномерного массива.
    /// </summary>
    /// <param name="array">Одномерный массив.</param>
    public Matrix(double[] array)
    {
        data = new double[1, array.Length];

        for (int i = 0; i < Width; i++)
            data[0, i] = array[i];
    }

    /// <summary>Позволяет обращаться к <see cref="Matrix"/> как к массиву.</summary>
    public double this[int y, int x]
    {
        get => data[y, x];
        set => data[y, x] = value;
    }

    ///<summary>Умножает <see cref="Matrix"/> на значение.</summary>
    public static Matrix operator *(Matrix matrix, double value)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] * value);
        return result;
    }
    ///<summary>Умножает значение на <see cref="Matrix"/>.</summary>
    public static Matrix operator *(double value, Matrix matrix)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = value * matrix[i, j]);
        return result;
    }
    ///<summary>Умножает <see cref="Matrix"/> на <see cref="Matrix"/>.</summary>
    public static Matrix operator *(Matrix matrix, Matrix matrix2)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] * matrix2[i, j]);
        return result;
    }

    ///<summary>Суммирует <see cref="Matrix"/> и значение.</summary>
    public static Matrix operator +(Matrix matrix, double value)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] + value);
        return result;
    }
    ///<summary>Суммирует значение и <see cref="Matrix"/>.</summary>
    public static Matrix operator +(double value, Matrix matrix)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = value + matrix[i, j]);
        return result;
    }
    ///<summary>Суммирует <see cref="Matrix"/>.</summary>
    public static Matrix operator +(Matrix matrix, Matrix matrix2)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] + matrix2[i, j]);
        return result;
    }

    ///<summary>Вычитает из <see cref="Matrix"/> значение.</summary>
    public static Matrix operator -(Matrix matrix, double value)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] - value);
        return result;
    }
    ///<summary>Вычитает из значения <see cref="Matrix"/>.</summary>
    public static Matrix operator -(double value, Matrix matrix)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = value - matrix[i, j]);
        return result;
    }
    ///<summary>Вычитает из матрицы <see cref="Matrix"/> <see cref="Matrix"/>.</summary>
    public static Matrix operator -(Matrix matrix, Matrix matrix2)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] - matrix2[i, j]);
        return result;
    }

    ///<summary>Делит <see cref="Matrix"/> на значение.</summary>
    public static Matrix operator /(Matrix matrix, double value)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] / value);
        return result;
    }
    ///<summary>Делит значение на <see cref="Matrix"/>.</summary>
    public static Matrix operator /(double value, Matrix matrix)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = value / matrix[i, j]);
        return result;
    }
    ///<summary>Делит <see cref="Matrix"/> на <see cref="Matrix"/>.</summary>
    public static Matrix operator /(Matrix matrix, Matrix matrix2)
    {
        var result = new Matrix(matrix.Height, matrix.Width);
        result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] / matrix2[i, j]);
        return result;
    }

    private void ProcessFunctionOverData(Action<int, int> func)
    {
        Parallel.For(0, Height, i =>
        {
            for (var j = 0; j < Width; j++)
                func(i, j);
        });
    }

    /// <summary>
    /// Транспонирование <see cref="Matrix"/>.
    /// </summary>
    public Matrix Transpose()
    {
        var result = new Matrix(Width, Height);
        ProcessFunctionOverData((i, j) => { result[j, i] = data[i, j]; });
        return result;
    }

    /// <summary>
    /// Возвращает двумерный массив <see cref="Matrix"/>.
    /// </summary>
    public double[,] ReturnArray() => data;

    /// <summary>
    /// Отображает <see cref="Matrix"/> в стороковом формате.
    /// </summary>
    public override string ToString()
    {
        string matrixToString = "[\n  ";
        for (int i = 0; i < Height; i++)
        {
            matrixToString += "[";
            for (int n = 0; n < Width; n++)
                matrixToString += $"{data[i, n]}\t";
            matrixToString = matrixToString.TrimEnd('\t') + "]\n  ";
        }
        matrixToString = matrixToString.TrimEnd(' ', ' ') + "]";
        return matrixToString;
    }
}