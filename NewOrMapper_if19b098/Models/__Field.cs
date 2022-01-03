using NewOrMapper_if19b098.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;



namespace NewOrMapper_if19b098.Models
{
    /// <summary>This class holds field metadata.</summary>
    internal class __Field
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // constructors                                                                                                     //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="entity">Parent entity.</param>
        public __Field(__Entity entity)
        {
            Entity = entity;
        }



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



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public methods                                                                                                   //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Returns a database column type equivalent for a field type value.</summary>
        /// <param name="value">Value.</param>
        /// <returns>Database type representation of the value.</returns>
        public object ToColumnType(object value)
        {
            if (IsForeignKey)
            {
                if (value == null) { return null; }

                Type t = (typeof(ILazy).IsAssignableFrom(Type) ? Type.GenericTypeArguments[0] : Type);
                return t._GetEntity().PrimaryKey.ToColumnType(t._GetEntity().PrimaryKey.GetValue(value));
            }
            if (value is Enum)
            {
                int temp = (int)value;
                if (ColumnType == typeof(Enum)) { return (int)value; }
                else if (Type.IsEnum) { return (int)value; }
                else if (ColumnType == typeof(int)) { return (int)value; }
                else if (ColumnType == typeof(short)) { return (short)((int)value); }
                else if (ColumnType == typeof(long)) { return (long)((int)value); }
                return temp;
            }

            if (Type == ColumnType)
            {
                return value;
            }

            if (value is bool)
            {
                if (ColumnType == typeof(int)) { return (((bool)value) ? 1 : 0); }
                else if (ColumnType == typeof(short)) { return (short)(((bool)value) ? 1 : 0); }
                else if (ColumnType == typeof(long)) { return (long)(((bool)value) ? 1 : 0); }
            }

            return value;
        }


        /// <summary>Returns a field type equivalent for a database column type value.</summary>
        /// <param name="value">Value.</param>
        /// <returns>Field type representation of the value.</returns>
        public object ToFieldType(object value, ICollection<object> localCache)
        {
            if (IsForeignKey)
            {
                if (typeof(ILazy).IsAssignableFrom(Type))
                {
                    return Activator.CreateInstance(Type, value);
                }
                return Orm._CreateObject(Type, value, localCache);
            }

            if (Type == typeof(bool))
            {
                if (value is int) { return ((int)value != 0); }
                else if (value is short) { return ((short)value != 0); }
                else if (value is long) { return ((long)value != 0); }
            }

            if (Type == typeof(short)) { return Convert.ToInt16(value); }
            else if (Type == typeof(int)) { return Convert.ToInt32(value); }
            else if (Type == typeof(long)) { return Convert.ToInt64(value); }

            if (Type.IsEnum) 
            { 
                return Enum.ToObject(Type, value); 
            }

            return value;
        }


        /// <summary>Gets the field value.</summary>
        /// <param name="obj">Object.</param>
        /// <returns>Field value.</returns>
        public object GetValue(object obj)
        {
            if (Member is PropertyInfo)
            {
                object rval = ((PropertyInfo)Member).GetValue(obj);

                if (rval is ILazy)
                {
                    if (!(rval is IEnumerable)) { return rval.GetType().GetProperty("Value").GetValue(rval); }
                }

                return rval;
            }

            throw new NotSupportedException("Member type not supported.");
        }


        /// <summary>Sets the field value.</summary>
        /// <param name="obj">Object.</param>
        /// <param name="value">Value.</param>
        public void SetValue(object obj, object value)
        {
            if (Member is PropertyInfo)
            {
                ((PropertyInfo)Member).SetValue(obj, value);
                return;
            }

            throw new NotSupportedException("Member type not supported.");
        }


        /// <summary>Fills a list for a foreign key.</summary>
        /// <param name="list">List.</param>
        /// <param name="obj">Object.</param>
        /// <param name="localCache">Local cache.</param>
        /// <returns>List.</returns>
        public object Fill(object list, object obj, ICollection<object> localCache)
        {
            Orm._FillList(Type.GenericTypeArguments[0], list, _FkSql,
                          new Tuple<string, object>[] { new Tuple<string, object>(":fk", Entity.PrimaryKey.GetValue(obj)) }, localCache);

            return list;
        }


        /// <summary>Updates references.</summary>
        /// <param name="obj">Object.</param>
        public void UpdateReferences(object obj)
        {
            if (!IsExternal) return;
            if (GetValue(obj) == null) return;

            Type innerType = Type.GetGenericArguments()[0];
            __Entity innerEntity = innerType._GetEntity();
            object pk = Entity.PrimaryKey.ToColumnType(Entity.PrimaryKey.GetValue(obj));

            if (IsManyToMany)
            {
                IDbCommand cmd = Orm.Connection.CreateCommand();
                cmd.CommandText = ("DELETE FROM " + AssignmentTable + " WHERE " + ColumnName + " = :pk");
                IDataParameter p = cmd.CreateParameter();
                p.ParameterName = ":pk";
                p.Value = pk;
                cmd.Parameters.Add(p);

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                foreach (object i in (IEnumerable)GetValue(obj))
                {
                    cmd = Orm.Connection.CreateCommand();
                    cmd.CommandText = ("INSERT INTO " + AssignmentTable + "(" + ColumnName + ", " + RemoteColumnName + ") VALUES (:pk, :fk)");
                    p = cmd.CreateParameter();
                    p.ParameterName = ":pk";
                    p.Value = pk;
                    cmd.Parameters.Add(p);

                    p = cmd.CreateParameter();
                    p.ParameterName = ":fk";
                    p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));
                    cmd.Parameters.Add(p);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            else
            {
                __Field remoteField = innerEntity.GetFieldForColumn(ColumnName);

                if (remoteField.IsNullable)
                {
                    try
                    {
                        IDbCommand cmd = Orm.Connection.CreateCommand();
                        cmd.CommandText = ("UPDATE " + innerEntity.TableName + " SET " + ColumnName + " = NULL WHERE " + ColumnName + " = :fk");
                        IDataParameter p = cmd.CreateParameter();
                        p.ParameterName = ":fk";
                        p.Value = pk;
                        cmd.Parameters.Add(p);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    catch (Exception) { }
                }

                foreach (object i in (IEnumerable)GetValue(obj))
                {
                    //TODO convert lazy obj to teacher
                    remoteField.SetValue(i, obj);

                    IDbCommand cmd = Orm.Connection.CreateCommand();
                    cmd.CommandText = ("UPDATE " + innerEntity.TableName + " SET " + ColumnName + " = :fk WHERE " + innerEntity.PrimaryKey.ColumnName + " = :pk");
                    IDataParameter p = cmd.CreateParameter();
                    p.ParameterName = ":fk";
                    p.Value = pk;
                    cmd.Parameters.Add(p);                    

                    p = cmd.CreateParameter();
                    p.ParameterName = ":pk";
                    p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));
                    cmd.Parameters.Add(p);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
        }
    }
}
