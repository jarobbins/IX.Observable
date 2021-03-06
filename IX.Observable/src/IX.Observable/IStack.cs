﻿namespace IX.Observable
{
    /// <summary>
    /// A contract for a stack.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    public interface IStack<T>
    {
        /// <summary>
        /// The number of elements in the observable stack.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Clears the observable stack.
        /// </summary>
        void Clear();

        /// <summary>
        /// Checks whether or not a certain item is in the stack.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns><c>true</c> if the item was found, <c>false</c> otherwise.</returns>
        bool Contains(T item);

        /// <summary>
        /// Peeks in the stack to view the topmost item, without removing it.
        /// </summary>
        /// <returns>The topmost element in the stack, if any.</returns>
        T Peek();

        /// <summary>
        /// Pops the topmost element from the stack, removing it.
        /// </summary>
        /// <returns>The topmost element in the stack, if any.</returns>
        T Pop();

        /// <summary>
        /// Pushes an element to the top of the stack.
        /// </summary>
        /// <param name="item">The item to push.</param>
        void Push(T item);

        /// <summary>
        /// Copies all elements of the stack to a new array.
        /// </summary>
        /// <returns>An array containing all items in the stack.</returns>
        T[] ToArray();

        /// <summary>
        /// Sets the capacity to the actual number of elements in the stack if that number is less than 90 percent of current capacity.
        /// </summary>
        void TrimExcess();
    }
}