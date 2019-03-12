// ***********************************************************************
// Assembly         : FairUtility
// Author           : Yubao Li
// Created          : 08-12-2015
//
// Last Modified By : Yubao Li
// Last Modified On : 08-12-2015
// ***********************************************************************
// <copyright file="JsonHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>json序列化帮助类</summary>
// ***********************************************************************

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure
{
    public static class JsonHelper
    {
        //private static JsonHelper _jsonHelper = new JsonHelper();
        //public static JsonHelper Instance { get { return _jsonHelper; } }
        //需要17以上vs
        

        public static string JsonSerialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
        }

        public static string JsonSerializeByConverter(this object obj, params JsonConverter[] converters)
        {
            
            return JsonConvert.SerializeObject(obj, converters);
        }

        public static T JsonDeserialize<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        public static T JsonDeserializeByConverter<T>(this string input,params JsonConverter[] converter)
        {
            return JsonConvert.DeserializeObject<T>(input, converter);
        }

        public static T JsonDeserializeBySetting<T>(this string input, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(input, settings);
        }

        private static object NullToEmpty(this object obj)
        {
            return null;
        }
    }
}