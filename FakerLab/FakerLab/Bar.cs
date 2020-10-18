using System;
using System.Collections.Generic;
using System.Text;

namespace Faker
{
    [DTO]
    public class Bar
    {
        public int integerValue;
        public List<string> stringList;
        public Foo foo;
    }
}
