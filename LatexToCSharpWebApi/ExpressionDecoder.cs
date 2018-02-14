using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace LatexToCSharpWebApi
{
    public class ExpressionDecoder
    {
        public Dictionary<string,string> syntaxTemplates;
        public List<char> ignoreCharacters;
        public string DecodeBody(string expression)
        {   
            
            //syntaxTemplates.Add("\\",);
            string header = expression.Split('=')[0];
            string body = expression.Split('=')[1];
            body = CleanBody(body);

            string[] paramsList = header.Split('(')[1].Split(',');
            for(int i=0;i<paramsList.Length;i++){
                if(paramsList[i].Contains(')')) paramsList[i] = paramsList[i].Remove(paramsList[i].IndexOf(')'),1);
                if(paramsList[i].Contains('(')) paramsList[i] = paramsList[i].Remove(paramsList[i].IndexOf('('),1);
                System.Console.WriteLine(paramsList[i]);
            }
            System.Console.WriteLine("header recived: "+header+"\nbody recived: \""+body+"\"");
            string codeReturnValue = mainDecoding(body,paramsList);
            return "\treturn "+codeReturnValue+";";
        }
        public double exampleMethod( int x, int y, int a, int b){
	        return Math.Pow(2,22);
        }

        public string mainDecoding(string body,string[] paramsList){
            System.Console.WriteLine("DECODING "+body);
            //System.Console.WriteLine(Math.Log(2,8));
            if(body.Contains(")("))
            body = body.Replace(")(",")*(");
            string codeReturnValue = "";
            List<string> tokenList = new List<string>();
            ignoreCharacters = new List<char>();
            ignoreCharacters.Add('+');
            ignoreCharacters.Add('-');
            ignoreCharacters.Add('=');
            ignoreCharacters.Add('*');
            ignoreCharacters.Add('/');
            ignoreCharacters.Add('\\');
            ignoreCharacters.Add('^');
            ignoreCharacters.Add('}');
            ignoreCharacters.Add('{');
            ignoreCharacters.Add(')');
            ignoreCharacters.Add('(');
            ignoreCharacters.Add(']');
            ignoreCharacters.Add('[');
            syntaxTemplates = new Dictionary<string,string>();
            syntaxTemplates.Add("^","System.Math.Pow($p1,$p2)");
            syntaxTemplates.Add("\\\\log_","System.Math.Log($p2,$p1)");
            syntaxTemplates.Add("\\\\pi","System.Math.PI");
            syntaxTemplates.Add("\\\\frac","$p1/$p2");
            syntaxTemplates.Add("\\\\sqrt","System.Math.Pow($p2,1/$p1)");
            string syntaxRecognision = "";
            bool parameterAdded = false;
            for(int i=0;i<body.Length;i++)
            {
                syntaxRecognision += body[i];
                //System.Console.WriteLine("container: "+syntaxRecognision);
                
                if(paramsList.Contains(syntaxRecognision))
                {
                    //System.Console.WriteLine("parameter "+syntaxRecognision);
                    if(parameterAdded)
                    {
                        System.Console.WriteLine("adding *");
                        tokenList.Add("*");
                    }
                    System.Console.WriteLine("adding "+syntaxRecognision);
                    tokenList.Add(syntaxRecognision);
                    parameterAdded = true;
                    syntaxRecognision = "";
                }
                else if(isNumber(body[i]))
                {
                    //System.Console.WriteLine("Num: "+syntaxRecognision);
                    if(body.Length>i+1)
                    if(isIgnoredCharacter(body[i+1])&&body[i+1]!='.')
                    {
                        System.Console.WriteLine("adding "+syntaxRecognision);
                        tokenList.Add(syntaxRecognision);
                        syntaxRecognision ="";
                        if(body[i+1]=='\\') tokenList.Add("*");
                    }
                    else if(!isIgnoredCharacter(body[i+1])&&!isNumber(body[i+1])&&body[i+1]!='.')
                    {
                        System.Console.WriteLine("adding "+syntaxRecognision);
                        tokenList.Add(syntaxRecognision);
                        syntaxRecognision ="";
                        System.Console.WriteLine("adding *");
                        //tokenList.Add("*");
                    }
                    //syntaxRecognision+=body[i];
                }
                else if(isIgnoredCharacter(body[i]))
                {
                    if(body[i]=='\\')
                    {
                        Console.WriteLine("backslash detected");
                        for(int j=i+1;j<body.Length;j++)
                        {
                            syntaxRecognision+=body[j];
                            Console.WriteLine(syntaxRecognision);
                            if(syntaxTemplates.ContainsKey(syntaxRecognision))
                            {
                                Console.WriteLine("recognized: "+syntaxRecognision); 
                                i=j;
                                break;
                            }
                        }                       

                    }
                    else if(body[i]=='('||body[i]=='{'||body[i]=='[')
                    {
                        syntaxRecognision = "";
                        string tmpToken = "";
                        System.Console.WriteLine("BL start");
                        int bracetLevel = 0;
                        for(int j=i;j<body.Length;j++)
                        {
                            tmpToken+=body[j];
                            //System.Console.WriteLine("BLtoken: "+tmpToken);
                            if(body[j]==')'||body[j]=='}'||body[j]==']')
                            {
                                bracetLevel--;
                               
                            }
                            if(body[j]=='{'||body[j]=='('||body[j]=='[')
                            {
                                bracetLevel++;
                            }
                            if(bracetLevel==0)
                            {
                                tmpToken = mainDecoding(tmpToken.Substring(1,tmpToken.Length-2),paramsList);
                                if(tmpToken.Length>1) tmpToken = "("+tmpToken+")";
                                i=j;
                                break;
                            }
                        }
                        System.Console.WriteLine("BL add "+tmpToken);
                        tokenList.Add(tmpToken);
                        //i+=tmpToken.Length;

                    }
                    //System.Console.WriteLine("ignored: "+syntaxRecognision);
                    if(syntaxRecognision.Length>0)
                    {
                         
                        System.Console.WriteLine("adding "+syntaxRecognision);
                        tokenList.Add(syntaxRecognision);
                        parameterAdded = false;
                        if(i+1<body.Length)
                        {
                            if(syntaxRecognision=="\\\\sqrt"&&body[i+1]!='[') tokenList.Add("2");
                            if(syntaxRecognision=="\\\\log_"&&body[i+1]!='{') 
                            {
                                tokenList.Add(body[i+1]+"");
                                i = i+1;
                            }
                            if(syntaxRecognision=="\\\\pi"
                            &&(isNumber(body[i+1])||(!isIgnoredCharacter(body[i+1])
                            &&!isNumber(body[i+1]))||body[i+1]=='\\')
                            &&body[i+1]!='*')
                                tokenList.Add("*");
                        }
                            
                        syntaxRecognision = "";
                         //potentialParam = false;
                    }
                    //tokenList.Add(body[i]+"");
                }

            }
            if(syntaxRecognision.Length>0)
            {
                System.Console.WriteLine("adding "+syntaxRecognision);
                tokenList.Add(syntaxRecognision);
            }
                
            System.Console.WriteLine("TokenList:");
            printTokenList(tokenList);
            for(int i=0;i<=tokenList.Count()-1;i++)
            {
                if(syntaxTemplates.ContainsKey(tokenList[i]))
                {
                    if(tokenList[i]=="^")
                    {
                        codeReturnValue = codeReturnValue.Remove(codeReturnValue.LastIndexOf(tokenList[i-1]),tokenList[i-1].Length);
                        if(tokenList[i+1]=="-")
                        {
                            codeReturnValue += syntaxTemplates[tokenList[i]].Replace("$p1",tokenList[i-1]).Replace("$p2",tokenList[i+1]+tokenList[i+2]);
                            i+=2;
                        }
                        else
                        {
                            codeReturnValue += syntaxTemplates[tokenList[i]].Replace("$p1",tokenList[i-1]).Replace("$p2",tokenList[i+1]);
                            i+=1;
                        }

                    }
                    else if(tokenList[i]=="\\\\frac"||tokenList[i]=="\\\\sqrt"||tokenList[i]=="\\\\log_")
                    {
                        Console.WriteLine("frac detected");
                        if(i+2<tokenList.Count)
                        {
                            codeReturnValue+=syntaxTemplates[tokenList[i]].Replace("$p1",tokenList[i+1]).Replace("$p2",tokenList[i+2]);
                            tokenList[i+2] = syntaxTemplates[tokenList[i]].Replace("$p1",tokenList[i+1]).Replace("$p2",tokenList[i+2]);
                        }
                        if(i<tokenList.Count)
                            i+=2;
                    }
                    else if(tokenList[i]=="\\\\pi")
                    {
                        codeReturnValue+=syntaxTemplates[tokenList[i]];
                    }
                                        
                }
                else codeReturnValue+=tokenList[i];
            }
            System.Console.WriteLine("returning: "+codeReturnValue);
            return codeReturnValue;
        }
        public string CleanBody(string expression)
        {
            string tmp = expression;
            while(tmp.IndexOf(' ')>-1)
            {
                tmp = tmp.Remove(tmp.IndexOf(' '),1);
            } 
            while(tmp.IndexOf(',')>-1)
            {
                tmp = tmp.Replace(',','.');
            }
            return tmp;
        }
        
        public void printTokenList(List<string> arg)
        {
            foreach(string t in arg){
                System.Console.WriteLine(t);
            }
        }
        private bool isNumber(char arg)
        {
            return (arg<='9'&&arg>='0');
        }
        private bool isIgnoredCharacter(char arg)
        {
            return ignoreCharacters.Contains(arg);
        }
        public string GenerateParameters(string expression)
        {
            var methodParams = "";
            List<MethodParameters> methodParamsList = new List<MethodParameters>();
            if(expression.Contains("="))
            {
                expression = expression.Substring(expression.IndexOf("(")+1, expression.IndexOf(")")-1);
            }
            System.Console.WriteLine(expression);
            //var parameterFromExpression = expression.Select(n => n).Where(b => char.IsLetter(b)).OrderBy(n => n);
            var parameterFromExpression = GetWords(expression);

            foreach (var item in parameterFromExpression.Distinct())
            {
                    methodParamsList.Add(new MethodParameters("int", item.ToString()));
            }

            methodParamsList.ForEach(element => {
                methodParams = methodParams + ' ' + element.type + ' ' + element.name + ',';
             });
             methodParams = methodParams.Substring(0, methodParams.Length-1);

            return methodParams;
        }

        static string[] GetWords(string input)
{
    MatchCollection matches = Regex.Matches(input, @"\b[a-zA-Z']*\b");

    var words = from m in matches.Cast<Match>()
                where !string.IsNullOrEmpty(m.Value)
                select TrimSuffix(m.Value);

     
    return words.ToArray();
}

static string TrimSuffix(string word)
{
    int apostropheLocation = word.IndexOf('\'');
    if (apostropheLocation != -1)
    {
        word = word.Substring(0, apostropheLocation);
    }

    return word;
}
    }   
}