﻿using System;
using System.Collections.Concurrent;

namespace Maple.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Maple.Core.ObservableObject" />
    public class BusyStack : ObservableObject
    {
        private ConcurrentBag<BusyToken> _items;
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        protected ConcurrentBag<BusyToken> Items
        {
            get { return _items; }
            set { SetValue(ref _items, value, InvokeOnChanged); }
        }

        private Action<bool> _onChanged;
        /// <summary>
        /// Gets or sets the on changed.
        /// </summary>
        /// <value>
        /// The on changed.
        /// </value>
        public Action<bool> OnChanged
        {
            get { return _onChanged; }
            set { SetValue(ref _onChanged, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusyStack"/> class.
        /// </summary>
        public BusyStack()
        {
            Items = new ConcurrentBag<BusyToken>();
        }

        /// <summary>
        /// Tries to take an item from the stack and returns true if that action was successful
        /// </summary>
        /// <returns></returns>
        public bool Pull()
        {
            var result = Items.TryTake(out BusyToken token);

            if (result)
                InvokeOnChanged();

            return result;
        }

        /// <summary>
        /// Adds a new <see cref="BusyToken" /> to the Stack
        /// </summary>
        /// <param name="token">The token.</param>
        public void Push(BusyToken token)
        {
            Items.Add(token);

            InvokeOnChanged();
        }

        /// <summary>
        /// Determines whether this instance has items.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has items; otherwise, <c>false</c>.
        /// </returns>
        public bool HasItems()
        {
            return Items?.TryPeek(out BusyToken token) ?? false;
        }

        /// <summary>
        /// Returns a new <see cref="BusyToken" /> thats associated with <see cref="this" /> instance of a <see cref="BusyStack" />
        /// </summary>
        /// <returns>
        /// a new <see cref="BusyToken" />
        /// </returns>
        public BusyToken GetToken()
        {
            return new BusyToken(this);
        }

        private void InvokeOnChanged()
        {
            DispatcherFactory.Invoke(OnChanged, HasItems());
        }
    }
}
