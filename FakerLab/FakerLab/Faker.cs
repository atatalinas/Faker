using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Faker
{
    public class Faker : IFaker
    {
        private GeneratorManager generatorsManager;
        private Stack<Type> processedTypes;

        public Faker()
        {
            generatorsManager = new GeneratorManager();
            processedTypes = new Stack<Type>();
        }

        private object GenerateType(Type type)
        {
            if (generatorsManager.IsGeneratable(type))
            {
                return generatorsManager.Generate(type);
            }

            if (processedTypes.Contains(type))
            {
                return null;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listType = type.GetGenericTypeDefinition();
                    var genericType = type.GetGenericArguments()[0];
                    var constructedListType = listType.MakeGenericType(genericType);

                    var random = new Random();
                    int length = random.Next(2, 15);

                    var list = Activator.CreateInstance(constructedListType);

                    for (int i = 0; i < length; i++)
                    {
                        list.GetType().GetMethod("Add").Invoke(list, new[] { Generate(genericType) });
                    }

                    return list;
                }
            }
            return null;
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            FieldInfo[] fields = type.GetFields();
            Type[] types = fields.Select(field => field.GetType()).ToArray();

            var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null, types, null);

            if (constructor == null)
            {
                constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            }

            return constructor;
        }

        private object[] GenerateParameters(ConstructorInfo constructor)
        {
            var parameters = new List<object>();

            constructor.GetParameters()
                .ToList()
                .ForEach(t => parameters.Add(GenerateType(t.ParameterType)));

            return parameters.ToArray();
        }

        private void SetFieldsAndProperties(object obj)
        {
            obj.GetType().GetFields().ToList()
                .ForEach(f => f.SetValue(obj, GenerateType(f.FieldType)));
            obj.GetType().GetProperties().ToList()
                .ForEach(p => p.SetValue(obj, GenerateType(p.PropertyType)));
        }

        public T Create<T>()
        {
            Type type = typeof(T);

            processedTypes.Push(type);

            var constructor = GetConstructor(type);

            object[] parameters = GenerateParameters(constructor);
            object obj = constructor.Invoke(parameters);

            SetFieldsAndProperties(obj);


            processedTypes.Pop();

            return (T)obj;
        }
    }
}
