using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace PSO
{
    class Program
    {
        static Random ran = null;
        const int NUMBER_PARTICLES = 5;
        const int NUMBER_ITERATIONS = 500;
        const int DIM = 1; // dimensions
        const double MINX = -100.0;
        const double MAXX = 100.0;
        const double RAND_MAX = 2147483647.0;
        static List<int> randomList  = new List<int> { 1915998, 2137407328, 298514680, 614427368, 1579399200, 2064477480, 779721781, 838759273, 944442403, 1191832244 };

        static void Main(string[] args)
        {
            var fileHandler = new FileHandler();
            try
            {
                Console.WriteLine("\nBegin PSO demo\n");
                ran = new Random(114);
                //Console.WriteLine("first 100 Random with seed 114\n");
                //var randomList = new List<double>();
                //var randomList32 = new List<double>();
                //var randomList32Less1 = new List<double>();
                for (var i =0; i < 100; i++)
                {
                    //var random = ran.NextDouble();
                    var random32 = ran.Next();
                    //var random32Less1 = random32 / RAND_MAX;
                    //Console.WriteLine($"random {i}: {random}\n");
                    //Console.WriteLine($"random32 {i}: {random32}\n");
                    //Console.WriteLine($"random32 0~1 {i}: {random32 / RAND_MAX}\n");
                    //randomList.Add(random);
                    //randomList32.Add(random32);
                    //randomList32Less1.Add(random32Less1);
                }

                //fileHandler.OutputString(randomList, "randomList");
                //fileHandler.OutputString(randomList32, "randomList32");
                //fileHandler.OutputString(randomList32Less1, "randomList32Less1");



                //double wi = 0.1; // inertia weight
                //double c1i = 0.3; // cognitive weight
                //double c2i = 1.8; // social weight
                var avgResults = new List<AvgResult>();
                for (var wi = 1.0; wi >= 0.1; wi -= 0.1)
                {
                    for (var c1i = 2.0; c1i >= 0.1; c1i -= 0.1)
                    {
                        for (var c2i = 2.0; c2i >= 0.1; c2i -= 0.1)
                        {
                            Console.WriteLine($"\nw = {wi}\tc1 = {c1i}\tc2i = {c2i}\t\n");
                            wi = Math.Round(wi, 1);
                            c1i = Math.Round(c1i, 1);
                            c2i = Math.Round(c2i, 1);
                            var avg50 = PSO(wi, c1i, c2i);
                            avgResults.Add(
                                new AvgResult
                                {
                                    w = wi,
                                    c1 = c1i,
                                    c2 = c2i,
                                    avg = avg50
                                });
                        }
                    }
                }

                Console.WriteLine("\nEnd PSO demonstration\n");
                fileHandler.OutputCsv(avgResults, "pso_result");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error: " + ex.Message);
            }
        } // Main()

        private static double PSO(double w, double c1, double c2)
        {
            List<double> results = new List<double>();
            for (var l = 0; l < 50; l++)
            {
                int iteration = 0;

                Particle[] swarm = new Particle[NUMBER_PARTICLES];
                double[] bestGlobalPosition = new double[DIM];
                double bestGlobalFitness = double.MaxValue;

                //double minV = -1.0 * MAXX;
                //double maxV = MAXX;

                // Initialize all Particle objects
                bestGlobalFitness = InitializeAllParticleObj(swarm, bestGlobalPosition, bestGlobalFitness);

                // Main processing loop
                do
                {
                    for (int i = 0; i < swarm.Length; ++i)
                    {
                        Particle currP = swarm[i];

                        UpdateNewVelocity(bestGlobalPosition, c1, c2, w, currP, out double[] newVelocity);
                        UpdateNewPosition(currP, newVelocity, out double[] newPosition);

                        var newFitness = ObjectiveFunction(newPosition);
                        currP.fitness = newFitness;

                        UpdateGlobalBest(ref bestGlobalPosition, ref bestGlobalFitness, currP, newPosition, newFitness);

                    } // each Particle

                    //Console.WriteLine($"itereation:{iteration}");
                    //Console.WriteLine($"best global position:{bestGlobalPosition[0]}");
                    //Console.WriteLine($"current best fitness:{bestGlobalFitness}");
                    iteration++;
                } while (iteration <= NUMBER_ITERATIONS && bestGlobalFitness != 0);
                results.Add(bestGlobalPosition[0]);

                //Console.WriteLine("\nProcessing complete");
                //Console.WriteLine($"Final best fitness = {bestGlobalFitness.ToString("F4")}({bestGlobalFitness})");
                //Console.WriteLine("Best position/solution:");
                //for (int i = 0; i < bestGlobalPosition.Length; ++i)
                //{
                    //Console.WriteLine($"x{i} = {bestGlobalPosition[i].ToString("F4")}({bestGlobalPosition[i]})");
                //}
                //Console.WriteLine("");
            }
            double sum = 0;
            var avg50 = new List<AvgResult>();
            results.ForEach(r =>
            {
                sum = sum + r;
                avg50.Add(new AvgResult
                {
                    w = 0.1,
                    c1 = 0.3,
                    c2 = 1.8,
                    avg = r
                });
            });
            //Console.WriteLine(sum / 50);
            var fileHandler = new FileHandler();
            fileHandler.OutputCsv(avg50, "avg50");
            return sum/50;
        }

        private static void UpdateGlobalBest(ref double[] bestGlobalPosition, ref double bestGlobalFitness, Particle currP, double[] newPosition, double newFitness)
        {
            if (newFitness < currP.bestFitness)
            {
                newPosition.CopyTo(currP.bestPosition, 0);
                currP.bestFitness = newFitness;
            }
            if (newFitness < bestGlobalFitness)
            {
                newPosition.CopyTo(bestGlobalPosition, 0);
                bestGlobalFitness = newFitness;
            }
        }

        private static void UpdateNewPosition(Particle currP, double[] newVelocity, out double[] newPosition)
        {
            newPosition = new double[DIM];
            for (int j = 0; j < currP.position.Length; ++j)
            {
                newPosition[j] = currP.position[j] + newVelocity[j];
                if (newPosition[j] < MINX)
                    newPosition[j] = MINX;
                else if (newPosition[j] > MAXX)
                    newPosition[j] = MAXX;
            }
            newPosition.CopyTo(currP.position, 0);
        }

        private static void UpdateNewVelocity(double[] bestGlobalPosition, double c1, double c2, double w, Particle currP, out double[] newVelocity)
        {
            newVelocity = new double[DIM];
            double r1, r2; // randomizations
            for (int j = 0; j < currP.velocity.Length; ++j)
            {
                r1 = ran.NextDouble();
                r2 = ran.NextDouble();

                newVelocity[j] = (w * currP.velocity[j]) +
                  (c1 * r1 * (currP.bestPosition[j] - currP.position[j])) +
                  (c2 * r2 * (bestGlobalPosition[j] - currP.position[j]));
                // adjust max and min
                //if (newVelocity[j] < minV)
                //    newVelocity[j] = minV;
                //else if (newVelocity[j] > maxV)
                //    newVelocity[j] = maxV;
            }
            newVelocity.CopyTo(currP.velocity, 0);
        }

        private static double InitializeAllParticleObj(Particle[] swarm, double[] bestGlobalPosition, double bestGlobalFitness)
        {
            Console.WriteLine("{x, v}");
            for (int i = 0; i < swarm.Length; ++i)
            {
                //position
                double[] randomPosition = new double[DIM];
                for (int j = 0; j < randomPosition.Length; ++j)
                {
                    double lo = MINX;
                    double hi = MAXX;
                    randomPosition[j] = (hi - lo) * ran.NextDouble() + lo;
                }
                double fitness = ObjectiveFunction(randomPosition);
                // velocity
                double[] randomVelocity = new double[DIM];
                for (int j = 0; j < randomVelocity.Length; ++j)
                {
                    double lo = -1.0 * Math.Abs(MAXX - MINX);
                    double hi = Math.Abs(MAXX - MINX);
                    randomVelocity[j] = (hi - lo) * ran.NextDouble() + lo;
                }
                swarm[i] = new Particle(randomPosition, fitness, randomVelocity, randomPosition, fitness);

                //update GBest if needs
                if (swarm[i].fitness < bestGlobalFitness)
                {
                    bestGlobalFitness = swarm[i].fitness;
                    swarm[i].position.CopyTo(bestGlobalPosition, 0);
                }
                Console.WriteLine("{"+randomPosition[0]+$" : ran({randomList[i*2]}), "+randomVelocity[0]+$" : ran({randomList[i*2+1]})"+"}");
            }

            return bestGlobalFitness;
        }

        static double ObjectiveFunction(double[] x)
        {
            return Math.Abs(x[0]);
        }
    }// class Program

    public class Particle
    {
        public double[] position;
        public double fitness;
        public double[] velocity;

        public double[] bestPosition;
        public double bestFitness;

        public Particle(double[] position, double fitness,
         double[] velocity, double[] bestPosition, double bestFitness)
        {
            this.position = new double[position.Length];
            position.CopyTo(this.position, 0);
            this.fitness = fitness;
            this.velocity = new double[velocity.Length];
            velocity.CopyTo(this.velocity, 0);
            this.bestPosition = new double[bestPosition.Length];
            bestPosition.CopyTo(this.bestPosition, 0);
            this.bestFitness = bestFitness;
        }

        public override string ToString()
        {
            string s = "";
            s += "==========================\n";
            s += "Position: ";
            for (int i = 0; i < this.position.Length; ++i)
                s += this.position[i].ToString("F2") + " ";
            s += "\n";
            s += "Fitness = " + this.fitness.ToString("F4") + "\n";
            s += "Velocity: ";
            for (int i = 0; i < this.velocity.Length; ++i)
                s += this.velocity[i].ToString("F2") + " ";
            s += "\n";
            s += "Best Position: ";
            for (int i = 0; i < this.bestPosition.Length; ++i)
                s += this.bestPosition[i].ToString("F2") + " ";
            s += "\n";
            s += "Best Fitness = " + this.bestFitness.ToString("F4") + "\n";
            s += "==========================\n";
            return s;
        }
    } // class Particle
}