# cs-grammatical-evolution

Grammatical evolution implemented using C#

# Install

```bash
Install-Package cs-grammatical-evolution -Version 1.0.2
```

# Usage

The sample code belows show how to use the Grammatical Evolution to solve the spiral classification problem:

```cs 
class Program
{
	static DataTable LoadData(string filename)
	{
		DataTable table = new DataTable();
		table.Columns.Add("X");
		table.Columns.Add("Y");
		table.Columns.Add("Label");

		int line_count = 0;
		using (StreamReader reader = new StreamReader(filename))
		{
			string line = reader.ReadLine();
			int.TryParse(line, out line_count);

			while ((line = reader.ReadLine()) != null)
			{
				string[] elements = line.Split(new char[] { '\t' });

				double x, y;
				int label;
				double.TryParse(elements[0].Trim(), out x);
				double.TryParse(elements[1].Trim(), out y);
				int.TryParse(elements[2].Trim(), out label);

				table.Rows.Add(x, y, label);
			}
		}
		return table;
	}

	static void CreateGrammar(GEPop<GESolution> pop)
	{
		pop.GERuleSet.RemoveAllRules();

		//pop.GERuleSet.AddRule("<expr>", "<lbracket>", "<expr>", "<biop>", "<expr>", "<rbracket>");
		pop.GERuleSet.AddRule("<expr>", "<lbracket>", "<expr>", "<biop>", "<expr>", "<rbracket>");
		pop.GERuleSet.AddRule("<expr>", "<pre-op>", "<lbracket>", "<expr>", "<rbracket>");
		//pop.GERuleSet.AddRule("<expr>", "<pre-op-2>", "<lbracket>", "<expr>", "<comma>", "<expr>", "<rbracket>");
		pop.GERuleSet.AddRule("<expr>", "<var>");

		pop.GERuleSet.AddRule("<biop>", "+");
		pop.GERuleSet.AddRule("<biop>", "-");
		pop.GERuleSet.AddRule("<biop>", "/");
		pop.GERuleSet.AddRule("<biop>", "*");

		//pop.GERuleSet.AddRule("<comma>", ",");
		//pop.GERuleSet.AddRule("<pre-op-2>", "Pow");
		pop.GERuleSet.AddRule("<pre-op>", "Sin");
		//pop.GERuleSet.AddRule("<pre-op>", "Tan");
		pop.GERuleSet.AddRule("<pre-op>", "Cos");
		pop.GERuleSet.AddRule("<lbracket>", "(");
		pop.GERuleSet.AddRule("<rbracket>", ")");

		pop.GERuleSet.AddRule("<var>", "X");
		pop.GERuleSet.AddRule("<var>", "Y");
		pop.GERuleSet.AddRule("<var>", "C1");
	}

	static void CreateTerminalSet(GEPop<GESolution> pop)
	{
		pop.OperatorSet.AddOperator("+");
		pop.OperatorSet.AddOperator("-");
		pop.OperatorSet.AddOperator("/");
		pop.OperatorSet.AddOperator("*");
		pop.OperatorSet.AddOperator("Sin");
		pop.OperatorSet.AddOperator("Cos");
		pop.OperatorSet.AddOperator("(");
		pop.OperatorSet.AddOperator(")");
		//pop.OperatorSet.AddOperator("Tan");
		//pop.OperatorSet.AddOperator("Pow");

		pop.ConstantSet.AddConstant("C1", 8);

		pop.VariableSet.AddVariable("X");
		pop.VariableSet.AddVariable("Y");

		pop.BuildTerminalSet();
	}

	static void Main(string[] args)
	{
		DataTable table = LoadData("dataset.txt");

		GEConfig config = new GEConfig("GEConfig.xml");

		GEPop<GESolution> pop = new GEPop<GESolution>(config);

		CreateTerminalSet(pop);

		//User can load BNF grammar from file or create programmatic ally
		pop.GERuleSet.LoadGrammar("grammar.bnf");

		Console.WriteLine(pop.GERuleSet.ToString());

		//CreateGrammar(pop);

		//Xianshun: Say if you implement a operator named "Move(X, Y)",
		//Which returns a value 1 if X > Y otherwise 0
		//use the following code to enable this use-defined operator in the BNF
		/*
		pop.EvaluateFunction += (function_name, function_args) =>
			{
				if (function_name == "Move")
				{
					double X = Convert.ToDouble(function_args.Parameters[0].Evaluate());
					double Y = Convert.ToDouble(function_args.Parameters[1].Evaluate());
					if (X > Y)
					{
						function_args.Result = 1;
					}
					else
					{
						function_args.Result = 0;
					}
				}
			};
		 */

		pop.CreateFitnessCase += (index) =>
		{
			SpiralFitnessCase fitness_case = new SpiralFitnessCase();
			fitness_case.X = double.Parse(table.Rows[index]["X"].ToString());
			fitness_case.Y = double.Parse(table.Rows[index]["Y"].ToString());
			fitness_case.Label = int.Parse(table.Rows[index]["Label"].ToString());

			return fitness_case;
		};

		pop.GetFitnessCaseCount += () =>
		{
			return table.Rows.Count;
		};

		pop.EvaluateObjectiveForSolution += (fitness_cases, s, objective_index) =>
		{
			double fitness = 0;
			for (int i = 0; i < fitness_cases.Count; i++)
			{
				SpiralFitnessCase fitness_case = (SpiralFitnessCase)fitness_cases[i];
				int correct_y = fitness_case.Label;
				int computed_y = fitness_case.ComputedLabel;
				fitness += (correct_y == computed_y) ? 0 : 1;
			}

			return fitness;
		};


		pop.BreedInitialPopulation();


		while (!pop.IsTerminated)
		{
			pop.Evolve();
			Console.WriteLine("Spiral Classification Generation: {0}", pop.CurrentGeneration);
			Console.WriteLine("Global Fitness: {0}\tCurrent Fitness: {1}", pop.GlobalBestProgram.Fitness, pop.FindFittestProgramInCurrentGeneration().Fitness);
			Console.WriteLine("Global Best Solution:\n{0}", pop.GlobalBestProgram);
			//Console.WriteLine("Current Best Solution:\n{0}", pop.FindFittestProgramInCurrentGeneration());
		}

		Console.WriteLine(pop.GlobalBestProgram.ToString());
	}
}
```

The GEConfig.xml and its child configuration files will be automatically generated if they do not exist, otherwise the configuration will be loaded from the existing GEConfig.xml and its child configuration files.
