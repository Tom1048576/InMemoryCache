using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using ApplicationCore.Attributes;

namespace ApplicationCore.Extensions;

public static class TableMetadataExtensions
{
    private static readonly ConcurrentDictionary<Type, TableMetadataAttribute> Cache = new();

    public static TableMetadataAttribute GetTableMetadata<TEntry>() where TEntry : class
        => GetTableMetadata(typeof(TEntry));

    public static TableMetadataAttribute GetTableMetadata(Type entryType)
    {
        return Cache.GetOrAdd(entryType, type =>
        {
            var attribute = type.GetCustomAttributes(typeof(TableMetadataAttribute), inherit: false)
                .OfType<TableMetadataAttribute>()
                .SingleOrDefault();

            if (attribute is null)
            {
                throw new InvalidOperationException($"Type '{type.FullName}' is missing the required TableMetadataAttribute.");
            }

            return attribute;
        });
    }
}
