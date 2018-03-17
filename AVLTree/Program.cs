using System;
using System.Collections;
using System.Collections.Generic;

namespace AVLTree
{
    public class AVLTree<T> : IList<T> where T : IComparable<T>, new()
    {
        public AVLTreeNode RootNode
        {
            get; private set;
        }

        public bool IsReadOnly
        {
            get;
        }
        public int Count
        {
            get
            {
                return RootNode.Count;
            }
        }

        public void Add(T value)
        {
            if (RootNode == null)
                RootNode = new AVLTreeNode(value, this);
            else
                RootNode.Add(value);
        }

        public bool Contains(T value)
        {
            return RootNode == null ? false : RootNode.Contains(value);
        }
        public void CopyTo(T[] array, int index)
        {
            if (Count > 0)
            {
                if (array == null)
                    throw new ArgumentNullException("array");

                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException("index");

                if ((array.Length - index) < Count)
                    throw new ArgumentException();

                RootNode.CopyTo(array, index);
            }
        }
        public void Clear()
        {
            if (RootNode != null)
            {
                RootNode.Clear();
                RootNode = null;
            }
        }

        public int IndexOf(T value)
        {
            return this.RootNode != null ? this.RootNode.IndexOf(value) : -1;
        }
        public void Insert(int index, T value)
        {
            throw new InvalidOperationException();
        }

        public bool Remove(T value)
        {
            return this.RootNode == null ? false : this.RootNode.Remove(value);
        }
        public void RemoveAt(int index)
        {
            if (this.RootNode != null)
                this.RootNode.RemoveAt(index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.RootNode != null)
                foreach (var item in this.RootNode)
                    yield return item;
            else
                yield break;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                if (this.RootNode != null)
                    return this.RootNode[index];
                else
                    throw new ArgumentOutOfRangeException("index");
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public class AVLTreeNode : IList<T>
        {
            public AVLTree<T> AVLTree
            {
                get; private set;
            }
            
            public AVLTreeNode Parent
            {
                get; private set;
            }
            public AVLTreeNode LeftHand
            {
                get; private set;
            }
            public AVLTreeNode RightHand
            {
                get; private set;
            }
                        
            public bool IsReadOnly
            {
                get;
            }
            public int Count
            {
                get; private set;
            }
            public int Level
            {
                get; private set;
            }
            public T Value
            {
                get; private set;
            }

            public AVLTreeNode(T value, AVLTree<T> avlTree)
            {
                AVLTree = avlTree;
                Value = value;
                Level = 1;
                Count = 1;                
            }

            void Reconstruct(bool recursive)
            {
                Count = 1;

                int leftLevel = 0;
                int rightLevel = 0;

                if (LeftHand != null)
                {
                    leftLevel = LeftHand.Level;
                    Count += LeftHand.Count;
                }

                if (RightHand != null)
                {
                    rightLevel = RightHand.Level;
                    Count += RightHand.Count;
                }

                if (leftLevel - rightLevel > 1)
                {
                    int leftNext = LeftHand.LeftHand == null ? 0 : LeftHand.LeftHand.Level;
                    int rightNext = LeftHand.RightHand == null ? 0 : LeftHand.RightHand.Level;

                    if (leftNext >= rightNext)
                    {
                        LeftHand.Elevate();
                        Reconstruct(true);
                    }
                    else
                    {
                        AVLTreeNode pivot = LeftHand.RightHand;

                        pivot.Elevate();
                        pivot.Elevate();

                        pivot.LeftHand.Reconstruct(false);
                        pivot.RightHand.Reconstruct(true);
                    }
                }
                else if (rightLevel - leftLevel > 1)
                {
                    int leftNext = RightHand.LeftHand == null ? 0 : RightHand.LeftHand.Level;
                    int rightNext = RightHand.RightHand == null ? 0 : RightHand.RightHand.Level;                    

                    if (rightNext >= leftNext)
                    {
                        RightHand.Elevate();
                        Reconstruct(true);
                    }
                    else
                    {
                        var pivot = RightHand.LeftHand;

                        pivot.Elevate();
                        pivot.Elevate();

                        pivot.LeftHand.Reconstruct(false);
                        pivot.RightHand.Reconstruct(true);
                    }
                }
                else
                {
                    Level = Math.Max(leftLevel, rightLevel) + 1;

                    if (Parent != null && recursive)
                        Parent.Reconstruct(true);
                }
            }
            void Elevate()
            {
                AVLTreeNode rootNode = Parent;
                AVLTreeNode parentNode = rootNode.Parent;

                Parent = parentNode;

                if (Parent == null)
                    AVLTree.RootNode = this;
                else
                {
                    if (parentNode.LeftHand == rootNode)
                        parentNode.LeftHand = this;
                    else
                        parentNode.RightHand = this;
                }

                if (rootNode.LeftHand == this)
                {
                    rootNode.LeftHand = RightHand;

                    if (RightHand != null)
                        RightHand.Parent = rootNode;

                    RightHand = rootNode;
                    rootNode.Parent = this;
                }
                else
                {
                    rootNode.RightHand = LeftHand;

                    if (LeftHand != null)
                        LeftHand.Parent = rootNode;

                    LeftHand = rootNode;
                    rootNode.Parent = this;
                }
            }

            public void Add(T value)
            {
                int compare = value.CompareTo(Value);

                if (compare < 0)
                {
                    if (LeftHand == null)
                    {
                        LeftHand = new AVLTreeNode(value, AVLTree)
                        {
                            Parent = this
                        };

                        LeftHand.Reconstruct(true);
                    }
                    else
                        LeftHand.Add(value);
                }
                else
                {
                    if (RightHand == null)
                    {
                        RightHand = new AVLTreeNode(value, AVLTree)
                        {
                            Parent = this
                        };

                        RightHand.Reconstruct(true);
                    }
                    else
                        RightHand.Add(value);
                }
            }

            public bool Contains(T value)
            {
                int compare = value.CompareTo(Value);

                if (compare == 0)
                    return true;

                if (compare < 0)
                    return LeftHand == null ? false : LeftHand.Contains(value);
                else
                    return RightHand == null ? false : RightHand.Contains(value);
            }
            public void CopyTo(T[] array, int index)
            {
                if (LeftHand != null)
                {
                    LeftHand.CopyTo(array, index);
                    index += LeftHand.Count;
                }

                array[index++] = Value;

                if (RightHand != null)
                    RightHand.CopyTo(array, index);

            }
            public void Clear()
            {
                if (LeftHand != null)
                    LeftHand.Clear();

                if (RightHand != null)
                    RightHand.Clear();

                LeftHand = RightHand = null;
            }

            public bool Remove(T value)
            {
                var compare = value.CompareTo(Value);

                if (compare == 0)
                {
                    if (LeftHand == null && RightHand == null)
                    {
                        if (Parent != null)
                        {
                            if (Parent.LeftHand == this)
                                Parent.LeftHand = null;
                            else
                                Parent.RightHand = null;

                            Parent.Reconstruct(true);
                        }
                        else
                            AVLTree.RootNode = null;
                    }
                    else
                    {
                        if (LeftHand == null || RightHand == null)
                        {
                            AVLTreeNode child = LeftHand ?? RightHand;

                            if (Parent != null)
                            {
                                if (Parent.LeftHand == this)
                                    Parent.LeftHand = child;
                                else
                                    Parent.RightHand = child;

                                child.Parent = Parent;
                                child.Parent.Reconstruct(true);
                            }
                            else
                            {
                                AVLTree.RootNode = child;
                                AVLTree.RootNode.Parent = null;
                            }
                        }
                        else
                        {
                            AVLTreeNode replace = LeftHand;

                            while (replace.RightHand != null)
                                replace = replace.RightHand;

                            T tempValue = Value;
                            Value = replace.Value;
                            replace.Value = tempValue;

                            return replace.Remove(replace.Value);
                        }
                    }

                    Parent = LeftHand = RightHand = null;
                    return true;
                }

                if (compare < 0)
                    return LeftHand == null ? false : LeftHand.Remove(value);
                else
                    return RightHand == null ? false : RightHand.Remove(value);
            }
            public void RemoveAt(int index)
            {
                if (LeftHand != null)
                {
                    if (index < LeftHand.Count)
                    {
                        LeftHand.RemoveAt(index);
                        return;
                    }
                    else
                        index -= LeftHand.Count;
                }

                if (index-- == 0)
                {
                    Remove(Value);
                    return;
                }

                if (RightHand != null && index < RightHand.Count)
                {
                    RightHand.RemoveAt(index);
                    return;
                }

                throw new ArgumentOutOfRangeException("index");
            }

            public int IndexOf(T value)
            {
                int compare = value.CompareTo(Value);

                if (compare == 0)
                {
                    if (LeftHand == null)
                        return 0;
                    else
                    {
                        int temp = LeftHand.IndexOf(value);
                        return temp == -1 ? LeftHand.Count : temp;
                    }
                }

                if (compare < 0)
                {
                    if (LeftHand == null)
                        return -1;
                    else
                        return LeftHand.IndexOf(value);
                }
                else
                {
                    if (RightHand == null)
                        return -1;
                    else
                        return RightHand.IndexOf(value);
                }
            }
            public void Insert(int index, T value)
            {
                throw new InvalidOperationException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                if (LeftHand != null)
                {
                    foreach (var item in LeftHand)
                        yield return item;
                }

                yield return Value;

                if (RightHand != null)
                {
                    foreach (var item in RightHand)
                        yield return item;
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public T this[int index]
            {
                get
                {
                    if (LeftHand != null)
                    {
                        if (index < LeftHand.Count)
                            return LeftHand[index];
                        else
                            index -= LeftHand.Count;
                    }

                    if (index-- == 0)
                        return Value;

                    if (RightHand != null && index < RightHand.Count)
                        return RightHand[index];

                    throw new ArgumentOutOfRangeException("index");
                }
                set
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
