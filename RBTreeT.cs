namespace BWofter.Collections.Trees
{
    using Internals;
    using Internals.Enumerations;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    /// <summary><para>An efficient tree structure used to store large sets of related data.</para></summary>
    /// <typeparam name="T"><para>The type of the values that are stored in the <see cref="RBTree{T}"/>.</para></typeparam>
    [DebuggerDisplay("{Count}")]
    public class RBTree<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        /// <summary><para>The <see cref="RBTree{T}"/> comparer.</para></summary>
        private Comparer<T> Comparer { get; set; }
        /// <summary><para>The total number of elements in the <see cref="RBTree{T}"/>.</para></summary>
        public int Count { get; private set; }
        /// <summary><para>If the <see cref="RBTree{T}"/> is currently marked as read-only.</para></summary>
        public bool IsReadOnly { get; private set; }
        /// <summary><para>The <see cref="RBTree{T}"/> root.</para></summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal RBNode<T> Root { get; private set; }
        /// <summary><para>Initializes the <see cref="RBTree{T}"/> comparer to <see cref="Comparer{T}.Default"/>.</para></summary>
        public RBTree() : this(Comparer<T>.Default) { }
        /// <summary><para>Initializes the <see cref="RBTree{T}"/> comparer to <paramref name="comparer"/>.</para></summary>
        /// <param name="comparer"><para>The comparer to initialize the <see cref="RBTree{T}"/> with.</para></param>
        /// <exception cref="ArgumentNullException"><para>Raised of <paramref name="comparer"/> is null.</para></exception>
        public RBTree(Comparer<T> comparer) => Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        /// <summary><para>Adds a value not already in the <see cref="RBTree{T}"/> to the <see cref="RBTree{T}"/>.</para></summary>
        /// <param name="value"><para>The value to add.</para></param>
        public void Add(T value)
        {
            if (!IsReadOnly)
            {
                if (Root == null)
                {
                    ++Count;
                    Root = new RBNode<T> { Value = value, Tree = this };
                }
                else if (DoAdd(value))
                {
                    ++Count;
                    FixRoot();
                }
            }
        }
        /// <summary><para>Returns a new <see cref="RBTree{T}"/> with <see cref="IsReadOnly"/> set to true. This will clone all nodes from the original tree to the new tree.</para></summary>
        /// <returns><para>A new <see cref="RBTree{T}"/> with <see cref="IsReadOnly"/> set to true.</para></returns>
        public RBTree<T> AsReadOnly()
        {
            if (IsReadOnly)
            {
                return this;
            }
            else
            {
                RBTree<T> newTree = new RBTree<T> { Comparer = Comparer, Count = Count, IsReadOnly = true };
                newTree.Root = Root?.Clone(newTree);
                return newTree;
            }
        }
        /// <summary><para>Removes all values in the <see cref="RBTree{T}"/>.</para></summary>
        public void Clear()
        {
            if (!IsReadOnly)
            {
                Count = 0;
                Root = null;
            }
        }
        /// <summary><para>Compares <paramref name="x"/> to <paramref name="y"/> using <see cref="Comparer"/>.</para></summary>
        /// <param name="x"><para>The left value.</para></param>
        /// <param name="y"><para>The right value.</para></param>
        /// <returns><para>0 if <paramref name="x"/> and <paramref name="y"/> are the same, -1 if <paramref name="x"/> is less than <paramref name="y"/>, 1 otherwise.</para></returns>
        internal int Compare(T x, T y) => Comparer.Compare(x, y);
        /// <summary><para>Determines if the <see cref="RBTree{T}"/> contains <paramref name="value"/>.</para></summary>
        /// <param name="value"><para>The value to check.</para></param>
        /// <returns><para>True if the <see cref="RBTree{T}"/> contains <paramref name="value"/>, false otherwise.</para></returns>
        public bool Contains(T value)
        {
            if (Root != null)
            {
                RBNode<T> comparable = Root;
                while (comparable != null)
                {
                    int comparison = Compare(comparable.Value, value);
                    if (comparison == 0)
                        return true;
                    else if (comparison < 0)
                        comparable = comparable.Right;
                    else if (comparison > 0)
                        comparable = comparable.Left;
                }
            }
            return false;
        }
        /// <summary><para>Copies the values of the <see cref="RBTree{T}"/> into <paramref name="array"/>, putting them at <paramref name="index"/>.</para></summary>
        /// <param name="array"><para>The array to copy to.</para></param>
        /// <param name="index"><para>The position to copy to.</para></param>
        public void CopyTo(T[] array, int index) { foreach (T v in this) array[index++] = v; }
        /// <summary><para>Attempts to add the value to the <see cref="RBTree{T}"/>.</para></summary>
        /// <param name="value"><para>The value to add.</para></param>
        /// <returns><para>True on add, false otherwise.</para></returns>
        private bool DoAdd(T value)
        {
            RBNode<T> comparable = Root;
            while (comparable != null)
            {
                int comparison = Compare(comparable.Value, value);
                if (comparison == 0)
                {
                    return false;
                }
                else if (comparison < 0)
                {
                    if (comparable.Right == null)
                    {
                        comparable.Right = new RBNode<T> { Parent = comparable, Color = Colors.Red, Value = value, Tree = this };
                        comparable.Right.InsertRebalanceCondition1();
                        return true;
                    }
                    else
                    {
                        comparable = comparable.Right;
                    }
                }
                else
                {
                    if (comparable.Left == null)
                    {
                        comparable.Left = new RBNode<T> { Parent = comparable, Color = Colors.Red, Value = value, Tree = this };
                        comparable.Left.InsertRebalanceCondition1();
                        return true;
                    }
                    else
                    {
                        comparable = comparable.Left;
                    }
                }
            }
            return false;
        }
        /// <summary><para>Attempts to remove the value from the <see cref="RBTree{T}"/>.</para></summary>
        /// <param name="value"><para>The value to remove.</para></param>
        /// <returns><para>True on remove, false otherwise.</para></returns>
        private bool DoRemove(T value)
        {
            RBNode<T> comparable = Root;
            while (comparable != null)
            {
                int comparison = Compare(comparable.Value, value);
                if (comparison == 0)
                {
                    bool noLeft = comparable.Left == null, noRight = comparable.Right == null;
                    if (noLeft && noRight)
                        comparable.Replace();
                    else if (noRight)
                        comparable = comparable.Replace(comparable.Left);
                    else if (noLeft)
                        comparable = comparable.Replace(comparable.Right);
                    else
                    {
                        RBNode<T> largest = comparable;
                        for (; largest.Right != null; largest = largest.Right) ;
                        comparable.Value = largest.Value;
                        comparable.Color = largest.Color;
                        largest.Replace();
                    }
                    comparable?.DeleteRebalanceCondition1();
                    return true;
                }
                else if (comparison < 0)
                {
                    if (comparable.Right == null)
                        return false;
                    else
                        comparable = comparable.Right;
                }
                else
                {
                    if (comparable.Left == null)
                        return false;
                    else
                        comparable = comparable.Left;
                }
            }
            return false;
        }
        /// <summary><para>Determines the new root of the <see cref="RBTree{T}"/>. If <see cref="Count"/> is 0, then the root is set to null. Otherwise, root is set to the value returned by find root on the root node.</para></summary>
        private void FixRoot()
        {
            if (Count == 0) Root = null;
            else for (; Root.Parent != null; Root = Root.Parent) ;
        }
        /// <summary><para>Gets a new enumerator for the <see cref="RBTree{T}"/>.</para></summary>
        /// <returns><para>A new enumerator for the <see cref="RBTree{T}"/>.</para></returns>
        public IEnumerator<T> GetEnumerator() => new RBEnumerator<T> { Tree = this };
        /// <summary><para>Calls the <see cref="RBTree{T}.GetEnumerator"/> method.</para></summary>
        /// <returns><para>A new enumerator for the <see cref="RBTree{T}"/>.</para></returns>
        /// <seealso cref="RBTree{T}.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary><para>Removes the value <paramref name="value"/> from the <see cref="RBTree{T}"/>, returning whether or not the removal was successful.</para></summary>
        /// <param name="value"><para>The value to remove.</para></param>
        /// <returns><para>True if the <see cref="RBTree{T}"/> removed <paramref name="value"/>, false otherwise.</para></returns>
        public bool Remove(T value)
        {
            if (!IsReadOnly && Root != null && DoRemove(value))
            {
                --Count;
                FixRoot();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}