using System;
using System.Collections;
using System.Collections.Generic;

namespace Matrices
{
    public class Matrix : IEnumerable
    {
        #region fields
        private double[,] _matrix;
        #endregion

        #region properties
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        #endregion

        #region Constructors
        //Used for initializing a matrix with specified size.
        public Matrix(int rows, int columns)
        {
            //Assigning the rows and colums for the array
            //Either rows or columns cannot be less than 1
            if (rows <= 0 | columns <= 0)
                throw new ArgumentOutOfRangeException("Either rows or columns cannot be less than 1.");

            Rows = rows;
            Columns = columns;

            //Initializing the matrix (the array)
            _matrix = new double[Rows, Columns];
        }

        //Used for initializing a new matrix using an already existed one.
        public Matrix(Matrix matrix)
        {
            if (matrix is null)
                throw new ArgumentNullException();

            Rows = matrix.Rows;
            Columns = matrix.Columns;
            _matrix = new double[Rows, Columns];
            Array.Copy(matrix._matrix, _matrix, matrix._matrix.Length);

        }

        public Matrix(int rows, int columns, double[,] matrix)
        {
            //Assigning the rows and colums for the array
            //Either rows or columns cannot be less than 1
            if (rows <= 0 | columns <= 0)
                throw new ArgumentOutOfRangeException("Either rows or columns cannot be less than 1.");

            Rows = rows;
            Columns = columns;

            if (matrix is null)
                throw new ArgumentNullException();
            if (matrix.Length != Rows * Columns)
                throw new ArgumentException("The array length is greate than what it should be given the number of rows and columns");

            _matrix = matrix;
        }
        #endregion

        #region indexers
        public double this[int row, int column]
        {
            get { return Convert.ToDouble(_matrix.GetValue(row, column)); }
            set { _matrix.SetValue(value, row, column); }
        }
        #endregion

        #region static funtions
        private static void Add_SubtractConfirmation(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1 is null | matrix2 is null)
                throw new ArgumentNullException("Any of the passed matrices cannot be null");

            if (matrix1.Rows != matrix2.Rows |
                matrix1.Columns != matrix2.Columns)
                throw new ArgumentException("The given matrices must have the same size in order to add them");
        }
        public static Matrix Add(Matrix matrix1, Matrix matrix2)
        {
            Add_SubtractConfirmation(matrix1, matrix2);
            Matrix matrix = new Matrix(matrix1.Rows, matrix1.Columns);
            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix1.Columns; j++)
                {
                    matrix[i, j] = matrix1[i, j] + matrix2[i, j];
                }
            }

            return matrix;
        }
        public static Matrix Subtract(Matrix matrix1, Matrix matrix2)
        {
            Add_SubtractConfirmation(matrix1, matrix2);
            Matrix matrix = new Matrix(matrix1.Rows, matrix1.Columns);
            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix1.Columns; j++)
                {
                    matrix[i, j] = matrix1[i, j] - matrix2[i, j];
                }
            }

            return matrix;
        }

        public static Matrix MatrixMultiply(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1 is null | matrix2 is null)
                throw new ArgumentNullException("Any of the passed matrices cannot be null");

            if (matrix1.Columns != matrix2.Rows)
                throw new ArgumentException("In order to muliply the matrices, the number of columns " +
                    "for the first matrix must equal the number of rows for the second matrix");

            var product = new Matrix(matrix1.Rows, matrix2.Columns);

            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix2.Columns; j++)
                {
                    var row = matrix1._matrix.GetRow(i);
                    var column = matrix2._matrix.GetColumn(j);
                    for (int e = 0; e < row.Length; e++)
                    {
                        product[i, j] += row[e] * column[e];
                    }
                }
            }

            return product;
        }

        public static Matrix ScalarMultiply(double scalar, Matrix matrix)
        {
            Matrix _matrix = Copy(matrix);

            for (int i = 0; i < _matrix.Rows; i++)
            {
                for (int j = 0; j < _matrix.Columns; j++)
                {
                    _matrix[i, j] = scalar * _matrix[i, j];
                }
            }
            return _matrix;
        }

        public static void Copy(Matrix sourceMatrix, out Matrix destinationMatrix) =>
            destinationMatrix = new Matrix(sourceMatrix);
        public static Matrix Copy(Matrix sourceMatrix) =>
            new Matrix(sourceMatrix);

        public static double Determinant(Matrix matrix)
        {
            if (matrix is null)
                throw new ArgumentNullException();

            double det = 0;

            if (IsSquareMatrix(matrix))
            {
                if (matrix.Rows == 2 && matrix.Columns == 2)
                {
                    return _2By2Determinant(matrix);
                }
                else
                {
                    //recursive definition
                    for (int j = 0; j < matrix.Columns; j++)
                    {
                        det += Math.Pow(-1, j) * matrix[0, j] * Determinant(SubMatrix(matrix, 0, j));
                    }
                }
            }
            else
                throw new ArgumentException("Determinants cannot be defined for non-square matrices");

            return det;
        }

        #region submatrix
        public static Matrix SubMatrix(Matrix matrix, int row, int column)
        {
            if (!IsSquareMatrix(matrix) | (matrix.Rows == 2 & matrix.Columns == 2))
                throw new ArgumentException("The given matrix should be a square matrix that is higher than 2x2");

            List<double> allowedElements = new List<double>();
            var subMatrix = new Matrix(matrix.Rows - 1, matrix.Columns - 1);


            var indexes = GetDimensionIndex(matrix);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    var normalIndex = GetNormalIndex(matrix, i, j);

                    if (indexes[normalIndex][0] != row & indexes[normalIndex][1] != column)
                    {
                        allowedElements.Add(matrix[i, j]);
                    }
                    else
                        continue;
                }
            }
            Populate(ref subMatrix, allowedElements);

            return subMatrix;
        }

        private static int GetNormalIndex(Matrix matrix, int row, int column)
        {
            if (matrix is null)
                throw new ArgumentNullException();

            if (row < 0 | column < 0)
                throw new ArgumentOutOfRangeException();

            return row * matrix.Rows + column;
        }
        private static void Populate(ref Matrix matrix, List<double> elements)
        {
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    int index = GetNormalIndex(matrix, i, j);
                    matrix[i, j] = elements[index];

                }
            }
        }
        public static List<char[]> GetDimensionIndex(Matrix matrix)
        {
            List<char[]> indexes = new List<char[]>();
            for (int i = 0; i < matrix._matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix._matrix.GetLength(1); j++)
                {
                    indexes.Add(new char[] { (char)i, (char)j });
                }
            }
            return indexes;
        }
        public static char[] GetDimensionIndex(Matrix matrix, int index)
        {
            return GetDimensionIndex(matrix)[index];
        }
        #endregion

        private static double _2By2Determinant(Matrix matrix) =>
            (matrix[0, 0] * matrix[1, 1]) - (matrix[0, 1] * matrix[1, 0]);

        public static Matrix Transpose(Matrix matrix)
        {
            if (matrix is null)
                throw new ArgumentNullException();

            var transpose = new Matrix(matrix.Rows, matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    transpose[j, i] = matrix[i, j];
                }
            }

            return transpose;
        }

        public static Matrix Inverse(Matrix matrix)
        {
            if (matrix is null)
                throw new ArgumentNullException();

            //We are finding the inverse of a square matrix using the Adjugate Formula

            if (!IsInversable(matrix))
                throw new ArgumentException("The matrix must be square and it's determinant cannot equal 0");

            Matrix adjugateMatrix = new Matrix(matrix.Rows, matrix.Columns);

            var det = Determinant(matrix);

            //if 2x2 matrix
            if (matrix.Rows == 2)
            {
                adjugateMatrix._matrix = new double[,] { { matrix[1, 1], -matrix[0, 1] }, { -matrix[1, 0], matrix[0, 0] } };
            }
            //otherwise
            else
            {
                adjugateMatrix = Transpose(CofactorsMatrix(matrix));
            }


            return ScalarMultiply((1 / det), adjugateMatrix);
        }

        public static Matrix CofactorsMatrix(Matrix matrix)
        {
            if (matrix is null)
                throw new ArgumentNullException();

            if (!IsSquareMatrix(matrix))
                throw new ArgumentException("Given matrix must be a square matrix");

            if (matrix.Rows == 2)
                throw new ArgumentException("Given matrix must be 3x3 or more");

            Matrix cofactorMat = new Matrix(matrix.Rows, matrix.Columns);

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    cofactorMat[i, j] = Math.Pow(-1, i + j) * Determinant(SubMatrix(matrix, i, j));
                }
            }

            return cofactorMat;
        }

        public static bool IsInversable(Matrix matrix) =>
            matrix.Determinant() != 0;
        public static bool IsSquareMatrix(Matrix matrix) =>
            matrix.Rows == matrix.Columns;

        #endregion

        #region instance functions
        public Matrix Add(Matrix matrix) => Add(this, matrix);
        public Matrix Subtract(Matrix matrix) => Subtract(this, matrix);
        public Matrix MatrixMultiply(Matrix matrix2) => MatrixMultiply(this, matrix2);
        public Matrix ScalarMultiply(double scalar) => ScalarMultiply(scalar, this);
        public Matrix Copy() => Copy(this);
        public double Determinant() => Determinant(this);
        public Matrix Transpose() => Transpose(this);
        public Matrix Inverse() => Inverse(this);
        public Matrix CofactorsMatrix() => CofactorsMatrix(this);
        public bool IsInversable() => IsInversable(this);
        public bool IsSquareMatrix() => IsSquareMatrix(this);
        #endregion

        #region operators
        public static Matrix operator +(Matrix matrix1, Matrix matrix2) => Add(matrix1, matrix2);
        public static Matrix operator -(Matrix matrix1, Matrix matrix2) => Subtract(matrix1, matrix2);
        public static Matrix operator *(Matrix matrix1, Matrix matrix2) => MatrixMultiply(matrix1, matrix2);
        public static Matrix operator *(double scalar, Matrix matrix2) => ScalarMultiply(scalar, matrix2);
        #endregion

        #region misc
        public override string ToString()
        {
            var matrix = "";

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    matrix += _matrix[i, j] + " ";
                }
                matrix += "\n";
            }
            return matrix;
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            _matrix.GetEnumerator();
        #endregion
    }
}
