using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7Corp.P7CoreWebApp.Services
{
    public class SomeScoped : ISomeScoped, ISomeLazyScoped
    {
        public string Name => nameof(SomeScoped);
    }
}
