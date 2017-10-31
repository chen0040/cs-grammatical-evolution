using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.ComponentModels
{
    using BNF.ProductionRules;
    using TreeGP.ComponentModels;
    using NCalc;
    public class GENode : TGPNode
    {
        private string mSymbol;
        private ProductionRule mRule;
        public delegate void EvaluateFunctionHandle(string name, FunctionArgs args);

        public string Symbol { get { return mSymbol; } }
        public ProductionRule Rule
        {
            get { return mRule; }
            set { mRule = value; }
        }

        public GENode(string symbol)
            : base(null)
        {
            mSymbol = symbol;
        }

        // Xianshun:
        // Use of NCalc for math expression evaluation is very expensive at the moment, should consider using
        // other or a self-implemented math expression parser
        public object GEEvaluate(EvaluateFunctionHandle OnEvaluateFunction)
        {
            string expression = null;
            return OnePassEvaluate(out expression, OnEvaluateFunction);

            
            //return BubbleupEvaluate(out expression, OnEvaluateFunction);
        }

        protected object OnePassEvaluate(out string expression, EvaluateFunctionHandle OnEvaluateFunction)
        {
            expression = Expression;
            Expression exp = new Expression(expression);
            exp.EvaluateFunction += (name, args) =>
            {
                OnEvaluateFunction(name, args);
            };
            return exp.Evaluate();
        }

        protected object BubbleupEvaluate(out string expression, EvaluateFunctionHandle OnEvaluateFunction)
        {
            object result = null;
            expression = null;
            if (mChildNodes.Count == 0)
            {
                if (mData.IsTerminal)
                {
                    expression = mData.Value.ToString();
                    result = mData.Value;
                    return result;
                }
                expression = mData.Symbol;
                return result;
            }

            if (mChildNodes.Count == 1)
            {
                return ((GENode)mChildNodes[0]).BubbleupEvaluate(out expression, OnEvaluateFunction);
            }

            StringBuilder sb = new StringBuilder();
            string child_expression=null;
            object child_result;
            for (int i = 0; i < mChildNodes.Count; ++i)
            {
                GENode node=((GENode)mChildNodes[i]);
                child_result = node.BubbleupEvaluate(out child_expression, OnEvaluateFunction);
                object value=child_result;
                if(value==null) value=child_expression;
                if (value is double)
                {
                    double dbl_value = (double)value;
                    if (double.IsPositiveInfinity(dbl_value))
                    {
                        value = TGPProtectedDefinition.Instance.LGP_REG_POSITIVE_INF;
                    }
                    else if (double.IsNegativeInfinity(dbl_value))
                    {
                        value = TGPProtectedDefinition.Instance.LGP_REG_NEGATIVE_INF;
                    }
                    else if (double.IsNaN(dbl_value))
                    {
                        value = 0;
                    }
                }
                
                if (i == 0)
                {
                    sb.AppendFormat("{0}", value);
                }
                else
                {
                    sb.AppendFormat(" {0}", value);
                }
            }

            expression = sb.ToString();
            //Console.WriteLine(expression);
            Expression exp = new Expression(expression);
            exp.EvaluateFunction += (name, args) =>
            {
                OnEvaluateFunction(name, args);
            };
            return exp.Evaluate();
        }

        public override TGPNode Clone()
        {
            GENode clone = new GENode(mSymbol);
            clone.Primitive=mData;
            clone.mParent = mParent;
            foreach (TGPNode child_node in mChildNodes)
            {
                GENode cloned_child = (GENode)child_node.Clone();
                cloned_child.mParent = clone;
                clone.mChildNodes.Add(cloned_child);
            }
            return clone;
        }

        public void AddChild(GENode child_node)
        {
            child_node.Parent = this;
            mChildNodes.Add(child_node);
        }

        public bool IsTerminalNode
        {
            get
            {
                return mChildNodes.Count == 0;
            }
        }

        public string Expression
        {
            get
            {
                if (mChildNodes.Count == 0)
                {
                    if (mData.IsTerminal)
                    {
                        return mData.Value.ToString();
                    }
                    return mData.Symbol;
                }

                if (mChildNodes.Count == 1)
                {
                    return ((GENode)mChildNodes[0]).Expression;
                }

                StringBuilder sb = new StringBuilder();
                //sb.Append("(");
                for (int i = 0; i < mChildNodes.Count; ++i)
                {
                    if (i == 0)
                    {
                        sb.AppendFormat("{0}", ((GENode)mChildNodes[i]).Expression);
                    }
                    else
                    {
                        sb.AppendFormat(" {0}", ((GENode)mChildNodes[i]).Expression);
                    }
                }
                //sb.Append(")");
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            if (mChildNodes.Count == 0)
            {
                if (mData.IsTerminal)
                {
                    return mData.ToString();
                }
                return mData.Symbol;
            }

            if (mChildNodes.Count == 1)
            {
                return mChildNodes[0].ToString();
            }


            StringBuilder sb = new StringBuilder();
            //sb.Append("(");
            for (int i = 0; i < mChildNodes.Count; ++i)
            {
                if (i == 0)
                {
                    sb.AppendFormat("{0}", mChildNodes[i]);
                }
                else
                {
                    sb.AppendFormat(" {0}", mChildNodes[i]);
                }
            }
            //sb.Append(")");
            return sb.ToString();

        }

        public int ChildCount
        {
            get { return mChildNodes.Count; }
        }
    }
}
