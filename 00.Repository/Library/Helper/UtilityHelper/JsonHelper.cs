// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Company Name">
//   Company Name
// </copyright>
// <summary>
//   Class JsonHelper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Library.Helpers.UtilityHelper
{
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public class JsonHelper
    {
        #region Universal Methods

        public string Serialize<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                GetSerializer<T>().WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public T Deserialize<T>(string json)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                    writer.Flush();
                    stream.Position = 0;
                    return (T)GetSerializer<T>().ReadObject(stream);
                }
            }
        }

        private static DataContractJsonSerializer GetSerializer<T>()
        {
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            };

            return new DataContractJsonSerializer(typeof(T), settings);
        }

        #endregion Universal Methods
    }
}
