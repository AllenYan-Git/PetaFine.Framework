 
//using Newtonsoft.Json;
//using System;

//using System.Threading.Tasks;

//namespace Infrastructure
//{
//    /// <summary>
//    /// WEBAPI调用类。调用方式举例：string url = string.Format("api/E_Config/GetValue/?Key={0}", Key);
//    /// var r =  APIUtils.CallAPI<string>(APIUtils.Enums.ApiOperator.GET, url);
//    /// </summary>
//    public class APIUtils
//    {
//        /// <summary>
//        /// 返回API地址，如http://localhost:8888/api/url
//        /// </summary>
//        /// <param name="url"></param>
//        /// <returns></returns>
//        public static string GetAPIUrl(string url)
//        {
//            return string.Format("{0}/api/{1}", AppSettingsHelper.GetString("APIUrl", ""), url);
//        }

//        /// <summary>
//        /// 返回API地址，如http://localhost:8888/api/
//        /// </summary>
//        /// <returns></returns>
//        public static string GetAPIUrl()
//        {
//            return string.Format("{0}/api/", AppSettingsHelper.GetString("APIUrl", ""));
//        }

//        /// <summary>
//        /// 执行API调用的 HTTP POST/PUT 操作
//        /// </summary>
//        /// <typeparam name="In">输入参数类型</typeparam>
//        /// <typeparam name="Out">输出参数类型</typeparam>
//        /// <param name="op">API操作类型</param>
//        /// <param name="apiUrl">API URL</param>
//        /// <param name="obj">输入参数实例</param>
//        /// <returns>返回输出参数实例</returns>
//        public static Out CallAPI<In, Out>(Enums.ApiOperator op, string apiUrl, In obj)
//        {
//            return ApiExecute<In, Out>(op, apiUrl, obj);
//        }

//        /// <summary>
//        /// 执行API调用的 HTTP GET/DELETE 操作
//        /// </summary>
//        /// <typeparam name="Out">输出参数类型</typeparam>
//        /// <param name="op">API操作类型</param>
//        /// <param name="apiUrl">API URL</param>
//        /// <returns>返回输出参数实例</returns>
//        public static Out CallAPI<Out>(Enums.ApiOperator op, string apiUrl)
//        {
//            return ApiExecute<object, Out>(op, apiUrl);
//        }

//        /// <summary>
//        /// 执行API操作
//        /// </summary>
//        /// <typeparam name="In">输入参数类型</typeparam>
//        /// <typeparam name="Out">输出参数类型</typeparam>
//        /// <param name="op">API操作类型</param>
//        /// <param name="apiUrl">API URL</param>
//        /// <param name="obj">输入参数实例</param>
//        /// <returns>返回输出参数实例</returns>
//        private static Out ApiExecute<In, Out>(Enums.ApiOperator op, string apiUrl, In obj = default(In))
//        {
//            // 需要引用  C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Net.Http.dll
//            using (HttpClient client = new HttpClient())
//            {
//                Out result = default(Out);
//                try
//                {
//                    HttpContent hc = null;
//                    if (obj != null && obj.GetType() != typeof(string) && obj.GetType().BaseType == typeof(object))
//                    {
//                        var requestJson = JsonConvert.SerializeObject(obj);
//                        hc = new StringContent(requestJson);
//                        hc.Headers.ContentType = new MediaTypeHeaderValue("application/json");
//                    }
//                    client.MaxResponseContentBufferSize = 1024 * 1024 * 50;//50M                
//                    client.BaseAddress = new Uri(AppSettingsHelper.GetString("APIUrl", ""));

//                    Task<HttpResponseMessage> task = null;
//                    if (op == Enums.ApiOperator.POST)
//                    {
//                        task = client.PostAsync(apiUrl, hc);
//                    }
//                    else if (op == Enums.ApiOperator.PUT)
//                    {
//                        task = client.PutAsync(apiUrl, hc);
//                    }
//                    else if (op == Enums.ApiOperator.DELETE)
//                    {
//                        task = client.DeleteAsync(apiUrl);
//                    }
//                    else
//                    {
//                        task = client.GetAsync(apiUrl);
//                    }
//                    task.Result.EnsureSuccessStatusCode();
//                    HttpResponseMessage response = task.Result;
//                    // 把Json数据转换成强类型实际数据
//                    var resultJson = response.Content.ReadAsStringAsync().Result;
//                    result = JsonConvert.DeserializeObject<Out>(resultJson);
//                }
//                catch (Exception ex)
//                {
//                    //LogHelper.Intance.WriteError("API调用异常(ApiExecute)", ex);
//                }
//                finally
//                {
//                    client.CancelPendingRequests();
//                    client.Dispose();
//                }
//                return result;
//            }
//        }
//        public class Enums
//        {
//            /// <summary>
//            /// API操作
//            /// </summary>
//            public enum ApiOperator
//            {
//                /// <summary>
//                /// GET
//                /// </summary>
//                GET,
//                /// <summary>
//                /// POST
//                /// </summary>
//                POST,
//                /// <summary>
//                /// PUT
//                /// </summary>
//                PUT,
//                /// <summary>
//                /// DELETE
//                /// </summary>
//                DELETE
//            }
//        }
//    }
      
//}
