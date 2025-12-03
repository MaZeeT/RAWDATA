using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

internal static class ModelBuilderExtensions
{
    public static void CreateMap(this ModelBuilder modelBuilder)
    {
        //getting access to the entity types dbSet below
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // getting the name of Categories and Products to lower cases.
            entityType.SetTableName(entityType.GetTableName()?.ToLower());
            foreach (var property in entityType.GetProperties())
            {
                var propertyName = property.Name.ToLower();
                property.SetColumnName(propertyName);
            }
        }
    }
}