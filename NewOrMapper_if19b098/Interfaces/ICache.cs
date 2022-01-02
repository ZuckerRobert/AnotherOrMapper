using System;



namespace NewOrMapper_if19b098.Interfaces
{
    /// <summary>Caches implement this interface.</summary>
    public interface ICache
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // methods                                                                                                          //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets an object from the cache.</summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>Object</returns>
        object Get(Type t, object pk);


        /// <summary>Puts an object into the cache.</summary>
        /// <param name="obj">Object.</param>
        void Put(object obj);
        

        /// <summary>Removes an object from the cache.</summary>
        /// <param name="obj">Object.</param>
        void Remove(object obj);


        /// <summary>Returns if the cache contains an object with the given primary key.</summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>Returns TRUE if the object is in the Cache, otherwise returns FALSE.</returns>
        bool Contains(Type t, object pk);


        /// <summary>Returns if the cache contains an object.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns TRUE if the object is in the Cache, otherwise returns FALSE.</returns>
        bool Contains(object obj);


        /// <summary>Gets if an object has changed.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns TRUE if the object has changed or might have changed, returns FALSE if the object is unchanged.</returns>
        bool HasChanged(object obj);
    }
}
