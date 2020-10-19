using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterface;

namespace DateTimeGenerator
{
    public class DateTimeGenerator : IGenerator
    {
        private readonly Random random;

        public DateTimeGenerator()
        {
            random = new Random();
        }

        public bool IsGeneratable(Type type)
        {
            return type == typeof(DateTime);
        }
         
        //получаем случайные дату и время
        public object Generate(Type type)
        {
            return new DateTime(random.Next(0, DateTime.Now.Year), random.Next(1, 12), random.Next(1, 30),
                random.Next(0, 24), random.Next(0, 60), random.Next(0, 60));
        }
    }
}
