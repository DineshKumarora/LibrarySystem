using System;
using System.Text;
using Newtonsoft.Json;

namespace AlertSubscriptionService.RabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectSerialize
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(obj);
            return Encoding.ASCII.GetBytes(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeSerialize(this byte[] arrBytes, Type type)
        {
            var json = Encoding.Default.GetString(arrBytes);
            return JsonConvert.DeserializeObject(json, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public static string DeSerializeText(this byte[] arrBytes)
        {
            return Encoding.Default.GetString(arrBytes);
        }
    }
}