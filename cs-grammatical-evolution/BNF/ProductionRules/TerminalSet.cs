using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.BNF.ProductionRules
{
    using TreeGP.ComponentModels;
    using TreeGP.ComponentModels.Operators;
    public class TerminalSet
    {
        public Dictionary<string, TGPPrimitive> mTerminals = new Dictionary<string, TGPPrimitive>();
        public void AddTerminal(TGPPrimitive entity)
        {
            mTerminals[entity.Symbol] = entity;
        }

        public TGPPrimitive FindNodeEntityByTerminal(string symbol)
        {
            if (!mTerminals.ContainsKey(symbol))
            {
                AddTerminal(new TGPOperator_Default(symbol));
            }
            return mTerminals[symbol];
        }

        internal void RemoveAllTerminals()
        {
            mTerminals.Clear();
        }
    }
}
