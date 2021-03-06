﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IX.Observable
{
    /// <summary>
    /// A queue that broadcasts its changes.
    /// </summary>
    /// <typeparam name="T">The type of items in the queue.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(QueueDebugView<>))]
    public class ObservableQueue<T> : ObservableCollectionBase, IQueue<T>, IEnumerable<T>, ICollection
    {
        /// <summary>
        /// The data container of the observable queue.
        /// </summary>
        protected internal Queue<T> internalContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}"/> class.
        /// </summary>
        public ObservableQueue()
            : base(null)
        {
            internalContainer = new Queue<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}"/> class.
        /// </summary>
        /// <param name="collection">A collection of items to copy from.</param>
        public ObservableQueue(IEnumerable<T> collection)
            : base(null)
        {
            internalContainer = new Queue<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the queue.</param>
        public ObservableQueue(int capacity)
            : base(null)
        {
            internalContainer = new Queue<T>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}"/> class.
        /// </summary>
        /// <param name="context">The synchronization context top use when posting observable messages.</param>
        public ObservableQueue(SynchronizationContext context)
            : base(context)
        {
            internalContainer = new Queue<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}"/> class.
        /// </summary>
        /// <param name="context">The synchronization context top use when posting observable messages.</param>
        /// <param name="collection">A collection of items to copy from.</param>
        public ObservableQueue(SynchronizationContext context, IEnumerable<T> collection)
            : base(context)
        {
            internalContainer = new Queue<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}"/> class.
        /// </summary>
        /// <param name="context">The synchronization context top use when posting observable messages.</param>
        /// <param name="capacity">The initial capacity of the queue.</param>
        public ObservableQueue(SynchronizationContext context, int capacity)
            : base(context)
        {
            internalContainer = new Queue<T>(capacity);
        }

        /// <summary>
        /// Gets the number of items currently in the queue.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return internalContainer.Count;
            }
        }

        /// <summary>
        /// Gets an enumerator for the observable queue.
        /// </summary>
        /// <returns>The queue enumerator.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return internalContainer.GetEnumerator();
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)internalContainer).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)internalContainer).SyncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index) => ((ICollection)internalContainer).CopyTo(array, index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Clears the queue of all its objects.
        /// </summary>
        public void Clear()
        {
            ClearInternal();

            AsyncPost(() =>
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionChanged();
            });
        }

        /// <summary>
        /// Clears the queue of all its objects (internal overridable procedure).
        /// </summary>
        protected virtual void ClearInternal()
        {
            var st = internalContainer;
            internalContainer = new Queue<T>();

            Task.Run(() => st.Clear());
        }

        /// <summary>
        /// Determines whether the queue contains a specific item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns><c>true</c> if the queue contains a specific item, <c>false</c> otherwise.</returns>
        public virtual bool Contains(T item) => internalContainer.Contains(item);

        /// <summary>
        /// Copies the contents of the queue to an array, starting at the specified index.
        /// </summary>
        /// <param name="array">The array to copy the items into.</param>
        /// <param name="arrayIndex">The index at which to start in the array.</param>
        public virtual void CopyTo(T[] array, int arrayIndex) => internalContainer.CopyTo(array, arrayIndex);

        /// <summary>
        /// Dequeues and removes an item from the queue.
        /// </summary>
        /// <returns>The dequeued item.</returns>
        public T Dequeue()
        {
            T item = DequeueInternal();

            AsyncPost((state) =>
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, oldItem: state, oldIndex: 0);
            }, item);

            return item;
        }

        /// <summary>
        /// Dequeues and removes an item from the queue (internal overridable procedure).
        /// </summary>
        /// <returns>The dequeued item.</returns>
        protected virtual T DequeueInternal() => internalContainer.Dequeue();

        /// <summary>
        /// Enqueues an item into the queue.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Enqueue(T item)
        {
            EnqueueInternal(item);

            var st = new Tuple<T, int>(item, Count - 1);

            AsyncPost((state) =>
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionChanged(NotifyCollectionChangedAction.Add, newItem: state.Item1, newIndex: st.Item2);
            }, st);
        }

        /// <summary>
        /// Enqueues an item into the queue (internal overridable procedure).
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        protected virtual void EnqueueInternal(T item) => internalContainer.Enqueue(item);

        /// <summary>
        /// Peeks at the topmost item in the queue without dequeueing it.
        /// </summary>
        /// <returns>The topmost item in the queue.</returns>
        public virtual T Peek() => internalContainer.Peek();

        /// <summary>
        /// Copies the items of the queue into a new array.
        /// </summary>
        /// <returns>An array of items that are contained in the queue.</returns>
        public virtual T[] ToArray() => internalContainer.ToArray();

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="ObservableQueue{T}"/>, if that number is less than 90 percent of current capacity.
        /// </summary>
        public virtual void TrimExcess() => internalContainer.TrimExcess();
    }

    internal sealed class QueueDebugView<T>
    {
        private readonly ObservableQueue<T> queue;

        public QueueDebugView(ObservableQueue<T> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            this.queue = queue;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] items = new T[queue.internalContainer.Count];
                queue.internalContainer.CopyTo(items, 0);
                return items;
            }
        }
    }
}