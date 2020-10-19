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

        //стек с обработанными типами
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

            if (!processedTypes.Contains(type) && IsDTO(type))
            {
                MethodInfo createMethod = typeof(Faker).GetMethod("Create").MakeGenericMethod(type);
                return createMethod.Invoke(this, null);
            }

            //защита от зацикливания
            if (processedTypes.Contains(type) && IsDTO(type))
            {
                return null;
            }

            //для случая со списком
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    //получаем объект Type, представляющий универсальный тип, на основе которого можно сконструировать текущий тип
                    var listType = type.GetGenericTypeDefinition();

                    var genericType = type.GetGenericArguments()[0];

                    //получаем сконструированный тип, сформированный путем замещения элементов объекта genericType параметрами текущего универсального типа
                    var constructedListType = listType.MakeGenericType(genericType);

                    var random = new Random();

                    //получаем длину списка
                    int length = random.Next(2, 15);

                    var list = Activator.CreateInstance(constructedListType);

                    for (int i = 0; i < length; i++)
                    {
                        list.GetType().GetMethod("Add").Invoke(list, new[] { GenerateType(genericType) });
                    }

                    return list;
                }
            }
            return null;
        }

        //проверяем наличие атрибута DTO
        public bool IsDTO(Type type)
        {
            return type.GetCustomAttributes(typeof(DTOAttribute), false).Length == 1;
        }

        public T Create<T>()
        {
            Type type = typeof(T);

            //нет атрибута DTO
            if (!IsDTO(type))
            {
                return default;
            }

            processedTypes.Push(type);

            //получаем конструктор
            var constructor = GetConstructor(type);

            //получаем параметры конструктора
            object[] parameters = GenerateParameters(constructor);

            //вызываем конструктор с заданными параметрами
            object obj = constructor.Invoke(parameters);

            //задаем значение полей и свойств
            SetFieldsAndProperties(obj);


            processedTypes.Pop();

            return (T)obj;
        }

        //конструктор
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

        //получение параметров конструктора
        private object[] GenerateParameters(ConstructorInfo constructor)
        {
            var parameters = new List<object>();

            constructor.GetParameters()
                .ToList()
                .ForEach(t => parameters.Add(GenerateType(t.ParameterType)));

            return parameters.ToArray();
        }

        //задаем значение полей и свойств
        private void SetFieldsAndProperties(object obj)
        {
            //задаем значение полей
            obj.GetType().GetFields().ToList()
                .ForEach(f => f.SetValue(obj, GenerateType(f.FieldType)));

            //задаем значение свойств
            obj.GetType().GetProperties().ToList()
                .ForEach(p => p.SetValue(obj, GenerateType(p.PropertyType)));
        }
    }
}
