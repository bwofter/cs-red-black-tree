namespace BWofter.Collections.Trees.Internals
{
    using Enumerations;
    using System;
    /// <summary><para>A class used to represent a node in a <see cref="RBTree{T}"/>.</para></summary>
    /// <typeparam name="T"><para>The type of the stored value.</para></typeparam>
    /// <seealso cref="RBTree{T}"/>
    [Serializable]
    internal sealed class RBNode<T>
    {
        /// <summary><para>The color of the current node.</para></summary>
        /// <seealso cref="Colors"/>
        public Colors Color { get; set; } = Colors.Black;
        /// <summary><para>The found grandparent of the current node, or null.</para></summary>
        public RBNode<T> GrandParent => Parent?.Parent;
        /// <summary><para>The found left child of the current node, or null.</para></summary>
        public RBNode<T> Left { get; set; }
        /// <summary><para>The found parent of the current node, or null.</para></summary>
        public RBNode<T> Parent { get; set; }
        /// <summary><para>The found right child of the current node, or null.</para></summary>
        public RBNode<T> Right { get; set; }
        /// <summary><para>The found sibling of the current node, or null.</para></summary>
        public RBNode<T> Sibling => Parent?.Left == this ? Parent?.Right : Parent?.Left;
        /// <summary><para>The <see cref="RBTree{T}"/> the <see cref="RBNode{T}"/> belongs to.</para></summary>
        public RBTree<T> Tree { get; set; }
        /// <summary><para>The found uncle of the current node, or null.</para></summary>
        public RBNode<T> Uncle => Parent?.Sibling;
        /// <summary><para>The value of the current node.</para></summary>
        public T Value { get; set; }
        /// <summary><para>Clones the current <see cref="RBNode{T}"/> and all of its children, returning the cloned result.</para></summary>
        /// <param name="tree"><para>An optional new tree for the <see cref="RBNode{T}"/>.</para></param>
        /// <param name="parent"><para>An optional new parent for the <see cref="RBNode{T}"/>.</para></param>
        /// <returns><para>The cloned <see cref="RBNode{T}"/>.</para></returns>
        public RBNode<T> Clone(RBTree<T> tree = null, RBNode<T> parent = null) => new RBNode<T> { Color = Color, Parent = parent, Tree = tree ?? Tree, Value = Value, Left = Left, Right = Right }.CloneChildren();
        /// <summary><para>Clones all children <see cref="RBNode{T}"/> of the current <see cref="RBNode{T}"/>.</para></summary>
        /// <returns><para>The current <see cref="RBNode{T}"/>.</para></returns>
        private RBNode<T> CloneChildren()
        {
            Left = Left?.Clone(Tree, this);
            Right = Right?.Clone(Tree, this);
            return this;
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        public void DeleteRebalanceCondition1()
        {
            if (Color == Colors.Black && Parent != null)
                DeleteRebalanceCondition2();
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        private void DeleteRebalanceCondition2()
        {
            if (Sibling?.Color == Colors.Red)
            {
                Parent.Color = Colors.Red;
                Sibling.Color = Colors.Black;
                RotateOnParent();
            }
            DeleteRebalanceCondition3();
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        private void DeleteRebalanceCondition3()
        {
            if (Parent.Color == Colors.Black &&
                Sibling.Color == Colors.Black &&
                Sibling.Left?.Color == Colors.Black &&
                Sibling.Right?.Color == Colors.Black)
            {
                Sibling.Color = Colors.Red;
                Parent.DeleteRebalanceCondition1();
            }
            else
            {
                DeleteRebalanceCondition4();
            }
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        private void DeleteRebalanceCondition4()
        {
            if (Parent.Color == Colors.Red &&
                Sibling.Color == Colors.Black &&
                Sibling.Left?.Color == Colors.Black &&
                Sibling.Right?.Color == Colors.Black)
            {
                Parent.Color = Colors.Black;
                Sibling.Color = Colors.Red;
            }
            else
            {
                DeleteRebalanceCondition5();
            }
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        private void DeleteRebalanceCondition5()
        {
            if (Sibling.Color == Colors.Black)
            {
                if (this == Parent.Left &&
                    Sibling.Right?.Color == Colors.Black &&
                    Sibling.Left?.Color == Colors.Red)
                {
                    Sibling.Color = Colors.Red;
                    Sibling.Left.Color = Colors.Black;
                    Sibling.RotateRight();
                }
                else if (this == Parent.Right &&
                    Sibling.Left?.Color == Colors.Black &&
                    Sibling.Right?.Color == Colors.Red)
                {
                    Sibling.Color = Colors.Red;
                    Sibling.Right.Color = Colors.Black;
                    Sibling.RotateLeft();
                }
            }
            DeleteRebalanceCondition6();
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        private void DeleteRebalanceCondition6()
        {
            Sibling.Color = Parent.Color;
            Parent.Color = Colors.Black;
            if (this == Parent.Left)
            {
                Sibling.Right.Color = Colors.Black;
                Parent.RotateLeft();
            }
            else
            {
                Sibling.Left.Color = Colors.Black;
                Parent.RotateRight();
            }
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        public void InsertRebalanceCondition1()
        {
            if (Parent == null)
                Color = Colors.Black;
            else if (Uncle?.Color == Colors.Red)
                InsertRebalanceCondition2();
            else if (Parent.Color != Colors.Black)
                InsertRebalanceCondition3Step1();
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        private void InsertRebalanceCondition2()
        {
            Parent.Color = Colors.Black;
            Uncle.Color = Colors.Black;
            GrandParent.Color = Colors.Red;
            GrandParent.InsertRebalanceCondition1();
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        private void InsertRebalanceCondition3Step1()
        {
            RBNode<T> nextNode = this;
            if (this == GrandParent?.Left?.Right)
            {
                Parent.RotateLeft();
                nextNode = Left;
            }
            else if (this == GrandParent?.Right?.Left)
            {
                Parent.RotateRight();
                nextNode = Right;
            }
            nextNode.InsertRebalanceCondition3Step2();
        }
        /// <summary><para>Performs a rebalance of the <see cref="RBTree{T}"/> starting with the current node.</para></summary>
        private void InsertRebalanceCondition3Step2()
        {
            RBNode<T> grandParent = GrandParent;
            if (this == Parent.Left)
                grandParent.RotateRight();
            else if (this == Parent.Right)
                grandParent.RotateLeft();
            Parent.Color = Colors.Black;
            grandParent.Color = Colors.Red;
        }
        /// <summary><para>Replaces the current <see cref="RBNode{T}"/> with <paramref name="replacementNode"/>.</para></summary>
        /// <param name="replacementNode"><para>The <see cref="RBNode{T}"/> or null to replace the current <see cref="RBNode{T}"/> with.</para></param>
        /// <returns><para>The <see cref="RBNode{T}"/> <paramref name="replacementNode"/>.</para></returns>
        public RBNode<T> Replace(RBNode<T> replacementNode = null)
        {
            if (Parent != null)
                if (Parent.Left == this) Parent.Left = replacementNode;
                else Parent.Right = replacementNode;
            if (replacementNode != null) replacementNode.Parent = Parent;
            return replacementNode;
        }
        /// <summary><para>Rotates the tree left with the current <see cref="RBNode{T}"/> as the pivot point.</para></summary>
        public void RotateLeft()
        {
            if (Right != null)
            {
                RBNode<T> newParent = Right;
                Right = newParent.Left;
                newParent.Left = this;
                RotateParents(newParent);
                if (Right != null)
                    Right.Parent = this;
            }
        }
        /// <summary><para>Rotates the tree left or right with the current <see cref="RBNode{T}"/>'s <see cref="Parent"/> as the pivot point.</para></summary>
        private void RotateOnParent()
        {
            if (this == Parent?.Left)
                Parent?.RotateLeft();
            else
                Parent?.RotateRight();
        }
        /// <summary><para>Swaps the current <see cref="RBNode{T}"/>'s <see cref="Parent"/> with <paramref name="newParent"/>.</para></summary>
        /// <param name="newParent"><para>The replacement for the current <see cref="RBNode{T}"/>'s <see cref="Parent"/>.</para></param>
        private void RotateParents(RBNode<T> newParent)
        {
            newParent.Parent = Parent;
            Parent = newParent;
            if (this == newParent.Parent?.Left)
                newParent.Parent.Left = newParent;
            else if (this == newParent.Parent?.Right)
                newParent.Parent.Right = newParent;
        }
        /// <summary><para>Rotates the tree right with the current <see cref="RBNode{T}"/> as the pivot point.</para></summary>
        public void RotateRight()
        {
            if (Left != null)
            {
                RBNode<T> newParent = Left;
                Left = newParent.Right;
                newParent.Right = this;
                RotateParents(newParent);
                if (Left != null)
                    Left.Parent = this;
            }
        }
        /// <summary><para>Returns the value of <see cref="Value"/>'s <see cref="System.Object.ToString"/> method.</para></summary>
        /// <returns><para>The string representation of <see cref="Value"/>.</para></returns>
        public override string ToString() => Value?.ToString();
    }
}