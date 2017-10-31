using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.AlgorithmModels.Mutation
{
    using System.Xml;
    
    using TreeGP.Core.ComponentModels;
    using TreeGP.Core.AlgorithmModels.Mutation;

    using GrammaticalEvolution.ComponentModels;
    using TreeGP.AlgorithmModels.Mutation;

    public class GEMutationInstructionFactory<P, S> : TGPMutationInstructionFactory<P, S>
        where S : GESolution
        where P : IGPPop
    {

        public GEMutationInstructionFactory()
        {

        }
        
        public GEMutationInstructionFactory(string filename)
            : base(filename)
        {
           
        }

        protected override MutationInstruction<P, S> LoadInstructionFromXml(string selected_strategy, XmlElement xml)
        {
            if (selected_strategy == "uniform_random")
            {
                return new GEMutationInstruction_UniformRandom<P, S>(xml);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected override MutationInstruction<P, S> LoadDefaultInstruction()
        {
            return new GEMutationInstruction_UniformRandom<P, S>();
        }

        public override MutationInstructionFactory<P, S> Clone()
        {
            GEMutationInstructionFactory<P, S> clone = new GEMutationInstructionFactory<P, S>(mFilename);
            return clone;
        }

    }
}
