using System.Reflection;

namespace MySaleForceBackup
{
    public static class DescendentOfInterface
    {
        public static IEnumerable<Type> GetAllDescendantsOfGenericType(
            this Assembly assembly,
            Type genericTypeDefinition)
        {
            IEnumerable<Type> GetAllAscendants(Type t)
            {
                var current = t;

                while (current.BaseType != typeof(object))
                {
                    yield return current.BaseType;
                    current = current.BaseType;
                }
            }

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            if (genericTypeDefinition == null)
                throw new ArgumentNullException(nameof(genericTypeDefinition));

            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException(
                                            "Specified type is not a valid generic type definition.",
                                            nameof(genericTypeDefinition));

            return assembly.GetTypes()
                           .Where(t => GetAllAscendants(t).Any(d =>
                                                                   d.IsGenericType &&
                                                                   d.GetGenericTypeDefinition()
                                                                    .Equals(genericTypeDefinition)));
        }

        public static IEnumerable<Type> GetAllDescendantsOf(this Assembly assembly, Type indexBase)
        {
            return assembly
                   .GetTypes()
                   .Where(type =>
                              type != indexBase &&
                              !type.IsInterface &&
                              !type.IsAbstract &&
                              indexBase.IsAssignableFrom(type))
                   .ToList();
        }

        public static IEnumerable<Type> GetAllDescendantsOf<T>(this Assembly assembly)
        {
            var indexBase = typeof(T);
            return assembly
                   .GetTypes()
                   .Where(type =>
                              type != indexBase &&
                              !type.IsInterface &&
                              !type.IsAbstract &&
                              indexBase.IsAssignableFrom(type))
                   .ToList();
        }
    }

    public class Registration
    {

        public void RegisterAllImplementationOfInterface(Type indexBase, IServiceCollection services)
        {
            Assembly.GetAssembly(indexBase).GetAllDescendantsOf(indexBase).ToList()
                    .ForEach(indexType => { services.AddSingleton(indexBase, indexType); });
        }

        public static void RegisterAllImplementationOfInterface<T>(IServiceCollection services)
        {
            var indexBase = typeof(T);
            Assembly.GetAssembly(indexBase).GetAllDescendantsOf<T>().ToList()
                    .ForEach(indexType => { services.AddScoped(indexBase, indexType); });

        }
    }
}