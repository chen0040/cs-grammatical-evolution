using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.AlgorithmModels.Crossover
{
    using System.Xml;
    using TreeGP.Core.ComponentModels;
    using GrammaticalEvolution.ComponentModels;
    using TreeGP.Core.AlgorithmModels.Crossover;
    using TreeGP.AlgorithmModels.Crossover;

    public class GECrossoverInstructionFactory<P, S> : TGPCrossoverInstructionFactory<P, S>
        where S : GESolution
        where P : IGPPop
    {
        public GECrossoverInstructionFactory()
        {

        }

        public GECrossoverInstructionFactory(string filename)
            : base(filename)
        {
            

        }

        protected override CrossoverInstruction<P, S> CreateInstructionFromXml(string strategy_name, XmlElement xml)
        {
            if (strategy_name == "one_point")
            {
                return new GECrossoverInstruction_OnePoint<P, S>(xml);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected override CrossoverInstruction<P,S>  CreateDefaultInstruction()
        {
 	        return new GECrossoverInstruction_OnePoint<P, S>();
        }

        public override CrossoverInstructionFactory<P, S> Clone()
        {
            GECrossoverInstructionFactory<P, S> clone = new GECrossoverInstructionFactory<P, S>(mFilename);
            return clone;
        }
    }
}
