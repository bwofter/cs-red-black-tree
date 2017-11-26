/*  Copyright 2017 B. Wofter
    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions 
    are met:
    1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
    3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this
    software without specific prior written permission.
    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
    IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
    CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
    OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
    DAMAGE.*/
namespace BWofter.Collections.Trees.Internals
{
    using System.Collections;
    using System.Collections.Generic;
    //TODO: document this struct. Or rewrite it. Whichever because it's pretty bad.
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
