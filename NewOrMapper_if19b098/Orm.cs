using NewOrMapper_if19b098.Interfaces;
using NewOrMapper_if19b098.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace NewOrMapper_if19b098
{
    /// <summary>This class allows access to OR framework functionalities.</summary>
    public static class Orm
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // private static members                                                                                           //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Entities.</summary>
        private static Dictionary<Type, __Entity> _Entities = new Dictionary<Type, __Entity>();



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public static properties                                                                                         //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets the database connection used by the framework.</summary>
        public static IDbConnection Connection { get; set; }


        /// <summary>Gets or sets the cache used by the framework.</summary>
        public static ICache Cache { get; set; }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public static methods                                                                                            //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>Gets an object.</summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="pk">Primary key.</param>
        /// <returns>Object.</returns>
        public static T Get<T>(object pk)
        {
            return (T) _CreateObject(typeof(T), pk, null);
        }


        /// <summary>Saves an object.</summary>
        /// <param name="obj">Object.</param>
        public static void Save(object obj)
        {
            if(Cache != null) { if(!Cache.HasChanged(obj)) return; }

            __Entity ent = obj._GetEntity();
            DBSettings dbSettings = new(Connection);

            //Check if table exists            
            if (!dbSettings.CheckIfTableExists(ent.TableName))
            {
                //table eventuell erstellen:
                if (!dbSettings.CreateTable(obj))
                    throw new Exception("Creation from Table failed");
            }
            Connection.Close();
            Connection.Open();

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("INSERT INTO " + ent.TableName + " (");

            string update = "ON CONFLICT (" + ent.PrimaryKey.ColumnName + ") DO UPDATE SET ";
            string insert = "";

            IDataParameter p;
            bool first = true;

            //alle felder durchgehen
            for (int i = 0; i < ent.Internals.Length; i++)
            {
                if(i > 0) { cmd.CommandText += ", "; insert += ", "; }
                cmd.CommandText += ent.Internals[i].ColumnName;

                insert += (":v" + i.ToString());

                p = cmd.CreateParameter();
                p.ParameterName = (":v" + i.ToString());
                p.Value = ent.Internals[i].ToColumnType(ent.Internals[i].GetValue(obj));
                cmd.Parameters.Add(p);

                if(!ent.Internals[i].IsPrimaryKey)
                {
                    if(first) { first = false; } else { update += ", "; }
                    update += (ent.Internals[i].ColumnName + " = " + (":w" + i.ToString()));

                    p = cmd.CreateParameter();
                    p.ParameterName = (":w" + i.ToString());
                    p.Value = ent.Internals[i].ToColumnType(ent.Internals[i].GetValue(obj));
                    cmd.Parameters.Add(p);
                }
            }
            cmd.CommandText += (") VALUES (" + insert + ") " + update);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            foreach(__Field i in ent.Externals) 
            {
                i.UpdateReferences(obj); 
            }

            if(Cache != null) 
            { 
                Cache.Put(obj); 
            }
        }


        /// <summary>Deletes an object.</summary>
        /// <param name="obj">Object.</param>
        public static void Delete(object obj)
        {
            __Entity ent = obj._GetEntity();

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("DELETE FROM " + ent.TableName + " WHERE " + ent.PrimaryKey.ColumnName + " = :pk");
            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":pk";
            p.Value = ent.PrimaryKey.GetValue(obj);
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            if(Cache != null) { Cache.Remove(obj); }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // private static methods                                                                                           //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets an entity descriptor for an object.</summary>
        /// <param name="o">Object.</param>
        /// <returns>Entity.</returns>
        internal static __Entity _GetEntity(this object o)
        {
            Type t = ((o is Type) ? (Type) o :o.GetType());

            if(!_Entities.ContainsKey(t))
            {
                _Entities.Add(t, new __Entity(t));
            }

            return _Entities[t];
        }



        /// <summary>Searches the cached objects for an object and returns it.</summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <param name="objects">Cached objects.</param>
        /// <returns>Returns the cached object that matches the current reader or NULL if no such object has been found.</returns>
        internal static object _SearchCache(Type t, object pk, ICollection<object> localCache)
        {
            if((Cache != null) && Cache.Contains(t, pk))
            {
                return Cache.Get(t, pk);
            }

            if(localCache != null)
            {
                foreach(object i in localCache)
                {
                    if(i.GetType() != t) continue;

                    if(t._GetEntity().PrimaryKey.GetValue(i).Equals(pk)) { return i; }
                }
            }
            return null;
        }


        /// <summary>Creates an object from a database reader.</summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="re">Reader.</param>
        /// <param name="localCache">Local cache.</param>
        /// <returns>Object.</returns>
        internal static object _CreateObject(Type t, Dictionary<string, object> columnValuePairs, ICollection<object> localCache)
        {
            __Entity ent = t._GetEntity();
            object rval = _SearchCache(t, ent.PrimaryKey.ToFieldType(columnValuePairs[ent.PrimaryKey.ColumnName], null), localCache);

            bool foundInCache = true;
            if(rval == null)
            {
                foundInCache = false;
                if (localCache == null) 
                { 
                    localCache = new List<object>(); 
                }
                localCache.Add(rval = Activator.CreateInstance(t));
            }

            foreach(__Field i in ent.Internals)
            {
                i.SetValue(rval, i.ToFieldType(columnValuePairs[i.ColumnName], localCache));
            }

            if (!foundInCache)
            {
                foreach (__Field i in ent.Externals)
                {
                    if (typeof(ILazy).IsAssignableFrom(i.Type))
                    {
                        i.SetValue(rval, Activator.CreateInstance(i.Type, rval, i.Member.Name));
                    }
                    else
                    {
                        i.SetValue(rval, i.Fill(Activator.CreateInstance(i.Type), rval, localCache));
                    }
                }
            }            

            return rval;
        }


        /// <summary>Creates an instance by its primary keys.</summary>
        /// <param name="t">Type.</param>
        /// <param name="primaryKey">Primary key.</param>
        /// <param name="localCache">Local cache.</param>
        /// <returns>Object.</returns>
        internal static object _CreateObject(Type t, object primaryKey, ICollection<object> localCache)
        {
            object rval = _SearchCache(t, primaryKey, localCache);
            __Entity modelEntity = t._GetEntity();

            IDbCommand cmd = Connection.CreateCommand();

            cmd.CommandText = t._GetEntity().GetSQL() + " WHERE " + t._GetEntity().PrimaryKey.ColumnName + " = :pk";

            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = (":pk");
            p.Value = primaryKey;
            cmd.Parameters.Add(p);

            IDataReader re = cmd.ExecuteReader();
            Dictionary<string, object> columnValuePairs = DataReaderToDictionary(re, modelEntity);


            re.Close();
            cmd.Dispose();

            if (Cache != null) { Cache.Put(rval); }

            Connection.Close();
            Connection.Open();
            rval = _CreateObject(t, columnValuePairs, localCache);

            return rval;
        }
        internal static object _CreateObjectOld(Type t, object primaryKey, ICollection<object> localCache)
        {
            object resultValue = _SearchCache(t, primaryKey, localCache);

            if (resultValue == null)
            {
                IDbCommand command = Connection.CreateCommand();
                __Entity modelEntity = t._GetEntity();
                command.CommandText = modelEntity.GetSQL() + " WHERE " + modelEntity.PrimaryKey.ColumnName + " = :pk";

                IDataParameter para = command.CreateParameter();
                para.ParameterName = (":pk");
                para.Value = primaryKey;
                command.Parameters.Add(para);

                IDataReader readerData = command.ExecuteReader();
                Dictionary<string, object> columnValuePairs = DataReaderToDictionary(readerData, modelEntity);
                readerData.Close();
                resultValue = _CreateObject(t, columnValuePairs, localCache);
                command.Dispose();
            }
            if (resultValue == null) { throw new Exception("No data."); }
            return resultValue;
        }
        private static Dictionary<string, object> DataReaderToDictionary(IDataReader dataReader, __Entity entity)
        {
            Dictionary<string, object> columnValuePairs = new();
            if (dataReader.Read())
            {
                foreach (__Field modelField in entity.Internals)
                {
                    columnValuePairs.Add(modelField.ColumnName, dataReader.GetValue(dataReader.GetOrdinal(modelField.ColumnName)));
                }
            }
            return columnValuePairs;
        }

        /// <summary>Fills a list.</summary>
        /// <param name="t">Type.</param>
        /// <param name="list">List.</param>
        /// <param name="re">Reader.</param>
        /// <param name="localCache">Local cache.</param>
        internal static void _FillList(Type t, object list, List<Dictionary<string, object>> dict, ICollection<object> localCache = null)
        {
            foreach(var dic in dict)
            {
                list.GetType().GetMethod("Add").Invoke(list, new object[] { _CreateObject(t, dic, localCache) });
            }
        }



        /// <summary>Fills a list.</summary>
        /// <param name="t">Type.</param>
        /// <param name="list">List.</param>
        /// <param name="sql">SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <param name="localCache">Local cache.</param>
        internal static void _FillList(Type t, object list, string sql, IEnumerable<Tuple<string, object>> parameters, ICollection<object> localCache = null)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = sql;

            foreach(Tuple<string, object> i in parameters)
            {
                IDataParameter p = cmd.CreateParameter();
                p.ParameterName = i.Item1;
                p.Value = i.Item2;
                cmd.Parameters.Add(p);
            }
            __Entity modelEntity = t._GetEntity();

            IDataReader readerData = cmd.ExecuteReader();

            List<Dictionary<string, object>> tempList = new();
            Dictionary<string, object> columnValuePairs = null;

            do
            {
                columnValuePairs = DataReaderToDictionary(readerData, modelEntity);
                if(columnValuePairs.Count > 0)
                tempList.Add(columnValuePairs);
            }
            while (columnValuePairs != null && columnValuePairs.Count > 0);
            readerData.Close();

            _FillList(t, list, tempList, localCache);
            readerData.Dispose();
            cmd.Dispose();
        }
    }
}
