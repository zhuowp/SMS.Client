using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace SMS.Client.Controls
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> originCollection, IEnumerable<T> addCollection)
        {
            foreach (T addItem in addCollection)
            {
                originCollection.Add(addItem);
            }
        }

        public static void OnNotifyCollectionChanged<T>(this NotifyCollectionChangedEventArgs e,
            Action<T> AddItem, Action<T> RemoveItem, Action<T, T> ReplaceItem, Action ResetItems)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (!(e.NewItems is IList addItems))
                    {
                        return;
                    }

                    foreach (T newItem in addItems)
                    {
                        AddItem?.Invoke(newItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (!(e.OldItems is IList removeItems))
                    {
                        return;
                    }

                    foreach (T oldItem in removeItems)
                    {
                        RemoveItem?.Invoke(oldItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetItems.Invoke();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (!(e.NewItems is IList newItems) || !(e.OldItems is IList oldItems))
                    {
                        return;
                    }

                    for (int i = 0; i < newItems.Count; i++)
                    {
                        ReplaceItem?.Invoke((T)newItems[i], (T)oldItems[i]);
                    }
                    break;
            }
        }

    }
}
