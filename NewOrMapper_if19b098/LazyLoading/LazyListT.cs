using NewOrMapper_if19b098.Interfaces;
using NewOrMapper_if19b098.Models;
using System;
using System.Collections;
using System.Collections.Generic;



namespace NewOrMapper_if19b098.LazyLoading
{
    /// <summary>This class implements a lazy loading wrapper for framework object lists.</summary>
    /// <typeparam name="T">Type.</typeparam>
    public class LazyList<T>: IList<T>, ILazy
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected members                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>List values.</summary>
        protected List<T> _InternalItems = null;

        /// <summary>SQL.</summary>
        protected string _Sql;

        /// <summary>SQL parameters.</summary>
        protected ICollection<Tuple<string, object>> _Params;



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // constructors                                                                                                     //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="sql">SQL.</param>
        /// <param name="parameters">SQL parameters.</param>
        internal protected LazyList(string sql, ICollection<Tuple<string, object>> parameters)
        {
            _Sql = sql;
            _Params = parameters;
        }


        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="obj">Object.</param>
        /// <param name="fieldName">FIeld name.</param>
        public LazyList(object obj, string fieldName)
        {
            __Field f = obj._GetEntity().GetFieldByName(fieldName);
            
            _Sql = f._FkSql;
            _Params = new Tuple<string, object>[] { new Tuple<string, object>(":fk", f.Entity.PrimaryKey.GetValue(obj)) };
        }




        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected properties                                                                                             //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets the list values.</summary>
        protected List<T> _Items
        {
            get
            {
                if(_InternalItems == null)
                {
                    _InternalItems = new List<T>();
                    Orm._FillList(typeof(T), _InternalItems, _Sql, _Params);
                }

                return _InternalItems;
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // [interface] IList<T>                                                                                             //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets an item by its index.</summary>
        /// <param name="index">Index.</param>
        /// <returns>Item.</returns>
        public T this[int index]
        {
            get { return _Items[index]; }
            set { _Items[index] = value; }
        }


        /// <summary>Gets the number of items in this list.</summary>
        public int Count
        {
            get { return _Items.Count; }
        }


        /// <summary>Gets if the list is read-only.</summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return ((IList<T>) _Items).IsReadOnly; }
        }


        /// <summary>Adds an item to the list.</summary>
        /// <param name="item">Item.</param>
        public void Add(T item)
        {
            _Items.Add(item);
        }


        /// <summary>Clears the list.</summary>
        public void Clear()
        {
            _Items.Clear();
        }


        /// <summary>Returns if the list contains an item.</summary>
        /// <param name="item">Item.</param>
        /// <returns>Returns TRUE if the list contains the item, otherwise returns FALSE.</returns>
        public bool Contains(T item)
        {
            return _Items.Contains(item);
        }


        /// <summary>Copies the list to an array.</summary>
        /// <param name="array">Array.</param>
        /// <param name="arrayIndex">Starting index.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }


        /// <summary>Returns an enumerator for this list.</summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }


        /// <summary>Returns the index of an item.</summary>
        /// <param name="item">Item.</param>
        /// <returns>Index.</returns>
        public int IndexOf(T item)
        {
            return _Items.IndexOf(item);
        }


        /// <summary>Inserts an item into the list.</summary>
        /// <param name="index">Index.</param>
        /// <param name="item">Item.</param>
        public void Insert(int index, T item)
        {
            _Items.Insert(index, item);
        }


        /// <summary>Removes an item from the list.</summary>
        /// <param name="item">Item.</param>
        /// <returns>Returns TRUE if successful, otherwise returns FALSE.</returns>
        public bool Remove(T item)
        {
            return _Items.Remove(item);
        }


        /// <summary>Removes an item with a specific index from the list.</summary>
        /// <param name="index">Index.</param>
        public void RemoveAt(int index)
        {
            _Items.RemoveAt(index);
        }


        /// <summary>Returns an enumerator for this list.</summary>
        /// <returns>Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }
    }
}
