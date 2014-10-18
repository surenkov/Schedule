using System;

namespace Schedule.Utils.Export
{
    interface IExporter
    {
        string FormatString();
        void Save(string path, object source);
        Type SourceType();
    }
}
