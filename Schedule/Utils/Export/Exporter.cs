using System;
using System.Linq;
using System.Collections.Generic;

namespace Schedule.Utils.Export
{
    public abstract class Exporter
    {
        private static List<Type> registeredExporters = new List<Type>();
        
        public static void Register(Type exporter)
        {
            registeredExporters.Add(exporter);
        }

        public static IList<Exporter> RegisteredExporters()
        {
            return registeredExporters.Select(e => Activator.CreateInstance(e) as Exporter).ToList();
        }

        public abstract string FormatString();
        public abstract void Save(string path, object source);
        public abstract Type SourceType();
    }
}
