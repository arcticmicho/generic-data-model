using DataModelParty.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelParty.DataParsers
{
    public abstract class DataWriterResult
    {
    }

    public abstract class DataWriter
    {
        public abstract void AddDataObject(DataObject doWriter);
    }

    public abstract class DataObjectWriter
    {
        public abstract bool SetArrayField<T>(string fieldName, IList<T> list);

        public abstract bool SetField<T>(string fieldName, T field);

        public abstract T GetWriterOuput<T>() where T : DataWriterResult;
    }
}
