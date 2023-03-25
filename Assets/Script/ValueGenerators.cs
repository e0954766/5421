using System;
using System.Collections.Generic;
using Accord.Statistics.Distributions.Univariate;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    public abstract class ValueGenerator
    {
        public abstract int Generate(JObject obj);
    }

    public class FixedValueGenerator : ValueGenerator
    {
        public override int Generate(JObject obj)
        {
            int value = obj.Value<int>("value");
            Type valueType = value.GetType();
            return value;
        }
    }

    public class RandomRangeValueGenerator : ValueGenerator
    {
        private static readonly Random rnd = new Random();

        public override int Generate(JObject obj)
        {
            int min = obj.Value<int>("min");
            int max = obj.Value<int>("max");

            return rnd.Next(min, max + 1);

            }
    }

    public class PoissonValueGenerator : ValueGenerator
    {
        private static readonly Random rnd = new Random();

        public override int Generate(JObject obj)
        {
            int lambda = obj.Value<int>("lambda");
            return (int)Math.Round(Poisson(lambda));
        }

        private static double Poisson(int lambda)
        {
            double L = Math.Exp(-lambda);
            double p = 1.0;
            int k = 0;
            do
            {
                k++;
                p *= rnd.NextDouble();
            } while (p > L);
            return k - 1;
        }
    }

    public class NormalValueGenerator : ValueGenerator
    {
        private static readonly NormalDistribution normalDistribution = new NormalDistribution();

        public override int Generate(JObject obj)
        {
            double mean = obj.Value<double>("lambda");
            double std = obj.Value<double>("lambda");
            double continuousValue = normalDistribution.Generate() * std + mean;
            int discreteValue = (int)Math.Round(continuousValue);
            return discreteValue;
        }
    }
}
