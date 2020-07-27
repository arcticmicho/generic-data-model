using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModelParty.DataParsers;
using DataModelParty.DataInterpreter;

namespace DataModelParty.Data
{
    public abstract class DataObject
    {
        protected string m_className;

        protected string m_assemblyName;

        protected Int64 m_id;

        public string ClassName
        {
            get { return m_className; }
        }

        public Int64 ID
        {
            get { return m_id; }
        }

        public DataObject()
        {
            m_className = GetType().FullName;
            m_assemblyName = GetType().AssemblyQualifiedName;
        }

        protected abstract void CustomDeserialize(DataObjectReader reader);
        protected abstract void CustomSerialize(DataObjectWriter writer);

        public void Serialize(DataObjectWriter writer)
        {
            writer.SetField<string>("className", m_className);
            writer.SetField<string>("assemblyName", m_assemblyName);
            CustomSerialize(writer);
        }

        public void Deserialize(DataObjectReader reader)
        {
            m_className = reader.GetField<string>("className");
            m_assemblyName = reader.GetField<string>("assemblyName");
            CustomDeserialize(reader);
        }
    }
}
