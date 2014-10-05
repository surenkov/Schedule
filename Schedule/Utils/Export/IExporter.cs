using Schedule.Controls.Editors;
using System;
using System.Collections.Generic;

namespace Schedule.Utils.Export
{
    interface IExporter
    {
        string FormatString();
        void Save(string path, object source, IEnumerable<Filter> filters);
    }
}
