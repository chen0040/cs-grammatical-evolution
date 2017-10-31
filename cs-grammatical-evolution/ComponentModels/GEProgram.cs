using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.ComponentModels
{
    using TreeGP.ComponentModels;
    using BNF.ProductionRules;
    using TreeGP.Distribution;
    using NCalc;
    using TreeGP.Core.ProblemModels;

    public class GEProgram : TGPProgram
    {
        public List<int> mCodon = new List<int>();
        protected ProductionRuleSet mRuleSet;
        protected TerminalSet mTerminalSet;
        private string mStartingSymbol="<expr>";
        protected int mMaxParseLengthBeforeTermination;
        protected int mChromosomeValueUpperBound;

        public GENode.EvaluateFunctionHandle mEvaluateFunction;

        public GEProgram(TGPOperatorSet os, TGPVariableSet vs, TGPConstantSet cs, List<KeyValuePair<TGPPrimitive, double>> ps, ProductionRuleSet rs, TerminalSet ts, string starting_symbol, int max_length_parsed, int value_upper_bound, GENode.EvaluateFunctionHandle evaluate_handler)
            : base(os, vs, cs, ps)
        {
            mRuleSet = rs;
            mStartingSymbol = starting_symbol;
            mTerminalSet = ts;
            mMaxParseLengthBeforeTermination = max_length_parsed;
            mChromosomeValueUpperBound = value_upper_bound;
            mEvaluateFunction = evaluate_handler;
        }

        public List<int> Codon
        {
            get { return mCodon; }
        }


        public override TGPProgram Clone()
        {
            GEProgram program = new GEProgram(mOperatorSet, mVariableSet, mConstantSet, mPrimitiveSet, mRuleSet, mTerminalSet, mStartingSymbol, mMaxParseLengthBeforeTermination, mChromosomeValueUpperBound, mEvaluateFunction);
            program.Copy(this);
            return program;
        }

        public GEProgram CloneWithoutGPTree()
        {
            GEProgram clone = new GEProgram(mOperatorSet, mVariableSet, mConstantSet, mPrimitiveSet, mRuleSet, mTerminalSet, mStartingSymbol, mMaxParseLengthBeforeTermination, mChromosomeValueUpperBound, mEvaluateFunction);

            clone.mCodon = new List<int>();
            int chromosome_length = mCodon.Count;
            for (int i = 0; i < chromosome_length; ++i)
            {
                clone.mCodon.Add(mCodon[i]);
            }

            clone.mDepth = mDepth;
            clone.mLength = mLength;

            
            return clone;
        }

        public override void Copy(TGPProgram rhs)
        {
            base.Copy(rhs);

            GEProgram rhs_=(GEProgram)rhs;
            mCodon = new List<int>();
            int chromosome_length=rhs_.mCodon.Count;
            for (int i = 0; i < chromosome_length; ++i)
            {
                mCodon.Add(rhs_.mCodon[i]);
            }
        }

        public ProductionRule FindGERuleByIndex(string symbol, int index)
        {
            return mRuleSet.FindRuleByIndex(symbol, index);
        }

        public int FindTerminationGERuleCount(string symbol)
        {
            return mRuleSet.FindTerminationRuleCount(symbol);
        }

        public ProductionRule FindTerminationGERule(string symbol, int index)
        {
            return mRuleSet.FindTerminationRule(symbol, index);
        }

        public int FindGERuleCount(string symbol)
        {
            return mRuleSet.FindRuleCount(symbol);
        }

        public bool IsGETerminal(string symbol)
        {
            return mRuleSet.IsTerminal(symbol);
        }

        public TGPPrimitive FindNodeEntityByProductionRuleTerminal(string symbol)
        {
            return mTerminalSet.FindNodeEntityByTerminal(symbol);
        }

        public virtual void BuildGPTree()
        {
            int codon_length=mCodon.Count;

            Stack<GENode> mParserStack = new Stack<GENode>();

            GENode node = new GENode(mStartingSymbol);
            mRootNode = node;
            mParserStack.Push(node);

            int length = 0;
            while (mParserStack.Count > 0)
            {
                node = mParserStack.Pop();

                if (IsGETerminal(node.Symbol))
                {
                    TGPPrimitive terminal=FindNodeEntityByProductionRuleTerminal(node.Symbol);
                    node.Primitive = terminal;
                }
                else
                {
                    int rule_count = FindGERuleCount(node.Symbol);

                    int rule_index = 0;
                    if (rule_count > 1)
                    {
                        int pointer_index = (length) % codon_length;

                        int genetic_code = (int)mCodon[pointer_index];
                        rule_index = genetic_code % rule_count;

                        length++;
                    }
                   
                    ProductionRule rule = FindGERuleByIndex(node.Symbol, rule_index);

                    if (length > mMaxParseLengthBeforeTermination)
                    {
                        rule_count=FindTerminationGERuleCount(node.Symbol);

                        rule_index = 0;

                        if (rule_count > 1)
                        {
                            int pointer_index = (length-1) % codon_length;

                            int genetic_code = mCodon[pointer_index];
                            rule_index = genetic_code % rule_count;
                        }

                        rule = FindTerminationGERule(node.Symbol, rule_index);
                    }

                    node.Rule = rule;

                    for (int i = 0; i < rule.RValueCount; ++i )
                    {
                        GENode child_node = new GENode(rule.FindRValueByIndex(i));
                        node.AddChild(child_node);
                    }
                    for (int i = node.ChildCount - 1; i >= 0; i-- )
                    {
                        GENode child_node = (GENode)node.FindChildByIndex(i);
                        mParserStack.Push(child_node);
                    }   
                }
            }

            CalcDepth();
            CalcLength();
        }




        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int chromosome_length = mCodon.Count;
            sb.Append("[");
            for (int i = 0; i < chromosome_length; ++i)
            {
                if (i == 0)
                {
                    sb.AppendFormat("{0}", mCodon[i]);
                }
                else
                {
                    sb.AppendFormat(", {0}", mCodon[i]);
                }
            }
            sb.Append("]");
            sb.AppendFormat("\n{0}", base.ToString());

            return sb.ToString();
        }

        internal void CreateRandomly(int iMaximumDepthForCreation)
        {
            for (int i = 0; i < iMaximumDepthForCreation; ++i)
            {
                mCodon.Add(DistributionModel.NextInt(mChromosomeValueUpperBound));
            }
        }

        public override object ExecuteOnFitnessCase(IGPFitnessCase fitness_case, params object[] tags)
        {
            List<string> variable_names = mVariableSet.TerminalNames;
            foreach(string variable_name in variable_names)
            {
                object input;
                if (fitness_case.QueryInput(variable_name, out input))
                {
                    mVariableSet.FindTerminalBySymbol(variable_name).Value = input;
                }
                else
                {
                    mVariableSet.FindTerminalBySymbol(variable_name).Value = 0;
                }
            }
            return ((GENode)mRootNode).GEEvaluate(mEvaluateFunction);
        }

        public virtual object Execute(Dictionary<string, object> variables, params object[] tags)
        {
            List<string> variable_names = mVariableSet.TerminalNames;
            foreach (string variable_name in variable_names)
            {
                if (variables.ContainsKey(variable_name))
                {
                    mVariableSet.FindTerminalBySymbol(variable_name).Value = variables[variable_name];
                }
                else
                {
                    mVariableSet.FindTerminalBySymbol(variable_name).Value = 0;
                }
            }
            return mRootNode.Evaluate();
        }


        public int CodonGeneUpperBound
        {
            get { return mChromosomeValueUpperBound; }
        }

        public override List<TGPPrimitive> FlattenPrimitives()
        {
            return new List<TGPPrimitive>();
        }
    }
}
