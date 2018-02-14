using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LatexToCSharpWebApi
{
    public class MethodParameters
    {
        public string type;
        public string name;

        public MethodParameters(string type, string name)
        {
            this.type = type;
            this.name = name;
        }
    }
        
}