using PluginInterface;
using System;
using System.Collections.Generic;



namespace BaseTypeGenerator
{
    public class BaseTypeGenerator: IGenerator
    {
        private readonly Random random;

        public BaseTypeGenerator()
        {
            random = new Random();
        }

        public bool IsGeneratable(Type type)
        {
            if (new List<Type>() { typeof(bool), typeof(byte), typeof(sbyte),
                typeof(short), typeof(ushort), typeof(int), typeof(uint),
                typeof(long), typeof(ulong), typeof(char), typeof(double),
                typeof(float), typeof(decimal), typeof(string)}.Contains(type))
            {
                return true;
            }

            return false;
        }

        private long GenerateLong()
        {
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        private string GenerateString()
        {
            int count = random.Next(1, 20);
            string result = "";

            for (int i = 0; i < count; i++)
            {
                result += (char)random.Next('A', 'z');
            }

            return result;
        }


        public object Generate(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean: return random.Next(0, 2) > 0;

                case TypeCode.Byte: return (byte)random.Next();

                case TypeCode.SByte: return (sbyte)random.Next();

                case TypeCode.Int16: return (short)random.Next();

                case TypeCode.UInt16: return (ushort)random.Next();

                case TypeCode.Int32: return random.Next();

                case TypeCode.UInt32: return (uint)random.Next();

                case TypeCode.Int64: return GenerateLong();

                case TypeCode.UInt64: return (ulong)GenerateLong();

                case TypeCode.Char: return (char)random.Next('A', 'z');

                case TypeCode.Double: return random.NextDouble();

                case TypeCode.Single: return (float)random.NextDouble();

                case TypeCode.Decimal: return new decimal(random.NextDouble());

                case TypeCode.String: return GenerateString();

                default: return null;
            }
        }

    }
}
