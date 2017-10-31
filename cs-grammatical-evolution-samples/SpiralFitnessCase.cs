using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.SpiralClassification
{
    using TreeGP.Core.ProblemModels;

    public class SpiralFitnessCase : IGPFitnessCase
    {
        private double mX;
        private double mY;
        private int mLabel;
        private int mComputedLabel;

        public int ComputedLabel
        {
            get { return mComputedLabel; }
        }

        public int Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        public double X
        {
            get { return mX; }
            set { mX = value; }
        }

        public double Y
        {
            get { return mY; }
            set { mY = value; }
        }

        public void StoreOutput(object result, int program_index)
        {
            double double_result = Convert.ToDouble(result);
            if (double_result < 0.5)
            {
                mComputedLabel = -1;
            }
            else
            {
                mComputedLabel = 1;
            }
        }

        public bool QueryInput(string variable_name, out object input)
        {
            input = 0;
            if (variable_name == "X")
            {
                input = mX;
                return true;
            }
            else if (variable_name=="Y")
            {
                input = mY;
                return true;
            }
            return false;
        }


        public int GetInputCount()
        {
            return 2;
        }


    }
}
