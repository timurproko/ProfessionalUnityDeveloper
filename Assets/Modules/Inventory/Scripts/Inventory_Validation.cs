using System;

namespace Modules.Inventories
{
    public partial class Inventory
    {
        private static T ThrowIfNull<T>(T value) where T : class
        {
            return value ?? throw new ArgumentNullException(nameof(value));
        }

        private static T ThrowIfNullReference<T>(T value) where T : class
        {
            return value ?? throw new NullReferenceException(nameof(value));
        }

        private void ThrowIfSizeZero(int w, int h)
        {
            if (w <= 0) throw new ArgumentException("Width must be greater than zero.");
            if (h <= 0) throw new ArgumentException("Height must be greater than zero.");
        }

        private void ThrowIfPosNotFitsInventory(int x, int y)
        {
            if (x < 0 || x > _width - 1) throw new IndexOutOfRangeException("X Pos does not fit inventory indexes.");
            if (y < 0 || y > _height - 1) throw new IndexOutOfRangeException("Y Pos does not fit inventory indexes.");
        }

        private void ThrowIfMatrixNotFits(Item[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            if (matrix.GetLength(0) != _width || matrix.GetLength(1) != _height)
                throw new ArgumentException("Matrix size must match inventory size.", nameof(matrix));
        }
    }
}