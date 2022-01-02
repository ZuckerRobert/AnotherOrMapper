using NewOrMapper_if19b098.Interfaces;
using System;



namespace NewOrMapper_if19b098.LazyLoading
{
    /// <summary>This class implements a lazy loading wrapper for framework objects.</summary>
    /// <typeparam name="T">Type.</typeparam>
    public class LazyObject<T>: ILazy
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected members                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Primary key.</summary>
        protected object _Pk;

        /// <summary>Value.</summary>
        protected T _Value;

        /// <summary>Initialized flag.</summary>
        protected bool _Initialized = false;



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // constructors                                                                                                     //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="pk">Primary key.</param>
        public LazyObject(object pk = null)
        {
            _Pk = pk;
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets the object value.</summary>
        public T Value
        {
            get
            {
                if(!_Initialized) { _Value = Orm.Get<T>(_Pk); _Initialized = true; }
                return _Value;
            }
            set
            {
                _Value = value;
                _Initialized = true;
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // operators                                                                                                        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Implements an implicit operator for the Lazy class.</summary>
        /// <param name="lazy">Lazy object.</param>
        public static implicit operator T(LazyObject<T> lazy)
        {
            return lazy._Value;
        }


        /// <summary>Implements an implicit operator for the Lazy class.</summary>
        /// <param name="lazy">Lazy object.</param>
        public static implicit operator LazyObject<T>(T obj)
        {
            LazyObject<T> rval = new LazyObject<T>();
            rval.Value = obj;

            return rval;
        }
    }
}
