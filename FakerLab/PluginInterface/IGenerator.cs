using System;

namespace PluginInterface
{
    public interface IGenerator
    {
        object Generate(Type type);
        bool IsGeneratable(Type type);
    }
}
