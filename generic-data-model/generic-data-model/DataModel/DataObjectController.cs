using System;
using System.Collections.Generic;
using System.Linq;
using DataModelParty.DataInterpreter;
using DataModelParty.DataParsers;

namespace DataModelParty.Data
{
    public class DataObjectSignal<T> where T : DataObject
    {
        private Action<T> m_dataObjectUpdateCallback;

        private Dictionary<Int64, Action<T>> m_dataObjectUpdateCallbackForId;

        public void RegisterSignal(Action<T> callback)
        {
            m_dataObjectUpdateCallback += callback;
        }

        public void UnregisterSignal(Action<T> callback)
        {
            m_dataObjectUpdateCallback -= callback;
        }

        public void RegisterSignalForId(Int64 id, Action<T> callback)
        {
            if (m_dataObjectUpdateCallbackForId.ContainsKey(id))
            {
                m_dataObjectUpdateCallbackForId[id] += callback;
            }
            else
            {
                m_dataObjectUpdateCallbackForId.Add(id, callback);
            }
        }

        public void UnregisterSignalForId(Int64 id, Action<T> callback)
        {
            if (m_dataObjectUpdateCallbackForId.ContainsKey(id))
            {
                m_dataObjectUpdateCallbackForId[id] -= callback;
            }
        }

        public void InvokeCallbacks(T updatedDataObject)
        {
            m_dataObjectUpdateCallback?.Invoke(updatedDataObject);
            if (m_dataObjectUpdateCallbackForId.ContainsKey(updatedDataObject.ID))
            {
                m_dataObjectUpdateCallbackForId[updatedDataObject.ID]?.Invoke(updatedDataObject);
            }
        }

    }

    public class DataObjectsController
    {
        private static DataObjectsController m_instance;

        private Dictionary<Type, Dictionary<Int64, DataObject>> m_dataObjects;
        private Dictionary<Type, DataObjectSignal<DataObject>> m_signals;

        public DataObjectsController()
        {
            m_dataObjects = new Dictionary<Type, Dictionary<long, DataObject>>();
            m_signals = new Dictionary<Type, DataObjectSignal<DataObject>>();
        }

        public static DataObjectsController Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new DataObjectsController();
                }
                return m_instance;
            }
        }

        public T GetDataObject<T>(Int64 id) where T : DataObject
        {
            if (m_dataObjects.ContainsKey(typeof(T)) && m_dataObjects[typeof(T)].ContainsKey(id))
            {
                return (T)m_dataObjects[typeof(T)][id];
            }
            return null;
        }

        public void SetDataObject(DataObject newDataObject)
        {
            Type type = newDataObject.GetType();
            Int64 id = newDataObject.ID;
            if (m_dataObjects.ContainsKey(type))
            {
                if (m_dataObjects[type].ContainsKey(id))
                {
                    m_dataObjects[type][id] = newDataObject;
                }
                else
                {
                    m_dataObjects[type].Add(id, newDataObject);
                }
                if (m_signals.ContainsKey(type))
                {
                    m_signals[type].InvokeCallbacks(newDataObject);
                }
            }
            else
            {
                Dictionary<Int64, DataObject> newDataDict = new Dictionary<Int64, DataObject>();
                newDataDict.Add(id, newDataObject);
                m_dataObjects.Add(type, newDataDict as Dictionary<Int64, DataObject>);
            }
        }

        public void WriteDataTo(ref DataWriter writer)
        {
            foreach(Type doType in m_dataObjects.Keys)
            {
                foreach(DataObject dObject in m_dataObjects[doType].Values)
                {
                    writer.AddDataObject(dObject);
                }
            }
        }

        public void ReadDataFrom(DataReader reader)
        {
            List<DataObjectReader> doReaders = reader.GetDataObjectsReader();

            for(int i=0, count=doReaders.Count; i<count; i++)
            {
                string className = doReaders[i].GetField<string>("className");
                string assemblyName = doReaders[i].GetField<string>("assemblyName");

                DataObject newObject = (DataObject)Activator.CreateInstance(null, className).Unwrap();
                newObject.Deserialize(doReaders[i]);
                SetDataObject(newObject);
            }
        }

        public void RegisterSignal<T>(Action<T> callback) where T : DataObject
        {
            Type type = typeof(T);
            if (m_signals.ContainsKey(type))
            {
                DataObjectSignal<T> action = m_signals[type] as DataObjectSignal<T>;
                action.RegisterSignal(callback);
            }
            else
            {
                var newSignal = new DataObjectSignal<T>();
                newSignal.RegisterSignal(callback);
                m_signals.Add(type, newSignal as DataObjectSignal<DataObject>);
            }

        }

        public void UnregisterSignal<T>(Action<T> callback) where T : DataObject
        {
            Type type = typeof(T);
            if (m_signals.ContainsKey(type))
            {
                DataObjectSignal<T> action = m_signals[type] as DataObjectSignal<T>;
                action.UnregisterSignal(callback);
            }
        }

        public void RegisterSignalForId<T>(Int64 id, Action<T> callback) where T : DataObject
        {
            Type type = typeof(T);
            if (m_signals.ContainsKey(type))
            {
                DataObjectSignal<T> action = m_signals[type] as DataObjectSignal<T>;
                action.RegisterSignal(callback);
            }
            else
            {
                var newSignal = new DataObjectSignal<T>();
                newSignal.RegisterSignalForId(id, callback);
                m_signals.Add(type, newSignal as DataObjectSignal<DataObject>);
            }

        }

        public void UnregisterSignalForId<T>(Int64 id, Action<T> callback) where T : DataObject
        {
            Type type = typeof(T);
            if (m_signals.ContainsKey(type))
            {
                DataObjectSignal<T> action = m_signals[type] as DataObjectSignal<T>;
                action.UnregisterSignal(callback);
            }
        }

    }
}

