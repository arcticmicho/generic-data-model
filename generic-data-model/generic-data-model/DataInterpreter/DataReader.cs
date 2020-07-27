using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelParty.DataInterpreter
{
    public abstract class DataReader
    {
        public abstract List<DataObjectReader> GetDataObjectsReader();
    }

    public abstract class DataObjectReader
    {
        public abstract T GetField<T>(string fieldName);

        public abstract IList<T> GetArrayField<T>(string fieldName);

        public abstract bool ContainsField(string fieldName);
    }

}
