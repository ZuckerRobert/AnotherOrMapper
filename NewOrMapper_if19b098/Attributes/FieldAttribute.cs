using System;



namespace NewOrMapper_if19b098.Attributes
{
    /// <summary>This attribute marks a member as a field.</summary>
    public class FieldAttribute: Attribute
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public members                                                                                                   //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Database column name.</summary>
        public string ColumnName = null;

        /// <summary>Database column type.</summary>
        public Type ColumnType = null;

        /// <summary>Nullable flag.</summary>
        public bool Nullable = false;
    }
}
