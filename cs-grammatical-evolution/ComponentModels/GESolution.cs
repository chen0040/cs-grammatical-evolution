using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeGP.ComponentModels;
using TreeGP.Distribution;
using TreeGP.Core.ComponentModels;

namespace GrammaticalEvolution.ComponentModels
{
    public class GESolution : TGPSolution
    {
        public GESolution()
        {

        }

        protected GESolution(IGPPop pop, int tree_count)
            : base(pop, tree_count)
        {
            
        }

        public override ISolution Create(IGPPop pop, int tree_count)
        {
            return new GESolution(pop, tree_count);
        }

        public override ISolution Clone()
        {
            GESolution clone = new GESolution(mPop, mTrees.Count);
            clone.Copy(this);
            return clone;
        }

        public void OnePointCrossover(GESolution rhs)
        {
            for (int tindex = 0; tindex < mTrees.Count; ++tindex)
            {
                GEProgram gp1 = (GEProgram)mTrees[tindex];
                GEProgram gp2 = (GEProgram)rhs.mTrees[tindex];

                List<int> codon1 = gp1.Codon;
                List<int> codon2 = gp2.Codon;

                int cut_point_index = 1 + DistributionModel.NextInt(codon1.Count - 2);

                for (int i = cut_point_index; i < codon1.Count; ++i)
                {
                    int temp = codon1[i];
                    codon1[i] = codon2[i];
                    codon2[i] = temp;
                }
            }
           

            TrashFitness();
            rhs.TrashFitness();
        }

        public override void MicroMutate()
        {

        }

        public void UniformMutate()
        {
            for (int tindex = 0; tindex < mTrees.Count; ++tindex)
            {
                GEProgram program = (GEProgram)mTrees[tindex];

                double mutation_rate = mPop.MacroMutationRate;

                int upper_bound = program.CodonGeneUpperBound;

                List<int> codon = program.Codon;
                for (int i = 0; i < codon.Count; ++i)
                {
                    if (DistributionModel.GetUniform() < mutation_rate)
                    {
                        codon[i] = DistributionModel.NextInt(upper_bound);
                    }
                }
            }

            TrashFitness();
        }

        public GESolution CloneWithoutGPTree()
        {
            GESolution clone = new GESolution(mPop, mTrees.Count);

            clone.mTrees.Clear();
            for (int i = 0; i < mTrees.Count; ++i)
            {
                clone.mTrees.Add(((GEProgram)mTrees[i]).CloneWithoutGPTree());
            }


            clone.mFitness = mFitness;
            clone.mIsFitnessValid = mIsFitnessValid;
            clone.mObjectiveValue = mObjectiveValue;

            foreach (string attrname in mAttributes.Keys)
            {
                clone.mAttributes[attrname] = mAttributes[attrname];
            }


            return clone;
        }

        protected virtual void BuildGPTree()
        {
            for (int i = 0; i < mTrees.Count; ++i)
            {
                ((GEProgram)mTrees[i]).BuildGPTree();
            }
        }

        public override void EvaluateFitness()
        {
            BuildGPTree();

            mObjectiveValue = mPop.EvaluateObjective(this, 0);

            if (mPop.IsMaximization)
            {
                mFitness = mObjectiveValue;
            }
            else
            {
                mFitness = -mObjectiveValue;
            }
            mIsFitnessValid = true;
        }

        internal void CreateRandomly(int iMaximumDepthForCreation)
        {
            for (int i = 0; i < mTrees.Count; ++i)
            {
                ((GEProgram)mTrees[i]).CreateRandomly(iMaximumDepthForCreation);
            }

            TrashFitness();
        }


        public virtual List<object> Execute(Dictionary<string, object> variables, params object[] tags)
        {
            List<object> results = new List<object>();
            for (int i = 0; i < mTrees.Count; ++i)
            {
                results.Add(((GEProgram)mTrees[i]).Execute(variables, tags));
            }
            return results;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mTrees.Count; ++i)
            {
                if (i != 0)
                {
                    sb.AppendLine();
                }
                sb.AppendFormat("GE[{0}]:\n {1}", i, mTrees[i]);
            }
            return sb.ToString();
        }
    }
}
