﻿// Copyright 2012 Peter Henell

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.ObjectModel;
using SQLDataProducer.Entities.Generators.Collections;
using SQLDataProducer.Entities.DatabaseEntities;

namespace SQLDataProducer.Entities.Generators
{
    public partial class Generator
    {
        public static readonly string GENERATOR_ValueFromOtherColumn = "Value from other Column";
        public static readonly string GENERATOR_RandomFOREIGNKEYValueEAGER = "Random FOREIGN KEY Value (EAGER)";
        public static readonly string GENERATOR_SequentialFOREIGNKEYValueEAGER = "Sequential FOREIGN KEY Value (EAGER)";
        public static readonly string GENERATOR_RandomInt = "Random Int";
        public static readonly string GENERATOR_CountingUpInteger = "Counting up Integer";
        public static readonly string GENERATOR_IdentityFromPreviousItem = "Identity from previous item";
        public static readonly string GENERATOR_StaticNumber = "Static Number";
        public static readonly string GENERATOR_NormallyDistributedRandomNumbers = "Normally Distributed Random Numbers";
        public static readonly string GENERATOR_ExponentialRandomNumbers = "Exponential Random Numbers";
        public static readonly string GENERATOR_WeibullRandomNumbers = "Weibull Random Numbers";
        public static readonly string GENERATOR_LaplaceRandomNumbers = "Laplace Random Numbers";


        static long _integerStartValue = 1;
        public static long IntegerStartValue
        {
            get
            {
                return _integerStartValue;
            }
            set
            {
                if (_integerStartValue != value)
                {
                    _integerStartValue = value;
                }
            }
        }

        public static ObservableCollection<Generator> GetGeneratorsForInt()
        {
            int maxValue = int.MaxValue;
            int minValue = 0;

            return CreateIntGeneratorsWithSettings(maxValue, minValue);
        }
        public static ObservableCollection<Generator> GetGeneratorsForSmallInt()
        {
            int maxValue = short.MaxValue;
            int minValue = 0;

            return CreateIntGeneratorsWithSettings(maxValue, minValue);
        }
        public static ObservableCollection<Generator> GetGeneratorsForBigInt()
        {
            long maxValue = int.MaxValue;
            long minValue = 0;

            return CreateIntGeneratorsWithSettings(maxValue, minValue);
        }
        public static ObservableCollection<Generator> GetGeneratorsForTinyInt()
        {
            long maxValue = byte.MaxValue;
            long minValue = 0;

            return CreateIntGeneratorsWithSettings(maxValue, minValue);
        }
        public static ObservableCollection<Generator> GetGeneratorsForBit()
        {
            long maxValue = 1;
            long minValue = 0;

            return CreateIntGeneratorsWithSettings(maxValue, minValue);
        }



        private static ObservableCollection<Generator> CreateIntGeneratorsWithSettings(long maxValue, long minValue)
        {
            ObservableCollection<Generator> valueGenerators = new ObservableCollection<Generator>();
            valueGenerators.Add(CreateRandomIntGenerator(minValue, maxValue));
            valueGenerators.Add(CreateIntUpCounter(minValue, maxValue));
            valueGenerators.Add(CreateIdentityFromExecutionItem());
            valueGenerators.Add(CreateQueryGenerator());
            valueGenerators.Add(CreateStaticNumberGenerator());
            valueGenerators.Add(CreateExponentialRandomNumbersGenerator());
            valueGenerators.Add(CreateNormallyDistributedRandomGenerator());
            valueGenerators.Add(CreateWeibullRandomNumbersGenerator());
            valueGenerators.Add(CreateLaplaceRandomNumbersGenerator());
            valueGenerators.Add(CreateValueFromOtherColumnGenerator());

            return valueGenerators;
        }

        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        public static Generator CreateRandomForeignKeyGenerator(ObservableCollection<string> fkkeys)
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            GeneratorParameter foreignParam = new GeneratorParameter("Keys", fkkeys, false);
            paramss.Add(foreignParam);
            Generator gen = new Generator(GENERATOR_RandomFOREIGNKEYValueEAGER, (n, p) =>
            {
                ObservableCollection<string> keys = (ObservableCollection<string>)GetParameterByName(p, "Keys");
                if (keys == null || keys.Count == 0)
                    throw new ArgumentException("There are no foreign keys in the table that this column references");
                
                return Wrap(keys[RandomSupplier.Instance.GetNextInt() % keys.Count]);
            }
                , paramss);
            return gen;
        }

        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        public static Generator CreateSequentialForeignKeyGenerator(ObservableCollection<string> fkkeys)
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            GeneratorParameter foreignParam = new GeneratorParameter("Keys", fkkeys, false);
            GeneratorParameter startIndex = new GeneratorParameter("Start Index", 1);
            GeneratorParameter maxIndex = new GeneratorParameter("Max Index", 1000);
            
            paramss.Add(foreignParam);
            paramss.Add(startIndex);
            paramss.Add(maxIndex);

            Generator gen = new Generator(GENERATOR_SequentialFOREIGNKEYValueEAGER, (n, p) =>
            {
                ObservableCollection<string> keys = (ObservableCollection<string>)GetParameterByName(p, "Keys");
                if (keys == null || keys.Count == 0)
                    throw new ArgumentException("There are no foreign keys in the table that this column references");

                int si = int.Parse(GetParameterByName(p, "Start Index").ToString());
                int mi = int.Parse(GetParameterByName(p, "Max Index").ToString());
                if (mi > fkkeys.Count)
                {
                    mi = fkkeys.Count;
                }
                return Wrap(keys[n.LongToInt() % keys.Count]);
            }
                , paramss);
            return gen;
        }

        // Todo: Cannot implemtent this as the generator does not know anything about the FK
        //public static GeneratorBase CreateSequentialForeignKeyGeneratorLazy()
        //{
        //    GeneratorParameterCollection paramss = new GeneratorParameterCollection();
            
        //    GeneratorBase gen = new GeneratorBase("Random FOREIGN KEY Value (LAZY)", (n, p) =>
        //    {
        //        return keys[n % keys.Count];
        //    }
        //        , paramss);
        //    return gen;
        //}

        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateRandomIntGenerator(long min, long max)
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            paramss.Add(new GeneratorParameter("MinValue", min));
            paramss.Add(new GeneratorParameter("MaxValue", max));

            Generator gen = new Generator(GENERATOR_RandomInt, (n, p) =>
                {
                    long maxValue = long.Parse(GetParameterByName(p, "MaxValue").ToString());
                    long minValue = long.Parse(GetParameterByName(p, "MinValue").ToString());
                   
                    return (RandomSupplier.Instance.GetNextInt() % maxValue) + minValue;;
                }
                , paramss);
            return gen;
        }


        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateIntUpCounter(long min, long max)
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            paramss.Add(new GeneratorParameter("MinValue", min));
            paramss.Add(new GeneratorParameter("MaxValue", max));

            Generator gen = new Generator(GENERATOR_CountingUpInteger, (n, p) =>
            {
                long maxValue = long.Parse(GetParameterByName(p, "MaxValue").ToString());
                long minValue = long.Parse(GetParameterByName(p, "MinValue").ToString());
                return (n - 1 + minValue) % maxValue;
            }
                , paramss);
            return gen;
        }


        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateIdentityFromExecutionItem()
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();
            paramss.Add(new GeneratorParameter("Item Number#", 1));

            Generator gen = new Generator(GENERATOR_IdentityFromPreviousItem, (n, p) =>
            {
                long value = long.Parse(GetParameterByName(p, "Item Number#").ToString());

                return string.Format("@i{0}_identity", value);
            }
                , paramss);
            return gen;
        }

        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateStaticNumberGenerator()
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            paramss.Add(new GeneratorParameter("Number", 0));

            Generator gen = new Generator(GENERATOR_StaticNumber, (n, p) =>
            {
                long value = long.Parse(GetParameterByName(p, "Number").ToString());

                return value;
            }
                , paramss);
            return gen;
        }

        /// <summary>
        /// http://www.sqlservercentral.com/articles/SQL+Uniform+Random+Numbers/91103/
        /// </summary>
        /// <returns></returns>
        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateNormallyDistributedRandomGenerator()
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            paramss.Add(new GeneratorParameter("Mean", 1.1));
            paramss.Add(new GeneratorParameter("StDev", 1.1));

            Generator gen = new Generator(GENERATOR_NormallyDistributedRandomNumbers, (n, p) =>
            {
                double Mean = double.Parse(GetParameterByName(p, "Mean").ToString());
                double StDev = double.Parse(GetParameterByName(p, "StDev").ToString());
                double URN1 = RandomSupplier.Instance.GetNextDouble();
                double URN2 = RandomSupplier.Instance.GetNextDouble();

                return (StDev * Math.Sqrt(-2 * Math.Log(URN1)) * Math.Cos(2 * Math.Acos(-1.0) * URN2)) + Mean;
            }
                , paramss);
            return gen;
        }

        /// <summary>
        /// http://www.sqlservercentral.com/articles/SQL+Uniform+Random+Numbers/91103/
        /// </summary>
        /// <returns></returns>
        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateExponentialRandomNumbersGenerator()
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            paramss.Add(new GeneratorParameter("Lambda", 1.1));

            Generator gen = new Generator(GENERATOR_ExponentialRandomNumbers, (n, p) =>
            {
                double Lambda = double.Parse(GetParameterByName(p, "Lambda").ToString());
                double URN1 = RandomSupplier.Instance.GetNextDouble();

                 //-LOG(@URN)/@Lambda
                return -1.0 * (Math.Log(URN1) / Lambda);
            }
                , paramss);
            return gen;
        }

        /// <summary>
        /// http://www.sqlservercentral.com/articles/SQL+Uniform+Random+Numbers/91103/
        /// </summary>
        /// <returns></returns>
        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateWeibullRandomNumbersGenerator()
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            paramss.Add(new GeneratorParameter("Alpha", 1.1));
            paramss.Add(new GeneratorParameter("Beta", 1.1));

            Generator gen = new Generator(GENERATOR_WeibullRandomNumbers, (n, p) =>
            {
                double Alpha = double.Parse(GetParameterByName(p, "Alpha").ToString());
                double Beta = double.Parse(GetParameterByName(p, "Beta").ToString());
                double URN1 = RandomSupplier.Instance.GetNextDouble();

                //RETURN POWER((-1. / @Alpha) * LOG(1. - @URN), 1./@Beta)
                return Math.Pow((-1.0 / Alpha) * Math.Log(1.0 - URN1), 1.0 / Beta);
            }
                , paramss);
            return gen;
        }

        /// <summary>
        /// http://www.sqlservercentral.com/articles/SQL+Uniform+Random+Numbers/91103/
        /// </summary>
        /// <returns></returns>
        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateLaplaceRandomNumbersGenerator()
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            paramss.Add(new GeneratorParameter("u", 1.1));
            paramss.Add(new GeneratorParameter("b", 1.1));

            Generator gen = new Generator(GENERATOR_LaplaceRandomNumbers, (n, p) =>
            {
                double u = double.Parse(GetParameterByName(p, "u").ToString());
                double b = double.Parse(GetParameterByName(p, "b").ToString());
                double URN1 = RandomSupplier.Instance.GetNextDouble();

                int s = 0;
                if (0 < URN1 - 0.5)
                    s = 1;
                if (0 > URN1 - 0.5)
                    s = -1;

                return u - b * Math.Log(1 - 2 * Math.Abs(URN1 - 0.5)) * s;
                //        RETURN @u - @b * LOG(1 - 2 * ABS(@URN - 0.5)) *
                //CASE WHEN 0 < @URN - 0.5 THEN 1 
                //     WHEN 0 > @URN - 0.5 THEN -1 
                //     ELSE 0 
                //END

            }
                , paramss);
            return gen;
        }

        [GeneratorMetaData(Generators.GeneratorMetaDataAttribute.GeneratorType.Integer)]
        private static Generator CreateValueFromOtherColumnGenerator()
        {
            GeneratorParameterCollection paramss = new GeneratorParameterCollection();

            paramss.Add(new GeneratorParameter("Referenced Column", null));
            paramss.Add(new GeneratorParameter("Referenced value plus", 0));

            Generator gen = new Generator(GENERATOR_ValueFromOtherColumn, (n, p) =>
            {
                long plus = long.Parse(GetParameterByName(p, "Referenced value plus").ToString());
                ColumnEntity otherColumn = GetParameterByName(p, "Referenced Column") as ColumnEntity;

                if (otherColumn != null && otherColumn.PreviouslyGeneratedValue != null)
                {
                    long a;
                    if (long.TryParse(otherColumn.PreviouslyGeneratedValue.ToString(), out a))
                        return a + plus;
                    else
                        return otherColumn.PreviouslyGeneratedValue;
                }


                return Wrap("NULL");
            }
                , paramss);
            return gen;
        }


        //public static object DownCounter(int n, object param)
        //{
        //    return 0 - n;
        //}
    }
}
