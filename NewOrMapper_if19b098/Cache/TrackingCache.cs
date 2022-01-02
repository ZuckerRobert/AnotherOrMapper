using NewOrMapper_if19b098.Interfaces;
using NewOrMapper_if19b098.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;



namespace NewOrMapper_if19b098
{
    /// <summary>This class provides a change-tracking cache implementation.</summary>
    public class TrackingCache: DefaultCache, ICache
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected members                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Hash items.</summary>
        protected Dictionary<Type, Dictionary<object, string>> _Hashes = new Dictionary<Type, Dictionary<object, string>>();



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected methods                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets the hash store for a type.</summary>
        /// <param name="t">Type.</param>
        /// <returns>Type hash store.</returns>
        protected virtual Dictionary<object, string> _GetHash(Type t)
        {
            if(_Hashes.ContainsKey(t)) { return _Hashes[t]; }

            Dictionary<object, string> rval = new Dictionary<object, string>();
            _Hashes.Add(t, rval);

            return rval;
        }


        /// <summary>Gets a hash for an object.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Hash.</returns>
        protected string _ComputeHash(object obj)
        {
            string rval = "";
            foreach(__Field i in obj._GetEntity().Internals) 
            {
                if(i.IsForeignKey)
                {
                    object m = i.GetValue(obj);
                    if(m != null) { rval += m._GetEntity().PrimaryKey.GetValue(m).ToString(); }
                }
                else { rval += (i.ColumnName + "=" + i.GetValue(obj).ToString() + ";"); }
            }

            foreach(__Field i in obj._GetEntity().Externals)
            {
                IEnumerable m = (IEnumerable) i.GetValue(obj);

                if(m != null)
                {
                    rval += (i.ColumnName + "=");
                    foreach(object k in m)
                    {
                        rval += k._GetEntity().PrimaryKey.GetValue(k).ToString() + ",";
                    }
                }
            }

            return Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(rval)));
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // [override] DefaultCache                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Puts an object into the cache.</summary>
        /// <param name="obj">Object.</param>
        public override void Put(object obj)
        {
            base.Put(obj);
            if(obj != null) { _GetHash(obj.GetType())[obj._GetEntity().PrimaryKey.GetValue(obj)] = _ComputeHash(obj); }
        }


        /// <summary>Removes an object from the cache.</summary>
        /// <param name="obj">Object.</param>
        public override void Remove(object obj)
        {
            base.Remove(obj);
            _GetHash(obj.GetType()).Remove(obj._GetEntity().PrimaryKey.GetValue(obj));
        }


        /// <summary>Gets if an object has changed.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns TRUE if the object has changed or might have changed, returns FALSE if the object is unchanged.</returns>
        public override bool HasChanged(object obj)
        {
            Dictionary<object, string> h = _GetHash(obj.GetType());
            object pk = obj._GetEntity().PrimaryKey.GetValue(obj);

            if(h.ContainsKey(pk))
            {
                return (h[pk] == _ComputeHash(obj));
            }

            return true;
        }
    }
}
