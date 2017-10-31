using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.BNF.ProductionRules
{
    public class ProductionRule
    {
        private string mLValue;
        private List<string> mRValues=new List<string>();

        public ProductionRule(string lValue, params string[] rValues)
        {
            mLValue = lValue;
            for (int i = 0; i < rValues.Length; ++i)
            {
                mRValues.Add(rValues[i]);
            }
        }

        public int RValueCount
        {
            get { return mRValues.Count; }
        }

        public string FindRValueByIndex(int index)
        {
            return mRValues[index];
        }
    }
}
