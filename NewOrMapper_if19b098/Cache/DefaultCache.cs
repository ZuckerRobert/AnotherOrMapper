using NewOrMapper_if19b098.Interfaces;
using System;
using System.Collections.Generic;



namespace NewOrMapper_if19b098
{
    /// <summary>This class provides a default cache implementation.</summary>
    public class DefaultCache: ICache
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected members                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Caches items.</summary>
        protected Dictionary<Type, Dictionary<object, object>> _Caches = new Dictionary<Type, Dictionary<object, object>>();



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected methods                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>Gets the cache for a type.</summary>
        /// <param name="t">Type.</param>
        /// <returns>Type cache.</returns>
        protected virtual Dictionary<object, object> _GetCache(Type t)
        {
            if(_Caches.ContainsKey(t)) { return _Caches[t]; }

            Dictionary<object, object> rval = new Dictionary<object, object>();
            _Caches.Add(t, rval);

            return rval;
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // [interface] ICache                                                                                               //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets an object from the cache.</summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>Object</returns>
        public virtual object Get(Type t, object pk)
        {
            Dictionary<object, object> c = _GetCache(t);

            if(c.ContainsKey(pk)) { return c[pk]; }
            return null;
        }


        /// <summary>Puts an object into the cache.</summary>
        /// <param name="obj">Object.</param>
        public virtual void Put(object obj)
        {
            if(obj != null) { _GetCache(obj.GetType())[obj._GetEntity().PrimaryKey.GetValue(obj)] = obj; }
        }


        /// <summary>Removes an object from the cache.</summary>
        /// <param name="obj">Object.</param>
        public virtual void Remove(object obj)
        {
            _GetCache(obj.GetType()).Remove(obj._GetEntity().PrimaryKey.GetValue(obj));
        }


        /// <summary>Returns if the cache contains an object with the given primary key.</summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>Returns TRUE if the object is in the Cache, otherwise returns FALSE.</returns>
        public virtual bool Contains(Type t, object pk)
        {
            return _GetCache(t).ContainsKey(pk);
        }


        /// <summary>Returns if the cache contains an object.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns TRUE if the object is in the Cache, otherwise returns FALSE.</returns>
        public virtual bool Contains(object obj)
        {
            return _GetCache(obj.GetType()).ContainsKey(obj._GetEntity().PrimaryKey.GetValue(obj));
        }


        /// <summary>Gets if an object has changed.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns TRUE if the object has changed or might have changed, returns FALSE if the object is unchanged.</returns>
        public virtual bool HasChanged(object obj)
        {
            return true;
        }
    }
}
