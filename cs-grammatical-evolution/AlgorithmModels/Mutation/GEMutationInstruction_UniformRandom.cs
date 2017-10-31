using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.AlgorithmModels.Mutation
{
    using System.Xml;
    using TreeGP.Core.ComponentModels;
    using GrammaticalEvolution.ComponentModels;
    using TreeGP.Core.AlgorithmModels.Mutation;
    using TreeGP.Distribution;

    public class GEMutationInstruction_UniformRandom<P, S> : MutationInstruction<P, S>
        where S : GESolution
        where P : IGPPop
    {
        public GEMutationInstruction_UniformRandom()
        {
            
        }

        public GEMutationInstruction_UniformRandom(XmlElement xml_level1)
            : base()
        {
            
        }

        public override void Mutate(P pop, S program)
        {
            program.UniformMutate();
        }

        public override MutationInstruction<P, S> Clone()
        {
            GEMutationInstruction_UniformRandom<P, S> clone = new GEMutationInstruction_UniformRandom<P, S>();
            return clone;
        }
    }
}
