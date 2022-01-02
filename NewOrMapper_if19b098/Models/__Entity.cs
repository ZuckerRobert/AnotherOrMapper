using NewOrMapper_if19b098.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace NewOrMapper_if19b098.Models
{
    /// <summary>This class holds entity metadata.</summary>
    internal class __Entity
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // constructors                                                                                                     //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="t">Type.</param>
        public __Entity(Type t)
        {
            EntityAttribute tattr = (EntityAttribute) t.GetCustomAttribute(typeof(EntityAttribute));
            if((tattr == null) || (string.IsNullOrWhiteSpace(tattr.TableName)))
            {
                TableName = t.Name.ToUpper();
            }
            else { TableName = tattr.TableName; }

            Member = t;
            List<__Field> fields = new List<__Field>();

            foreach(PropertyInfo i in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if((IgnoreAttribute) i.GetCustomAttribute(typeof(IgnoreAttribute)) != null) continue;

                __Field field = new __Field(this);

                FieldAttribute fattr = (FieldAttribute) i.GetCustomAttribute(typeof(FieldAttribute));

                if(fattr != null)
                {
                    if(fattr is PrimaryKeyAttribute)
                    {
                        PrimaryKey = field;
                        field.IsPrimaryKey = true;
                    }

                    field.ColumnName = (fattr?.ColumnName ?? i.Name);
                    field.ColumnType = (fattr?.ColumnType ?? i.PropertyType);

                    field.IsNullable = fattr.Nullable;

                    if(field.IsForeignKey = (fattr is ForeignKeyAttribute))
                    {
                        field.IsExternal = typeof(IEnumerable).IsAssignableFrom(i.PropertyType);

                        field.AssignmentTable  = ((ForeignKeyAttribute) fattr).AssignmentTable;
                        field.RemoteColumnName = ((ForeignKeyAttribute) fattr).RemoteColumnName;
                        field.IsManyToMany = (!string.IsNullOrWhiteSpace(field.AssignmentTable));
                    }
                }
                else
                {
                    if((i.GetGetMethod() == null) || (!i.GetGetMethod().IsPublic)) continue;

                    field.ColumnName = i.Name;
                    field.ColumnType = i.PropertyType;
                }
                field.Member = i;

                fields.Add(field);
            }

            Fields = fields.ToArray();
            Internals = fields.Where(m => (!m.IsExternal)).ToArray();
            Externals  = fields.Where(m => m.IsExternal).ToArray();
        }



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



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public methods                                                                                                   //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets the entity SQL.</summary>
        /// <param name="prefix">Prefix.</param>
        /// <returns>SQL string.</returns>
        public string GetSQL(string prefix = null)
        {
            if(prefix == null)
            {
                prefix = "";
            }

            string rval = "SELECT ";
            for(int i = 0; i < Internals.Length; i++)
            {
                if(i > 0) { rval += ", "; }
                rval += prefix.Trim() + Internals[i].ColumnName;
            }
            rval += (" FROM " + TableName);

            return rval;
        }


        /// <summary>Gets a field by its column name.</summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Field.</returns>
        public __Field GetFieldForColumn(string columnName)
        {
            columnName = columnName.ToUpper();
            foreach(__Field i in Internals)
            {
                if(i.ColumnName.ToUpper() == columnName) { return i; }
            }

            return null;
        }


        /// <summary>Gets a field by its field name.</summary>
        /// <param name="fieldName">Field name.</param>
        /// <returns>Field.</returns>
        public __Field GetFieldByName(string fieldName)
        {
            foreach(__Field i in Fields)
            {
                if(i.Member.Name == fieldName) { return i; }
            }

            return null;
        }
    }
}
