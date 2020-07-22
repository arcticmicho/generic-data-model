using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelParty.DataInterpreter
{
    public abstract class DataInterpreter
    {
        public abstract T GetField<T>(string fieldName);

        public abstract IList<T> GetArrayField<T>(string fieldName);

        public abstract bool SetArrayField<T>(string fieldName, IList<T> list);

        public abstract bool SetField<T>(string fieldName, T field);


    }
}
