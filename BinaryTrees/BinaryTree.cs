using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T>
        where T : IComparable
    {
        BinaryNode<T> rootNode = null;
        public void Add(T e)
        {
            if (rootNode == null)
                rootNode = new BinaryNode<T>(e, 0);
            else
                Add(rootNode, e, 1);
        }

        public void Add(BinaryNode<T> parent, T e, int depth)
        {
            bool isRight = e.CompareTo(parent.Value) >= 0;
            if (isRight && parent.Right == null)
                parent.Right = new BinaryNode<T>(e, depth);
            else if (!isRight && parent.Left == null)
                parent.Left = new BinaryNode<T>(e, depth);
            else
                Add(isRight ? parent.Right : parent.Left, e, depth+1);
        }

        public bool Contains(T item) =>
            rootNode != null && Search(rootNode, item) != null;
        

        public static BinaryNode<T> Search(BinaryNode<T> root, T element)
        {
            if (root == null) return null;
            if (element.Equals(root.Value)) return root;
            return element.CompareTo(root.Value) >= 0
                ? Search(root.Right, element)
                : Search(root.Left, element);
        }

        public IEnumerator GetEnumerator()
        {
            if (rootNode != null)
                foreach (var item in rootNode)
                    yield return item;
                
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (rootNode != null)
                foreach (var item in rootNode)
                    yield return item;
        }

        public T this[int i]
        {
            get
            {
                var currentNode = rootNode;
                while (true)
                {
                    var leftSize = currentNode.Left?.Depth ?? 0;
                    if (i == leftSize)
                        return currentNode.Value;
                    if (i < leftSize)
                        currentNode = currentNode.Left;
                    else
                    {
                        currentNode = currentNode.Right;
                        i -= 1 + leftSize;
                    }
                }
            }//доделать
        }
    }

    public class BinaryNode<T> : IEnumerable<T>
        where T : IComparable
    {
        public T Value;
        public BinaryNode<T> Left, Right;
        public int Depth;

        public BinaryNode(T val, int depth)
        {
            Value = val;
            Depth = depth;
        }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable<T> collection = GetCollection(this);
            foreach (var item in collection)
                yield return item;  
        }

        private IEnumerable<T> GetCollection(BinaryNode<T> parent)
        {
            var list = new List<T>();
            if (parent == null)
                return new List<T>(0);
            list.Add(parent.Left.Value);
            list.AddRange(GetCollection(parent.Left));
            list.Add(parent.Right.Value);
            list.AddRange(GetCollection(parent.Right));
            return list;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
             return GetEnumerator();
        }
    }
}