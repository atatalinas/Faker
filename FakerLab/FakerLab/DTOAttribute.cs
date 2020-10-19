using System;
using System.Collections.Generic;
using System.Text;

namespace Faker
{
    //пользовательский атрибут DTO
    [AttributeUsage(AttributeTargets.Class)]
    public class DTOAttribute: Attribute
    {
    }
}
