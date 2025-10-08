namespace ApplicationCore.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class TableMetadataAttribute : Attribute
{
    public TableMetadataAttribute(string databaseName, string tableName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name must be provided.", nameof(databaseName));
        }

        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("Table name must be provided.", nameof(tableName));
        }

        DatabaseName = databaseName;
        TableName = tableName;
        TableKey = $"{databaseName}.{tableName}";
    }

    public string DatabaseName { get; }

    public string TableName { get; }

    /// <summary>
    /// Composite key used for cache storage.
    /// </summary>
    public string TableKey { get; }
}
