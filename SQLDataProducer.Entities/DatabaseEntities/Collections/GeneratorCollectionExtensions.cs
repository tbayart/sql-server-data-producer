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

using System.Collections.ObjectModel;

namespace SQLDataProducer.Entities.DatabaseEntities.Collections
{
    public static class GeneratorCollectionExtensions
    {
        public static ObservableCollection<Generators.Generator> Clone(this ObservableCollection<Generators.Generator> gens)
        {
            var newGens = new ObservableCollection<Generators.Generator>();
            foreach (var item in gens)
            {
                newGens.Add(item.Clone());
            }
            return newGens;
        }
    }
}