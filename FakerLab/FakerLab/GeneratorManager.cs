using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PluginInterface;


namespace Faker
{
    public class GeneratorManager : IGenerator
    {
       
        private List<IGenerator> generators;

        private readonly string PLUGINS_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

        public GeneratorManager()
        {
            var loader = new PluginsLoader<IGenerator>(PLUGINS_PATH);

            generators = loader.Load();
        }
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
