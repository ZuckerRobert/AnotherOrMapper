using NewOrMapper_if19b098.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace NewOrMapper_if19b098.Models
{
    /// <summary>This class holds entity metadata.</summary>
    internal partial class __Entity
    {


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets the member type.</summary>
        public Type Member
        {
            get; private set;
        }


        /// <summary>Gets the table name.</summary>
        public string TableName
        {
            get; private set;
        }


        /// <summary>Gets the entity fields.</summary>
        public __Field[] Fields
        {
            get; private set;
        }


        /// <summary>Gets external fields.</summary>
        /// <remarks>External fields are referenced fields that do not belong to the underlying table.</remarks>
        public __Field[] Externals
        {
            get; private set;
        }


        /// <summary>Gets internal fields.</summary>
        public __Field[] Internals
        {
            get; private set;
        }


        /// <summary>Gets the entity primary key.</summary>
        public __Field PrimaryKey
        {
            get; private set;
        }



    }
}
