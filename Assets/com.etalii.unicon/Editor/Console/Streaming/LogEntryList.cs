namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using EtAlii.Unicon;

    public class LogEntryList : IList
    {
        private readonly LinkedList<LogEntry> _linkedList;

        public LogEntryList(LinkedList<LogEntry> linkedList)
        {
            _linkedList = linkedList;
        }

        IEnumerator IEnumerable.GetEnumerator() => _linkedList.GetEnumerator();

        public int Add(object value)
        {
            _linkedList.AddLast((LogEntry)value);
            return _linkedList.Count;
        }

        public void Clear() => _linkedList.Clear();

        public bool Contains(object value) => _linkedList.Contains((LogEntry)value);

        public int IndexOf(object value)
        {
            var index = 0;
            foreach (var itemToCheck in _linkedList)
            {
                if (itemToCheck == value)
                {
                    return index;
                }

                index++;
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            if (index == 0)
            {
                _linkedList.AddFirst((LogEntry)value);
                return;
            }
            
            var node = _linkedList.First;
            for (var i = 0; i < index; i++)
            {
                node = node!.Next;
            }

            _linkedList.AddAfter(node!, (LogEntry)value);
        }

        public void Remove(object value) => _linkedList.Remove((LogEntry)value);

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count => _linkedList.Count;
        public bool IsSynchronized => false;
        public object SyncRoot { get; } = new ();
        public bool IsReadOnly => false;

        public LogEntry this[int index] => ((IList)this)[index] as LogEntry;

        object IList.this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return _linkedList.First();
                }
            
                var node = _linkedList.First;
                for (var i = 0; i < index; i++)
                {
                    node = node!.Next;
                }

                return node!.Value;
            }
            set
            {
                if (index == 0)
                {
                    _linkedList.First.Value = (LogEntry)value;
                }
            
                var node = _linkedList.First;
                for (var i = 0; i < index; i++)
                {
                    node = node!.Next;
                }

                node!.Value = (LogEntry)value;
            }        
        }


        public void RemoveAt(int index)
        {
            if (index == 0)
            {
                _linkedList.RemoveFirst();
                return;
            }
            
            var node = _linkedList.First;
            for (var i = 0; i < index; i++)
            {
                node = node!.Next;
            }

            _linkedList.Remove(node!);
        }

        public bool IsFixedSize => false;

    }
}
