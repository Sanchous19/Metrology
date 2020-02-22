using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1
{
    public class ProgramAnalysis
    {
        public string[] _code;
        private readonly string[] _operators = { ">>=", "<<=", "&=", "|=", "^=", "~=", ">>", "<<", "&", "|", "^", "~", ">=", "<=", ">", "<", "!=" ,"==", "**=",
            "//=", "+=", "-=", "*=", "/=", "%=", "**", "//", "+", "*", "/", "%", "=", "@", "," };
        private readonly string[] _wordOperators = { "not in", "is not", "not", "or", "and", "in", "is", "del", "pass", "continue", "break", "return", "print" };
        private readonly string[] _wordOperatorsWithColon = { "elif", "else", "for", "if", "while", "lambda" };

        public ProgramAnalysis(string[] code)
        {
            NumberOfLinesInCode = code.Count();
            _code = new string[NumberOfLinesInCode];
            Array.Copy(code, _code, NumberOfLinesInCode);
            OperatorsInCode = new Dictionary<string, int>();
            OperandsInCode = new Dictionary<string, int>();

            DeleteComments();
            CountConstStrings();
            CountOperatorsInCode();
            DeleteImport();
            CountConstNumbers();
            #region operator -
            int numberOfMinus = CheckOperator("-");
            if (numberOfMinus > 0)
                OperatorsInCode.Add("-", numberOfMinus);
            #endregion
            #region operand True False
            if (CheckWordOperator("True") > 0)
                OperandsInCode.Add("True", CheckWordOperator("True"));
            if (CheckWordOperator("False") > 0)
                OperandsInCode.Add("False", CheckWordOperator("False"));
            #endregion
            #region operator .
            int numberOfVirgules = CheckOperator(".");
            if (numberOfVirgules > 0)
                OperatorsInCode.Add("x.attribute", numberOfVirgules);
            #endregion
            CountVariablesInCode();
            OperatorsInCode = OperatorsInCode.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            OperandsInCode = OperandsInCode.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            NumberOfUniqueOperators = OperatorsInCode.Count;
            NumberOfUniqueOperands = OperandsInCode.Count;

            foreach (var pair in OperatorsInCode)
                NumberOfAllOperators += pair.Value;
            foreach (var pair in OperandsInCode)
                NumberOfAllOperands += pair.Value;

            DictionaryOfTheProgram = NumberOfUniqueOperators + NumberOfUniqueOperands;
            ProgramLength = NumberOfAllOperators + NumberOfAllOperands;
            ProgramScope = (int)(ProgramLength * Math.Log(DictionaryOfTheProgram, 2));
        }
        
        public int NumberOfLinesInCode { get; set; }
        public Dictionary<string, int> OperatorsInCode { get; set; }
        public Dictionary<string, int> OperandsInCode { get; set; }
        public int NumberOfUniqueOperators { get; set; }
        public int NumberOfUniqueOperands { get; set; }
        public int NumberOfAllOperators { get; set; }
        public int NumberOfAllOperands { get; set; }
        public int DictionaryOfTheProgram { get; set; }
        public int ProgramLength { get; set; }
        public int ProgramScope { get; set; }


        public void DeleteComments()
        {
            //int count = 0;
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                if (_code[i].Contains('#'))
                {
                    //count++;
                    int num = _code[i].IndexOf('#');
                    _code[i] = _code[i].Remove(num, _code[i].Length - num);
                }
            }
            //if (count > 0)
            //    OperatorsInCode.Add("#", count);
        }


        public void DeleteImport()
        {
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                if (_code[i].Length > 6 && _code[i].Substring(0, 6) == "import")
                {
                    _code[i] = _code[i].Remove(0, 6);
                    string lib = _code[i].Trim();
                    int len = lib.Length;
                    _code[i] = "";
                    for (int j = i + 1; j < NumberOfLinesInCode; j++) 
                    {
                        string line = _code[j];
                        for (int k = 0; k < _code[j].Length; k++)
                        {
                            if (k < line.Length - len + 1 && String.Compare(lib, line.Substring(k, len)) == 0 && (k == 0 || line[k - 1] == ' ' || line[k - 1] == '(')
                                && ((k + len) == line.Length || line[k + len] == ' ' || line[k + len] == '.'))
                            {
                                line = line.Remove(k, len);
                                line = line.Insert(k, " ");
                                _code[j] = line;
                            }
                        }
                    }
                }
            }
        }


        public void CountConstStrings()
        {
            List<string> constStrings = new List<string>();
            char quote = '\'';
            bool inQuotes = false;
            string constString = null;

            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                string line = _code[i];
                for (int j = 0; j < line.Length; j++)
                {
                    char ch = line[j];
                    if (inQuotes)
                    {
                        _code[i] = _code[i].Remove(j, 1);
                        _code[i] = _code[i].Insert(j, " ");
                        line = _code[i];
                        if (ch == quote)
                        {
                            if (constString.Last() == '\\' && constString[constString.Length - 2] != '\\')
                            {
                                constString = constString.Insert(constString.Length, ch.ToString());
                                continue;
                            }
                            inQuotes = false;
                            constString = constString.Insert(constString.Length, ch.ToString());
                            constStrings.Add(constString);
                            constString = null;
                            continue;
                        }
                        else
                            constString = constString.Insert(constString.Length, ch.ToString());
                    }
                    else if (ch == '\'' || ch == '"')
                    {
                        constString = "";
                        constString = constString.Insert(constString.Length, ch.ToString());
                        quote = ch;
                        inQuotes = true;
                        _code[i] = _code[i].Remove(j, 1);
                        _code[i] = _code[i].Insert(j, " ");
                        line = _code[i];
                    }
                }
                if (inQuotes)
                    constString = constString.Insert(constString.Length, '\n'.ToString());
            }

            for (int i = 0; i < constStrings.Count; i++) 
            {
                string str = constStrings[i];
                int num = 1;
                for (int j = i + 1; j < constStrings.Count; j++)
                {
                    if (constStrings[j] == str)
                    {
                        num++;
                        constStrings.RemoveAt(j);
                        j--;
                    }
                }
                OperandsInCode.Add(str, num);
            }
        }


        public void CountConstNumbers()
        {
            List<string> constNumbers = new List<string>();
            bool isNumber = false, isNumeral = false;
            string constNumber = "";

            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                string line = _code[i];
                for (int j = 0; j < line.Length; j++)
                {
                    char ch = line[j];
                    if ((ch >= '0' && ch <= '9') || ch == '.' || ch == '-')
                    {
                        if (ch == '-' && constNumber.Length != 0)
                        {
                            isNumber = false;
                            isNumeral = false;
                            constNumber = null;
                            constNumber = "";
                            continue;
                        }
                        if (ch >= '0' && ch <= '9')
                            isNumeral = true;
                        isNumber = true;
                        constNumber = constNumber.Insert(constNumber.Length, ch.ToString());
                        if (j == line.Length - 1 && isNumeral && (j - constNumber.Length + 1 == 0 || line[j - constNumber.Length] == ' ')) 
                        {
                            j = j - constNumber.Length + 1;
                            _code[i] = _code[i].Remove(j, constNumber.Length);
                            _code[i] = _code[i].Insert(j, " ");
                            line = _code[i];
                            constNumbers.Add(constNumber);
                        }
                    }
                    else if (isNumber)
                    {
                        if(isNumeral)
                        {
                            if (line[j] == ' ' && (j - constNumber.Length == 0 || line[j - constNumber.Length - 1] == ' '))
                            {
                                j -= constNumber.Length;
                                _code[i] = _code[i].Remove(j, constNumber.Length);
                                _code[i] = _code[i].Insert(j, " ");
                                line = _code[i];
                                constNumbers.Add(constNumber);
                            }
                        }
                        isNumber = false;
                        isNumeral = false;
                        constNumber = null;
                        constNumber = "";
                    }
                }
                isNumber = false;
                isNumeral = false;
                constNumber = null;
                constNumber = "";
            }

            for (int i = 0; i < constNumbers.Count; i++)
            {
                string str = constNumbers[i];
                int num = 1;
                for (int j = i + 1; j < constNumbers.Count; j++)
                {
                    if (constNumbers[j] == str)
                    {
                        num++;
                        constNumbers.RemoveAt(j);
                        j--;
                    }
                }
                OperandsInCode.Add(str, num);
            }
        }


        public void CountVariablesInCode()
        {
            List<string> variables = new List<string>();
            for (int i = 0; i < NumberOfLinesInCode; i++)
                variables.AddRange(_code[i].Split(' '));
            
            for (int i = 0; i < variables.Count; i++)
            {
                string str = variables[i];
                int num = 1;
                for (int j = i + 1; j < variables.Count; j++)
                {
                    if (variables[j] == str)
                    {
                        num++;
                        variables.RemoveAt(j);
                        j--;
                    }
                }
                if (str != "") 
                    OperandsInCode.Add(str, num);
            }
        }


        public void CountOperatorsInCode()
        {
            #region operators + * / & | ...
            for (int i = 0; i < _operators.Count(); i++) 
            {
                string operat = _operators[i];
                int num = CheckOperator(operat);
                if (num > 0)
                    OperatorsInCode.Add(operat, num);
            }
            #endregion
            #region operators [] [:] [::]
            int zeroColons = 0, oneColon = 0, twoColons = 0;
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                int numOfColon = 0, numOfBrackets = 0;
                for (int j = 0; j < _code[i].Length; j++)
                {
                    if (numOfBrackets > 0)
                    {
                        if (_code[i][j] == ':')
                            numOfColon++;
                        else if (_code[i][j] == ']')
                        {
                            numOfBrackets--;
                            if (numOfColon == 0)
                                zeroColons++;
                            else if (numOfColon == 1)
                                oneColon++;
                            else if (numOfColon == 2)
                                twoColons++;
                            numOfColon = 0;
                        }
                        else if (_code[i][j] == '[')
                            numOfBrackets++;
                        else
                            continue;
                        _code[i] = _code[i].Remove(j, 1);
                        _code[i] = _code[i].Insert(j, " ");
                    }
                    else if (_code[i][j] == '[')
                    {
                        numOfBrackets = 1;
                        _code[i] = _code[i].Remove(j, 1);
                        _code[i] = _code[i].Insert(j, " ");
                    }
                }
            }
            if (zeroColons > 0)
                OperatorsInCode.Add("[]", zeroColons);
            if (oneColon > 0)
                OperatorsInCode.Add("[:]", oneColon);
            if (twoColons > 0)
                OperatorsInCode.Add("[::]", twoColons);
            #endregion
            #region operator {:,...}
            int numberOfCurlyBrackets = 0;
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                bool inCurlyBrackets = false;
                for (int j = 0; j < _code[i].Length; j++)
                {
                    if (_code[i][j] == '{')
                    {
                        numberOfCurlyBrackets++;
                        inCurlyBrackets = true;
                    }
                    else if (_code[i][j] == '}')
                        inCurlyBrackets = false;
                    else if (!(inCurlyBrackets && _code[i][j] == ':'))
                        continue;
                    _code[i] = _code[i].Remove(j, 1);
                    _code[i] = _code[i].Insert(j, " ");
                }
            }
            if (numberOfCurlyBrackets != 0)
                OperatorsInCode.Add("{:,...}", numberOfCurlyBrackets);
            #endregion
            #region operators not in is or and ...
            for (int i = 0; i < _wordOperators.Count(); i++)
            {
                string operat = _wordOperators[i];
                int num = CheckWordOperator(operat);
                if (num > 0)
                    OperatorsInCode.Add(operat, num);
            }
            #endregion
            #region delete def
            int numberOfDef = 0;
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                if (_code[i].Length >= 3 && String.Compare("def", _code[i].Substring(0, 3)) == 0 && _code[i][3] == ' ') 
                {
                    _code[i] = "";
                    numberOfDef++;
                }
            }
            //if (numberOfDef > 0)
            //    OperatorsInCode.Add("def ... :", numberOfDef);
            #endregion
            #region operator for if elif else while def lambda
            for (int i = 0; i < _wordOperatorsWithColon.Count(); i++)
            {
                string operat = _wordOperatorsWithColon[i];
                int num = CheckWordOperatorWithColon(operat);
                if (num > 0)
                {
                    if (i == 2) 
                    {
                        int num2 = OperatorsInCode["in"] - num;
                        OperatorsInCode.Remove("in");
                        if (num2 > 0) 
                            OperatorsInCode.Add("in", num2);
                        OperatorsInCode.Add("for ... in ... :", num);
                    }
                    else if (i == 3)
                        OperatorsInCode.Add("if:-elif:-else:", num);
                    else if (i == 4)
                        OperatorsInCode.Add("while ... :", num);
                    else if (i == 5)
                        OperatorsInCode.Add("lambda:", num);
                }
            }
            #endregion
            CountFunctions();
            #region operator ()
            int numberOfBrackets = CheckOperator("(");
            CheckOperator(")");
            if (numberOfBrackets > 0) 
                OperatorsInCode.Add("()", numberOfBrackets);
            #endregion
            #region operator ,
            int numberOfVirgules = CheckOperator(",");
            if (numberOfVirgules > 0)
                OperatorsInCode.Add(",", numberOfVirgules);
            #endregion
        }


        public int CheckOperator(string operat)
        {
            int num = 0;
            int len = operat.Length;
            for (int i = 0; i < NumberOfLinesInCode; i++) 
            {
                string line = _code[i];
                for (int j = 0; j < line.Length - len + 1; j++) 
                {
                    if (String.Compare(operat, line.Substring(j, len)) == 0)
                    {
                        line = line.Remove(j, len);
                        line = line.Insert(j, " ");
                        _code[i] = line;
                        num++;
                    }
                }
            }
            return num;
        }


        public int CheckWordOperator(string operat)
        {
            int num = 0;
            int len = operat.Length;
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                string line = _code[i];
                for (int j = 0; j < line.Length - len + 1; j++)
                {
                    if (String.Compare(operat, line.Substring(j, len)) == 0 && (j == 0 || line[j - 1] == ' ' || line[j - 1] == '(' || line[j - 1] == ')') && 
                        ((j + len) == line.Length || line[j + len] == ' ' || line[j + len] == '(' || line[j + len] == ')' || line[j + len] == ':'))
                    {
                        line = line.Remove(j, len);
                        line = line.Insert(j, " ");
                        _code[i] = line;
                        num++;
                    }
                }
            }
            return num;
        }


        public int CheckWordOperatorWithColon(string operat)
        {
            int num = 0, len = operat.Length;
            bool deleteColon = true;
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                string line = _code[i];
                for (int j = 0; j < line.Length; j++)
                {
                    if (j < line.Length - len + 1 && String.Compare(operat, line.Substring(j, len)) == 0 && (j == 0 || line[j - 1] == ' ' || line[j - 1] == '(' || 
                        line[j - 1] == ')') && ((j + len) == line.Length || line[j + len] == ' ' || line[j + len] == '(' || line[j + len] == ')' || line[j + len] == ':')) 
                    {
                        line = line.Remove(j, len);
                        line = line.Insert(j, " ");
                        _code[i] = line;
                        num++;
                        deleteColon = true;
                    }
                    else if (deleteColon && line[j]==':')
                    {
                        line = line.Remove(j, 1);
                        line = line.Insert(j, " ");
                        _code[i] = line;
                        deleteColon = false;
                    }
                }
                deleteColon = false;
            }
            return num;
        }


        public void CountFunctions()
        {
            List<string> listOfFunctions = new List<string>();
            for (int i = 0; i < _code.Length; i++)
            {
                string line = _code[i];
                for (int j = 1; j < line.Length; j++) 
                {
                    if (line[j] == '(' && line[j - 1] != ' ' && line[j - 1] != '(')
                    {
                        _code[i] = _code[i].Remove(j, 1);
                        _code[i] = _code[i].Insert(j, " ");
                        line = _code[i];
                        int numberOfBrackets = 0;
                        for (int k = j + 1; k < line.Length; k++) 
                        {
                            if (line[k] == '(')
                                numberOfBrackets++;
                            if (line[k] == ')')
                            {
                                if (numberOfBrackets == 0)
                                {
                                    _code[i] = _code[i].Remove(k, 1);
                                    _code[i] = _code[i].Insert(k, " ");
                                    line = _code[i];
                                    break;
                                }
                                else
                                    numberOfBrackets--;
                            }
                        }
                        for (int k = j - 1; ; k--)
                        {
                            if (k == -1 || line[k] == '.' || line[k] == ' ')
                            {
                                string nameOfFunction = line.Substring(k + 1, j - k - 1);
                                listOfFunctions.Add(nameOfFunction);
                                _code[i] = _code[i].Remove(k + 1, j - k - 1);
                                _code[i] = _code[i].Insert(k + 1, " ");
                                line = _code[i];
                                j = k + 2;
                                break;
                            }
                        }
                    }

                }
            }
            for (int i = 0; i < listOfFunctions.Count; i++)
            {
                string str = listOfFunctions[i];
                int num = 1;
                for (int j = i + 1; j < listOfFunctions.Count; j++)
                {
                    if (listOfFunctions[j] == str)
                    {
                        num++;
                        listOfFunctions.RemoveAt(j);
                        j--;
                    }
                }
                OperatorsInCode.Add(str + "()", num);
            }
        }
    }
}
