using NewOrMapper_if19b098.Models;
using Npgsql;
using System;
using System.Data;
using System.Linq;

namespace NewOrMapper_if19b098
{
    public class DBSettings
    {
        public IDbConnection Connection { get; set; }

        public DBSettings(IDbConnection connection)
        {
            Connection = connection;
        }


        public IDbCommand CreateCreateTableCommand(object obj, IDbCommand cmd)
        {
            if (cmd is NpgsqlCommand)
            {
                cmd.CommandText += CreateNpgsqlTeachers();
                Console.WriteLine($"Create Table Teachers");

                //cmd = CreateNpgsqlTableComman(obj);
                cmd.CommandText += CreateNpgsqlClasses();
                Console.WriteLine($"Create Table Classes");

                cmd.CommandText += CreateNpgsqlCourses();
                Console.WriteLine($"Create Table Classes");

                cmd.CommandText += CreateNpgsqlStudents();
                Console.WriteLine($"Create Table Students");

                cmd.CommandText += CreateNpgsqlStudentCourses();
                Console.WriteLine($"Create Table StudentCourses");

                cmd.CommandText += CreateNpgsqlLock();
                Console.WriteLine($"Create Table Lock");

            }
            else if (cmd is null)
            {
            }
            return cmd;
        }

        private string DropAllTables()
        {
            return @"drop table locks;
                    drop table students;
                    drop table student_courses;
                    drop table courses;
                    drop table classes;
                    drop table teachers;
                    ";

        }
        private string CreateNpgsqlStudentCourses()
        {
            return "CREATE TABLE STUDENT_COURSES ( " +
                "KSTUDENT TEXT NOT NULL REFERENCES STUDENTS(ID), " +
                "KCOURSE text NOT NULL REFERENCES COURSES(ID) );\n";

        }

        private string CreateNpgsqlStudents()
        {
            return "CREATE TABLE STUDENTS ( " +
                "ID Text NOT NULL PRIMARY KEY, " +
                "NAME TEXT, " +
                "FIRSTNAME TEXT, " +
                "GENDER INTEGER, " +
                "BDATE TIMESTAMP, " +
                "KCLASS TEXT REFERENCES CLASSES(ID), " +
                "GRADE INTEGER );\n";
        }

        private string CreateNpgsqlLock()
        {
            return "CREATE TABLE LOCKS ( " +
                "OWNER_KEY TEXT NOT NULL," +
                "TYPE_KEY TEXT NOT NULL, " +
                "OBJECT_ID TEXT NOT NULL " +
                ");\n";
        }

        private string CreateNpgsqlCourses()
        {
            string query = "CREATE TABLE COURSES " +
                "( ID TEXT NOT NULL PRIMARY KEY, " +
                "HACTIVE INTEGER NOT NULL DEFAULT 0, " +
                "NAME TEXT, " +
                "KTEACHER TEXT NOT NULL REFERENCES TEACHERS(ID) " +
                ");\n";

            return query;
        }

        private string CreateNpgsqlTeachers()
        {
            string tablename = "TEACHERS";
            string start = $"CREATE TABLE {tablename} (\n";
            string middle = "";
            string end = "";
            string[] columns = {"ID", "NAME", "FIRSTNAME", "GENDER", "BDATE", "HDATE", "SALARY" };
            string[] type = { "TEXT NOT NULL", "TEXT", "TEXT", "INTEGER", "TIMESTAMP", "TIMESTAMP", "INTEGER" };
        
            for(int i = 0; i < columns.Length; i++)
            {
                middle += columns[i] + " ";
                middle += type[i];
                middle += ",\n";
            }
            end += "PRIMARY KEY (ID)";

            var result = start + middle  + end + ")\n;\n";

            return result;
        }
        private string CreateNpgsqlClasses()
        {
            string tablename = "CLASSES";
            string start = $"CREATE TABLE {tablename} (\n";
            string middle = "";
            string end = "";
            string[] columns = { "ID", "NAME", "KTEACHER" };
            string[] type = { "TEXT NOT NULL", "TEXT", "TEXT"};

            for (int i = 0; i < columns.Length; i++)
            {
                middle += columns[i] + " ";
                middle += type[i];
                middle += ",\n";
            }
            end += "PRIMARY KEY (ID),\n";

            //fk_tourid character varying(32) not null,
            //foreign key(fk_tourid) references tours(tourid) on delete cascade

            end += "foreign key (KTEACHER) references TEACHERS(ID)";

            return start + middle + end + ")\n;\n"; 
        }
        private string CreateNpgsqlTableComman(object obj)
        {
            __Entity entityFromObj = obj._GetEntity();
            string start = "CREATE TABLE " + entityFromObj.TableName + " (\n";
            string createTable = string.Empty;
            string ending = string.Empty;

            bool first = true;

            for (int i = 0; i < entityFromObj.Fields.Length; i++)
            {
                if (entityFromObj.Fields[i].IsForeignKey)
                {
                    //TODO
                    string fk_Colums = entityFromObj.Fields[i].ColumnName;
                    string parentTable = "";
                    string parentKeyColums = "";

                    ending += $"FOREIGN KEY ({fk_Colums}) " +
                                $"REFERENCES {parentTable} ({parentKeyColums})";
                }

                if (i > 0)
                {
                    createTable += ",\n";
                }
                createTable += "\"" + entityFromObj.Fields[i].ColumnName + "\""; //name

                //plus datatype
                if (entityFromObj.Fields[i].ColumnType == typeof(string))
                {
                    createTable += " VARCHAR (50) ";
                }
                else if (entityFromObj.Fields[i].ColumnType == typeof(int))
                {
                    createTable += " INT ";
                }
                else if (entityFromObj.Fields[i].ColumnType == typeof(System.DateTime))
                {
                    createTable += " TIMESTAMP ";
                }
                else
                {
                    createTable += " INT ";
                }

                if (entityFromObj.Fields[i].IsPrimaryKey)
                {
                    createTable += "PRIMARY KEY";
                }

            }
            var answer = start + (createTable + ")\n");
            return answer;
        }
        public bool CheckIfTableExists(string tableName)
        {
            //Connection.Open();
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {tableName}";
            IDataReader response = null;
            try
            {
                response = cmd.ExecuteReader();
            }
            catch
            {
                //TODO Exception handling
            }

            cmd.Dispose();            

            if (response == null || response.FieldCount == 0)
                return false;

            Connection.Close();
            return true;
        }
        public bool CreateTable(object obj)
        {
            Connection.Open();
            IDbCommand cmd = Connection.CreateCommand();

            try
            {
                __Entity entityFromObj = obj._GetEntity();
                DBSettings dBSettings = new(Connection);

                //cmd = Connection.CreateCommand();

                //cmd = dBSettings.CreateCreateTableCommand(obj, cmd);
                cmd = dBSettings.CreateCreateTableCommand(obj, cmd);

                var response = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                cmd.Dispose();
                Connection.Close();
            }
            return true;
        }
    }
}
