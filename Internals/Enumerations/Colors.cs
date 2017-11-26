namespace BWofter.Collections.Trees.Internals.Enumerations
{
    /// <summary><para>Defines colors for <see cref="RBNode{T}"/>.</para></summary>
    internal enum  Colors : byte
    {
        /// <summary><para>The value of <see cref="RBNode{T}.Color"/> if the <see cref="RBNode{T}"/> is black.</para></summary>
        Black = 0,
        /// <summary><para>The value of <see cref="RBNode{T}.Color"/> if the <see cref="RBNode{T}"/> is red.</para></summary>
        Red = 1
    }
}
