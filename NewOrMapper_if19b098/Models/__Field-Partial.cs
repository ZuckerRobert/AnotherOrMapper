using NewOrMapper_if19b098.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;



namespace NewOrMapper_if19b098.Models
{
    /// <summary>This class holds field metadata.</summary>
    internal partial class __Field
    {
        


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets the parent entity.</summary>
        public __Entity Entity
        {
            get; private set;
        }


        /// <summary>Gets the field member.</summary>
        public MemberInfo Member
        {
            get; internal set;
        }


        /// <summary>Gets the field type.</summary>
        public Type Type
        {
            get
            {
                if (Member is PropertyInfo) { return ((PropertyInfo)Member).PropertyType; }

                throw new NotSupportedException("Member type not supported.");
            }
        }


        /// <summary>Gets the column name in table.</summary>
        public string ColumnName
        {
            get; internal set;
        }


        /// <summary>Gets the column database type.</summary>
        public Type ColumnType
        {
            get; internal set;
        }


        /// <summary>Gets if the column is a primary key.</summary>
        public bool IsPrimaryKey
        {
            get; internal set;
        } = false;


        /// <summary>Gets if the column is a foreign key.</summary>
        public bool IsForeignKey
        {
            get; internal set;
        } = false;


        /// <summary>Assignment table.</summary>
        public string AssignmentTable
        {
            get; internal set;
        }


        /// <summary>Remote (far side) column name.</summary>
        public string RemoteColumnName
        {
            get; internal set;
        }


        /// <summary>Gets if the field belongs to a m:n relationship.</summary>
        public bool IsManyToMany
        {
            get; internal set;
        }


        /// <summary>Gets if the column is nullable.</summary>
        public bool IsNullable
        {
            get; internal set;
        } = false;


        /// <summary>Gets if the the column is an external foreign key field.</summary>
        public bool IsExternal
        {
            get; internal set;
        } = false;



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal properties                                                                                              //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets the foreign key SQL.</summary>
        internal string _FkSql
        {
            get
            {
                if (IsManyToMany)
                {
                    return Type.GenericTypeArguments[0]._GetEntity().GetSQL() +
                           " WHERE ID IN (SELECT " + RemoteColumnName + " FROM " + AssignmentTable + " WHERE " + ColumnName + " = :fk)";
                }

                return Type.GenericTypeArguments[0]._GetEntity().GetSQL() + " WHERE " + ColumnName + " = :fk";
            }
        }
    }
}
