using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LatexToCSharpWebApi.Controllers
{
    public class ValuesController : Controller
    {
        // GET api/values/5
        
        [HttpPost, Route("/api/Values/GetMethodData/")]
        public List<String> GetMethodData([FromBody] Object parameter)
        {
            string expression = parameter.ToString();
            expression = expression.Split(":")[1];
            expression = expression.Remove(0,1);
            expression = expression.Remove(expression.LastIndexOf("\""),expression.Length-expression.LastIndexOf("\""));
             System.Console.WriteLine("działa");
            if(expression!=null) System.Console.WriteLine(expression);
            else Console.WriteLine("Still nothing");
             ExpressionDecoder decoder = new ExpressionDecoder();
              List<String> methodDataJson = new List<String>();
             methodDataJson.Add(decoder.GenerateParameters(expression));
             methodDataJson.Add(decoder.DecodeBody(expression));
             return methodDataJson;
        }
    }
}
