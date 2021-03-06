﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SQLDataProducer.Entities.DataConsumers
{
    public static class PluginLoader
    {
        public static List<DataConsumerPluginWrapper> LoadPluginsFromFolder(string folder)
        {
            var list = new List<DataConsumerPluginWrapper>();

            var types = LoadTypesFromFolder(folder);
            foreach (Type type in types)
            {
                var meta = GetMetaDataOfType(type);
                // TODO: LOG this failure so that the developers can know why their plugin does not load.
                if (null == meta)
                    continue;

                var consumerName = meta.ConsumerName;
                var optionTemplate = meta.OptionsTemplate;

                DataConsumerPluginWrapper plug = new DataConsumerPluginWrapper(consumerName, type, optionTemplate);

                list.Add(plug);
            }
            return list;
        }

        public static ConsumerMetaDataAttribute GetMetaDataOfType(Type type)
        {
            return type.GetCustomAttributes(typeof(ConsumerMetaDataAttribute), true)
                .FirstOrDefault() as ConsumerMetaDataAttribute;
        }

        private static List<Type> LoadTypesFromFolder(string folder)
        {
            var pluginTypes = new List<Type>();
            var files = System.IO.Directory.GetFiles(folder, "*.dll");
            foreach (var file in files)
            {
                try
                {
                    var typesInFile = LoadIDataConsumerTypes(file);
                    foreach (var t in typesInFile)
                    {
                        pluginTypes.Add(t);
                    }
                }
                catch (Exception)
                {
                    // TODO: Log the error so that the developer of the plugin will know that there was some problem loading it.
                }
                
            }

            return pluginTypes;
        }

        private static IEnumerable<Type> LoadIDataConsumerTypes(string fileName)
        {
            Assembly asm = Assembly.LoadFrom(fileName);
            Type[] types = asm.GetTypes();
            
            foreach (Type thisType in types)
            {
                var interfacesOfTheType = thisType.GetInterfaces();
                
                if (interfacesOfTheType.Any(x => 
                    x.Name == typeof(IDataConsumer).Name && 
                    x.Namespace == typeof(IDataConsumer).Namespace))
                {
                    yield return thisType;   
                }
            }
           
        }

    }
}
