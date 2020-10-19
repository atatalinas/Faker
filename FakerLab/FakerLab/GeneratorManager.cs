using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PluginInterface;


namespace Faker
{
    public class GeneratorManager : IGenerator
    {
       //список генераторов
        private List<IGenerator> generators;

        //путь к папке с плагинами
        private readonly string PLUGINS_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

        public GeneratorManager()
        {
            var loader = new PluginsLoader<IGenerator>(PLUGINS_PATH);

            //загружаем плагины
            generators = loader.LoadPlugins();
        }

        //подходит ли указанный тип
        public bool IsGeneratable(Type type)
        {
            foreach (var generator in generators)
            {
                if (generator.IsGeneratable(type))
                {
                    return true;
                }
            }

            return false;
        }

        //получаем генераторы
        public object Generate(Type type)
        {
            foreach (var generator in generators)
            {
                if (generator.IsGeneratable(type))
                {
                    return generator.Generate(type);
                }
            }

            return null;
        }
    }
}
