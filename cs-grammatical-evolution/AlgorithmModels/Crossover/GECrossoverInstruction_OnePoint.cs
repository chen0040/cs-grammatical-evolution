using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrammaticalEvolution.AlgorithmModels.Crossover
{
    using System.Xml;
    using TreeGP.Core.ComponentModels;
    using TreeGP.ComponentModels;
    using GrammaticalEvolution.ComponentModels;
    using TreeGP.Core.AlgorithmModels.Crossover;
    using TreeGP.AlgorithmModels.Crossover;
    using TreeGP.Distribution;

    public class GECrossoverInstruction_OnePoint<P, S> : CrossoverInstruction<P, S>
        where S : GESolution
        where P : IGPPop
    {
        public GECrossoverInstruction_OnePoint()
            : base()
        {
           
        }

        public GECrossoverInstruction_OnePoint(XmlElement xml_level1)
            : base()
        {
            
        }

        public override CrossoverInstruction<P, S> Clone()
        {
            GECrossoverInstruction_OnePoint<P, S> clone = new GECrossoverInstruction_OnePoint<P, S>();
            return clone;
        }

        public override List<S> Crossover(P pop, params S[] parents)
        {
            S gp1 = parents[0].CloneWithoutGPTree() as S;
            S gp2 = parents[1].CloneWithoutGPTree() as S;

            if (DistributionModel.GetUniform() < pop.CrossoverRate)
            {
                gp1.OnePointCrossover(gp2);
            }

            List<S> children = new List<S>();
            children.Add(gp1);
            children.Add(gp2);
            return children;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(">> Name: GECrossoverInstruction_OnePoint\n");
            
            return sb.ToString();
        }
    }
}
