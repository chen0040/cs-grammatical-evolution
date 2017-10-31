using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.AlgorithmModels.PopInit
{
    using System.Xml;
    using TreeGP.Core.ComponentModels;
    using GrammaticalEvolution.ComponentModels;
    using TreeGP.AlgorithmModels.PopInit;
    using TreeGP.Core.AlgorithmModels.PopInit;

    public class GEPopInitInstructionFactory<P, S> : TGPPopInitInstructionFactory<P, S>
        where S : GESolution
        where P : IGPPop
    {
        public delegate int ChromosomeLengthRequestedHandle();
        public event ChromosomeLengthRequestedHandle ChromosomeLengthRequested;

        public GEPopInitInstructionFactory()
        {

        }

        public GEPopInitInstructionFactory(string filename)
            : base(filename)
        {
            

        }

        protected override PopInitInstruction<P, S> LoadInstructionFromXml(string selected_strategy, XmlElement xml_level1)
        {
            if (selected_strategy == "random")
            {
                GEPopInitInstruction_Random<P, S> instruction = new GEPopInitInstruction_Random<P, S>(xml_level1);
                instruction.ChromosomeLengthRequested += () =>
                {
                    return ChromosomeLengthRequested();
                };

                return instruction;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected override PopInitInstruction<P, S> LoadDefaultInstruction()
        {
            GEPopInitInstruction_Random<P, S> instruction = new GEPopInitInstruction_Random<P, S>();
            instruction.ChromosomeLengthRequested += () =>
                {
                    return ChromosomeLengthRequested();
                };

            return instruction;
        }

        public override PopInitInstructionFactory<P, S> Clone()
        {
            GEPopInitInstructionFactory<P, S> clone = new GEPopInitInstructionFactory<P, S>(mFilename);
            return clone;
        }
    }
}
