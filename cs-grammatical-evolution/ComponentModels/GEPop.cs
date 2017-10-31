using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.ComponentModels
{
    using TreeGP.ComponentModels;
    using BNF.ProductionRules;

    using TreeGP.Distribution;

    using GrammaticalEvolution.AlgorithmModels.PopInit;
    using GrammaticalEvolution.AlgorithmModels.Mutation;
    using GrammaticalEvolution.AlgorithmModels.Crossover;

    using NCalc;
    using TreeGP.Core.ProblemModels;
    using TreeGP.Core.ComponentModels;
    using TreeGP.Core.AlgorithmModels.PopInit;
    using TreeGP.Core.AlgorithmModels.Crossover;
    using TreeGP.Core.AlgorithmModels.Mutation;
    

    public class GEPop<S> : TGPPop<S>
        where S : GESolution, new()
    {
        private string mStartingSymbol = "<expr>";
        private ProductionRuleSet mRuleSet = new ProductionRuleSet();
        private TerminalSet mTerminalSet = new TerminalSet();

        public ProductionRuleSet GERuleSet
        {
            get { return mRuleSet; }
        }

        public override void Evolve(EvolutionProgressReportHandler report_handler = null)
        {
            this.MuPlusLambdaEvolve(report_handler);
            mCurrentGeneration++;

            Analyze();
        }

        public delegate void EvaluateFunctionHandle(string name, FunctionArgs args);
        public event EvaluateFunctionHandle EvaluateFunction;

        public GEPop(GEConfig config)
            : base(config)
        {

        }

        public override ISolution CreateSolution()
        {
            return mSolutionFactory.Create(this, mTreeCount);
        }

        private GEConfig GEConfig
        {
            get { return (GEConfig)mConfig; }
        }

        public int MaxParseLengthBeforeTermination
        {
            get { return GEConfig.MaxParseLengthBeforeTermination; }
            set { GEConfig.MaxParseLengthBeforeTermination = value; }
        }

        public int ChromosomeValueUpperBound
        {
            set { GEConfig.ChromosomeValueUpperBound = value; }
            get { return GEConfig.ChromosomeValueUpperBound; }
        }

        public int ChromosomeLength
        {
            get { return GEConfig.ChromosomeLength; }
            set { GEConfig.ChromosomeLength = value; }
        }

      
        
        public override object CreateProgram()
        {
            GEProgram program = new GEProgram(
                mOperatorSet, mVariableSet, mConstantSet, mPrimitiveSet,
                mRuleSet, mTerminalSet,
                mStartingSymbol,
                MaxParseLengthBeforeTermination, ChromosomeValueUpperBound,
                this._EvaluateFunction);
            return program;
        }

        protected override PopInitInstructionFactory<IGPPop, S> CreatePopInitInstructionFactory(string filename)
        {
            GEPopInitInstructionFactory<IGPPop, S> factory = new GEPopInitInstructionFactory<IGPPop, S>(filename);
            factory.ChromosomeLengthRequested += () =>
                {
                    return GEConfig.ChromosomeLength;
                };
            return factory;
        }

        protected override MutationInstructionFactory<IGPPop, S> CreateMutationInstructionFactory(string filename)
        {
            return new GEMutationInstructionFactory<IGPPop, S>(filename);
        }

        protected override CrossoverInstructionFactory<IGPPop, S> CreateCrossoverInstructionFactory(string filename)
        {
            return new GECrossoverInstructionFactory<IGPPop, S>(filename);
        }

        private void AddProductionRuleTerminal(TGPPrimitive entity)
        {
            mTerminalSet.AddTerminal(entity);
        }

        public void BuildTerminalSet()
        {
            mTerminalSet.RemoveAllTerminals();
            int op_count=mOperatorSet.OperatorCount;
            for (int i = 0; i < op_count; ++i)
            {
                mTerminalSet.AddTerminal(mOperatorSet.FindOperatorByIndex(i));
            }
            List<string> variable_names = mVariableSet.TerminalNames;
            foreach(string variable_name in variable_names)
            {
                mTerminalSet.AddTerminal(mVariableSet.FindTerminalBySymbol(variable_name));
            }
            int constant_count = mConstantSet.TerminalCount;
            for (int i = 0; i < constant_count; ++i)
            {
                mTerminalSet.AddTerminal(mConstantSet.FindTerminalByIndex(i));
            }
        }

        public string StartingSymbol
        {
            get { return mStartingSymbol; }
            set { mStartingSymbol = value; }
        }

        protected void _EvaluateFunction(string name, FunctionArgs args)
        {
            if (name == "Iflt")
            {
                if (args.Parameters.Length == 4)
                {
                    double param1 = Convert.ToDouble(args.Parameters[0].Evaluate());
                    double param2 = Convert.ToDouble(args.Parameters[1].Evaluate());
                    if (param1 < param2)
                    {
                        args.Result = args.Parameters[2].Evaluate();
                    }
                    else
                    {
                        args.Result = args.Parameters[3].Evaluate();
                    }
                }
                
            }
            else if (name == "Ifgt")
            {
                if (args.Parameters.Length == 4)
                {
                    double param1 = Convert.ToDouble(args.Parameters[0].Evaluate());
                    double param2 = Convert.ToDouble(args.Parameters[1].Evaluate());
                    if (param1 > param2)
                    {
                        args.Result = args.Parameters[2].Evaluate();
                    }
                    else
                    {
                        args.Result = args.Parameters[3].Evaluate();
                    }
                }
            }

            if (EvaluateFunction != null)
            {
                EvaluateFunction(name, args);
            }
        }
    }
}
