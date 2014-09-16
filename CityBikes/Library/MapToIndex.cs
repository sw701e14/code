namespace Library
{
    /// <summary>
    /// Defines a method that maps an element of type <typeparamref name="T"/> onto an index on a grid.
    /// </summary>
    /// <typeparam name="T">The type of the element that is to be mapped.</typeparam>
    /// <param name="item">The item that should be mapped.</param>
    /// <param name="width">The width of the grid onto which <paramref name="item"/> should be mapped.</param>
    /// <param name="height">The height of the grid onto which <paramref name="item"/> should be mapped.</param>
    /// <param name="x">When the method returns, contains the zero-based x-coordinate to which <paramref name="item"/> should be mapped.</param>
    /// <param name="y">When the method returns, contains the zero-based y-coordinate to which <paramref name="item"/> should be mapped.</param>
    public delegate void MapToIndex<T>(T item, int width, int height, out int x, out int y);
}
