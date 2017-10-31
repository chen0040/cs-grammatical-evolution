using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.AlgorithmModels.PopInit
{
    using System.Xml;
    using TreeGP.Core.ComponentModels;
    using TreeGP.Core.AlgorithmModels.PopInit;
    using GrammaticalEvolution.ComponentModels;
    using TreeGP.Distribution;

    public class GEPopInitInstruction_Random<P, S> : PopInitInstruction<P, S>
        where S : GESolution
        where P : IGPPop
    {
        public delegate int ChromosomeLengthRequestedHandle();
        public event ChromosomeLengthRequestedHandle ChromosomeLengthRequested;

        public GEPopInitInstruction_Random()
        {

        }

        public GEPopInitInstruction_Random(XmlElement xml_level1)
            : base(xml_level1)
        {
            
        }

        public override void Initialize(P pop)
        {
	        int iPopulationSize=pop.PopulationSize;

            int chromosome_length = ChromosomeLengthRequested();

	        for(int i=0; i<iPopulationSize; i++)
	        {
                S program = pop.CreateSolution() as S;

                program.CreateRandomly(chromosome_length);
                pop.AddSolution(program);
	        }
        }

        public override PopInitInstruction<P, S> Clone()
        {
            GEPopInitInstruction_Random<P, S> clone = new GEPopInitInstruction_Random<P, S>();
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(">> Name: GEPopInitInstruction_Random\n");

            return sb.ToString();
        }
    }
}
