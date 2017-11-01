using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeGP.ComponentModels;
using System.Xml;
using System.IO;

namespace GrammaticalEvolution.ComponentModels
{
    public class GEConfig : TGPConfig
    {
        private int mMaxParseLengthBeforeTermination = 40;
        private int mChromosomeValueUpperBound = 30;
        private int mChromosomeLength = 10;

        public int ChromosomeLength
        {
            get { return mChromosomeLength; }
            set { mChromosomeLength = value; }
        }

        public int MaxParseLengthBeforeTermination
        {
            get { return mMaxParseLengthBeforeTermination; }
            set { mMaxParseLengthBeforeTermination = value; }
        }

        public int ChromosomeValueUpperBound
        {
            get { return mChromosomeValueUpperBound; }
            set { mChromosomeValueUpperBound = value; }
        }

        public override string GetScript(string p)
        {
            if (mScripts.ContainsKey(p))
            {
                string scriptPath = mScripts[p];
                if (!File.Exists(scriptPath))
                {
                    DirectoryInfo parentDir = Directory.GetParent(scriptPath);
                    if (!parentDir.Exists)
                    {
                        parentDir.Create();
                    }
                    if (p == ScriptNames.CrossoverInstructionFactory)
                    {
                        File.WriteAllText(scriptPath, Properties.Resources.CrossoverInstructionFactory);
                    }
                    else if (p == ScriptNames.MutationInstructionFactory)
                    {
                        File.WriteAllText(scriptPath, Properties.Resources.MutationInstructionFactory);
                    }
                    else if (p == ScriptNames.PopInitInstructionFactory)
                    {
                        File.WriteAllText(scriptPath, Properties.Resources.PopInitInstructionFactory);
                    }
                    else if (p == ScriptNames.ReproductionSelectionInstructionFactory)
                    {
                        File.WriteAllText(scriptPath, Properties.Resources.ReproductionSelectionInstructionFactory);
                    }
                    else if (p == ScriptNames.SurvivalInstructionFactory)
                    {
                        File.WriteAllText(scriptPath, Properties.Resources.SurvivalInstructionFactory);
                    }

                }

            }
            return null;
        }

        public GEConfig(string filename)
            : base()
        {
            if(!File.Exists(filename))
            {
                File.WriteAllText(filename, Properties.Resources.GEConfig);
            }
            base.Load(filename);

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement doc_root = doc.DocumentElement;

            foreach (XmlElement xml_level1 in doc_root.ChildNodes)
            {
                if (xml_level1.Name == "parameters")
                {
                    foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
                    {
                        if (xml_level2.Name == "param")
                        {
                            string attrname = xml_level2.Attributes["name"].Value;
                            string attrvalue = xml_level2.Attributes["value"].Value;
                            if (attrname == "MaxParseLengthBeforeTermination")
                            {
                                int value = 0;
                                int.TryParse(attrvalue, out value);
                                mMaxParseLengthBeforeTermination = value;
                            }
                            else if (attrname == "ChromosomeValueUpperBound")
                            {
                                int value = 0;
                                int.TryParse(attrvalue, out value);
                                mChromosomeValueUpperBound = value;
                            }
                            else if (attrname == "ChromosomeLength")
                            {
                                int value = 0;
                                int.TryParse(attrvalue, out value);
                                mChromosomeLength = value;
                            }
                        }
                    }
                }
            }
        }
    }
}
