using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LR2
{
    public class ProgramAnalysis
    {
        public string[] _code;

        public ProgramAnalysis(string[] code)
        {
            NumberOfLinesInCode = code.Length;
            _code = new string[NumberOfLinesInCode];
            Array.Copy(code, _code, NumberOfLinesInCode);

            DeleteUnnecessaryStrings();
            NumberOfConditionalStatementsLabel = CountConditionalStatementsLabel();
            NumberOfOperators = CountOperators();
            SaturationOfConditionalStatements = (double)NumberOfConditionalStatementsLabel / (double)NumberOfOperators;

            _code = new string[NumberOfLinesInCode];
            Array.Copy(code, _code, NumberOfLinesInCode);
            CountMaximumNestingLevelOfConditionalOperator();
        }

        public int NumberOfLinesInCode { get; set; }
        public int NumberOfOperators { get; set; }
        public int NumberOfConditionalStatementsLabel { get; set; }
        public double SaturationOfConditionalStatements { get; set; }
        public int MaximumNestingLevelOfConditionalOperator { get; set; } = 0;

        public void DeleteUnnecessaryStrings()
        {
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                _code[i] = _code[i].Trim();
                if (_code[i].StartsWith("#"))
                    _code[i] = "";
                else if (_code[i].StartsWith("import "))
                    _code[i] = "";
                else if (_code[i].StartsWith("def "))
                    _code[i] = "";
                else if ((_code[i].StartsWith("else ") || _code[i].StartsWith("else:")) && !_code[i].StartsWith("else if ") && !_code[i].StartsWith("else if(")) 
                {
                    int n = _code[i].IndexOf(':');
                    _code[i] = _code[i].Remove(0, n + 1);
                    _code[i] = _code[i].TrimStart();
                }
            }
        }


        public int CountOperators()
        {
            int count = 0;
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                if (_code[i] != "")
                    count++;
                if (_code[i].StartsWith("if ") || _code[i].StartsWith("elif ") || _code[i].StartsWith("else if ") || _code[i].StartsWith("for ") || 
                    _code[i].StartsWith("while ") || _code[i].StartsWith("if(") || _code[i].StartsWith("elif(") || _code[i].StartsWith("while(") ||
                    _code[i].StartsWith("else if(") || _code[i].StartsWith("else:") || _code[i].StartsWith("else "))
                {
                    string str = _code[i].Clone().ToString();
                    int n = str.IndexOf(':');
                    str = str.Remove(0, n + 1);
                    str = str.TrimStart();
                    if (str != "") 
                        count++;
                }
            }
            return count;
        }


        public int CountConditionalStatementsLabel()
        {
            int count = 0;
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                if (_code[i].StartsWith("if ") || _code[i].StartsWith("elif ") || _code[i].StartsWith("while ") || _code[i].StartsWith("for ") || 
                    _code[i].StartsWith("else if ") || _code[i].StartsWith("if(") || _code[i].StartsWith("elif(") || _code[i].StartsWith("while(") || 
                    _code[i].StartsWith("else if("))
                    count++;
            }
            return count;
        }


        public void CountMaximumNestingLevelOfConditionalOperator()
        {
            for (int i = 0; i < NumberOfLinesInCode; i++)
            {
                int j = 0;
                while (j < _code[i].Length && _code[i][j] == ' ')
                    j++;
                string indent = "".PadRight(j);
                string cloneIndent = "".PadRight(j + 4);
                _code[i] = _code[i].Remove(0, j);
                if (_code[i].StartsWith("if ")|| _code[i].StartsWith("if("))
                {
                    CountNesting(i, 0, cloneIndent);
                    CountSequence(i, 0, indent);
                }
                else if(_code[i].StartsWith("for ") || _code[i].StartsWith("while ") || _code[i].StartsWith("while("))
                    CountNesting(i, 0, cloneIndent);
            }
        }


        public void CountNesting(int i, int level, string indent)
        {
            if (level > MaximumNestingLevelOfConditionalOperator)
                MaximumNestingLevelOfConditionalOperator = level;

            _code[i] = indent.Clone().ToString();
            i++;
            while (i < NumberOfLinesInCode)
            {
                string str = _code[i].Clone().ToString();
                if (str.StartsWith(indent))
                {
                    str = str.Remove(0, indent.Length);
                    string cloneIndent = indent.PadRight(indent.Length + 4);
                    if (str.StartsWith("if ") || str.StartsWith("if("))
                    {
                        CountNesting(i, level + 1, cloneIndent);
                        CountSequence(i, level + 1, indent);
                    }
                    else if (str.StartsWith("for ") || str.StartsWith("while ") || str.StartsWith("while("))
                        CountNesting(i, level + 1, cloneIndent);
                    _code[i] = indent.Clone().ToString();
                    i++;
                }
                else
                    break;
            }
        }


        public void CountSequence(int i, int level, string indent)
        {
            if (level > MaximumNestingLevelOfConditionalOperator)
                MaximumNestingLevelOfConditionalOperator = level;
            _code[i] = indent.Clone().ToString();
            i++;
            while(i < NumberOfLinesInCode)
            {
                string str = _code[i].Clone().ToString();
                if (str.StartsWith(indent))
                {
                    str = str.Remove(0, indent.Length);
                    if (str.StartsWith("else if ") || str.StartsWith("elif ") || str.StartsWith("elif(") || str.StartsWith("else if("))
                    {
                        _code[i] = "";
                        string cloneIndent = indent.Clone().ToString().PadRight(indent.Length + 4);
                        CountNesting(i, level + 1, cloneIndent);
                        CountSequence(i, level + 1, indent);
                        break;
                    }
                    else if (str.StartsWith("else ") || str.StartsWith("else:"))
                    {
                        _code[i] = "";
                        string cloneIndent = indent.Clone().ToString().PadRight(indent.Length + 4);
                        CountNesting(i, level, cloneIndent);
                        break;
                    }
                    else if (str.StartsWith(" "))
                        i++;
                    else
                        break;
                }
                else
                    break;
            }
        }
    }
}
