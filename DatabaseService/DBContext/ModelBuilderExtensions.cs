using Microsoft.EntityFrameworkCore;

namespace DatabaseService;

static class ModelBuilderExtensions
{
    /// <summary>
    /// Method from class example that converts the names of the tables into lovercases 
    /// Needed for automapper and making life easier
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="names"></param>
    //public static void CreateMap(this ModelBuilder modelBuilder, params string[] names)
    public static void CreateMap(this ModelBuilder modelBuilder)
    {
        //getting access to the entity types dbSet below
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // getting the name of Categories and Products to lower cases.
            entityType.SetTableName(entityType.GetTableName().ToLower());
            foreach (var property in entityType.GetProperties())
            {
                var propertyName = property.Name.ToLower();
                /*  //.when names are = to property then i want to extract and put that in lowercase
                  var entityName = "";
                  if (names.ToList().Contains(property.Name)) // names is converted here into a list of strings not array anymore
                  {
                      entityName = entityType.ClrType.Name.ToLower(); // this also gets rid of all stuff including id and name (here we add the name of the class in front of the attribute: categoryname)
                  }*/

                property.SetColumnName(propertyName);
            }
        }
    }
}