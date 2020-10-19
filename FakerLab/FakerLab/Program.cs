using System;

namespace Faker
{
    class Program
    {
        static void Main(string[] args)
        {
            var faker = new Faker();
            Foo foo = faker.Create<Foo>();
            Console.WriteLine($"floatValue: {foo.floatValue}, string: {foo.str}, dt: {foo.dt}, bar.integerValue: {foo.bar.integerValue}, bar.foo: {foo.bar.foo}");
            Console.WriteLine("bar.list:");

            foreach (var item in foo.bar.stringList)
            {
                Console.Write($"{item}\n");
            }


            Console.ReadKey();
        }
    }
}
