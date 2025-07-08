using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace INPUTLAGFIX
{
    public abstract class BackuperFromXml
    {
        public virtual T DeserializeAutoRunsItem<T>(XmlReader reader)
        {
            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(reader.Name));
            return (T)serializer.Deserialize(reader);
        }
    }
}
