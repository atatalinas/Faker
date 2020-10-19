using System;
using System.Collections.Generic;
using System.Text;

namespace Faker
{
    [DTO]
    public class Foo
    {
        public float floatValue;
        public string str;
        public Bar bar;
        public DateTime dt;

        public Foo(float x)
        {
            floatValue = x;
        }

        private Foo()
        {
            floatValue = 30.6f;
            str = "abc";
        }
    }
}
