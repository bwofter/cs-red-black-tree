namespace BWofter.Collections.Trees.Internals
{
    using System.Collections;
    using System.Collections.Generic;
    //TODO: document this class. Or rewrite it. Whichever because it's pretty bad.
    internal struct RBEnumerator<T> : IEnumerator<T>
    {
        private RBNode<T> CurrentNode { get; set; }
        internal RBTree<T> Tree { get; set; }
        private T PreviousValue { get; set; }
        public T Current => CurrentNode.Value;
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            CurrentNode = null;
            Tree = null;
        }
        public bool MoveNext()
        {
            if (CurrentNode == null)
            {
                CurrentNode = Tree?.Root;
                for (; CurrentNode?.Left != null; CurrentNode = CurrentNode.Left) ;
            }
            else
            {
                PreviousValue = CurrentNode.Value;
                if (CurrentNode.Right == null)
                {
                    while (CurrentNode != null && Tree.Compare(CurrentNode.Value, PreviousValue) < 1)
                    {
                        CurrentNode = CurrentNode.Parent;
                    }
                }
                else
                {
                    CurrentNode = CurrentNode.Right;
                    for (; CurrentNode?.Left != null; CurrentNode = CurrentNode.Left) ;
                }
            }
            return CurrentNode != null;
        }
        public void Reset() => CurrentNode = null;
    }
}
