using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.BNF.ProductionRules
{
    using System.IO;

    public class ProductionRuleSet
    {
        public Dictionary<string, List<ProductionRule>> mRuleSet = new Dictionary<string, List<ProductionRule>>();
        private Dictionary<string, List<ProductionRule>> mTerminableRuleSet = new Dictionary<string, List<ProductionRule>>();

        public void RemoveAllRules()
        {
            mRuleSet.Clear();
            mTerminableRuleSet.Clear();
        }

        public ProductionRule AddRule(string lValue, params string[] rValue)
        {
            if (!mRuleSet.ContainsKey(lValue))
            {
                mRuleSet[lValue] = new List<ProductionRule>();
            }
            ProductionRule rule = new ProductionRule(lValue, rValue);
            mRuleSet[lValue].Add(rule);
            if (rValue.Length == 1)
            {
                if (!mTerminableRuleSet.ContainsKey(lValue))
                {
                    mTerminableRuleSet[lValue] = new List<ProductionRule>();
                }
                mTerminableRuleSet[lValue].Add(rule);
            }
            return rule;
        }

        public ProductionRule FindRuleByIndex(string lValue, int index)
        {
            return mRuleSet[lValue][index];
        }

        public int FindRuleCount(string lValue)
        {
            return mRuleSet[lValue].Count;
        }

        public int FindTerminationRuleCount(string lValue)
        {
            if (!mTerminableRuleSet.ContainsKey(lValue))
            {
                return 0;
            }
            return mTerminableRuleSet[lValue].Count;
        }

        public bool IsTerminal(string symbol)
        {
            return !mRuleSet.ContainsKey(symbol);
        }

        public ProductionRule FindTerminationRule(string symbol, int index)
        {
            if (mTerminableRuleSet.ContainsKey(symbol))
            {
                return mTerminableRuleSet[symbol][index];
            }
            return null;
        }

        public enum TokenType
        {
            LValue,
            RValue,
            AssignmentOp,
            Seperator,
            NA
        }

        public void LoadGrammar(string filename)
        {
            mTerminableRuleSet.Clear();
            mRuleSet.Clear();

            //Parse Backus Naur Form
            List<string> tokenizers = new List<string>();
            List<TokenType> token_types = new List<TokenType>();
            using(StreamReader reader =new StreamReader(filename))
            {
                string line = null;
                while ((line = reader.ReadLine())!=null)
                {
                    string[] line_elements = line.Split(new char[] { ' ', '\t' });
                    foreach (string line_element in line_elements)
                    {
                        string token=line_element.Trim();
                        if (!string.IsNullOrEmpty(token))
                        {
                            tokenizers.Add(token);
                            token_types.Add(TokenType.NA);
                        }
                    }
                }
            }

            for (int i = 0; i < tokenizers.Count; ++i)
            {
                if(tokenizers[i]=="::=")
                {
                    token_types[i]=TokenType.AssignmentOp;
                    token_types[i - 1] = TokenType.LValue;
                }
                else if (tokenizers[i] == "|")
                {
                    token_types[i] = TokenType.Seperator;
                }
                else
                {
                    token_types[i] = TokenType.RValue;
                }
            }

            

            List<KeyValuePair<string, List<string>>> rules = new List<KeyValuePair<string, List<string>>>();
            for (int i = 0; i < tokenizers.Count; )
            {
                if (token_types[i] == TokenType.LValue)
                {
                    List<string> rValues = new List<string>();
                    string lValue = tokenizers[i];
                    i++;
                    while (i < tokenizers.Count && token_types[i] != TokenType.LValue)
                    {
                        if (token_types[i] == TokenType.RValue)
                        {
                            rValues.Add(tokenizers[i]);
                        }
                        else if (token_types[i] == TokenType.Seperator)
                        {
                            rules.Add(new KeyValuePair<string, List<string>>(lValue, rValues));
                            rValues = new List<string>();
                        }
                        i++;
                    }
                    rules.Add(new KeyValuePair<string, List<string>>(lValue, rValues));
                }
                else
                {
                    i++;
                }
            }

            for (int i = 0; i < rules.Count; ++i)
            {
                AddRule(rules[i].Key, rules[i].Value.ToArray());
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string lValue in mRuleSet.Keys)
            {
                List<ProductionRule> rules = mRuleSet[lValue];
                sb.AppendFormat("{0} ::= ", lValue);
                for(int i=0; i < rules.Count; ++i)
                {
                    if (i != 0)
                    {
                        sb.AppendFormat("\n\t| ");
                    }
                    ProductionRule rule = rules[i];
                    int rValueCount = rule.RValueCount;
                    for (int j = 0; j < rValueCount; ++j)
                    {
                        if (j != 0)
                        {
                            sb.Append(" ");
                        }
                        sb.Append(rule.FindRValueByIndex(j));
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
