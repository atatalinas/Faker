using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Faker
{
    public class PluginsLoader<T> 
    {
        //путь к плагинам
        private readonly string PLUGINS_PATH;

        public PluginsLoader(string path)
        {
            PLUGINS_PATH = path;
        }

        //загрузка плагинов
        public List<T> LoadPlugins()
        {
            var pluginsDirectory = new DirectoryInfo(PLUGINS_PATH);

            if (!pluginsDirectory.Exists)
            {
                pluginsDirectory.Create();
            }

            var assemblies = GetAssemblies();
            var plugins = GetPlugins(assemblies);

            //получаем плагины
            return plugins;
        }

        private List<Assembly> GetAssemblies()
        {
            //получаем файлы с расширением .dll
            var files = Directory.GetFiles(PLUGINS_PATH, "*.dll");

            //загружаем сборку с заданным именем или путем
            var assemblies = files.Select(file => Assembly.LoadFrom(file)).ToList();

            return assemblies;
        }

        private List<T> GetPlugins(List<Assembly> assemblies)
        {
            var plugins = new List<T>();

            foreach (var asm in assemblies)
            {
                Type[] types;

                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    types = null;
                }

                if (types != null)
                {
                    foreach (var type in types)
                    {
                        //Выполняем поиск интерфейса с заданным именем
                        if (type.GetInterface(typeof(T).Name) != null)
                        {
                            //находим заданный тип в этой сборке и создает его экземпляр
                            var plugin = (T)asm.CreateInstance(type.FullName);
                            plugins.Add(plugin);
                        }
                    }
                }
            }

            return plugins;
        }
    }
}
