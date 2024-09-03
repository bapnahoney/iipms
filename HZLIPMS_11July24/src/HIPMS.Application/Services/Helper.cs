using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace HIPMS.Services;

public static class Helper
{
    public static string GetCurrentFinancialYear()
    {
        int CurrentYear = DateTime.Today.Year;
        int PreviousYear = DateTime.Today.Year - 1;
        int NextYear = DateTime.Today.Year + 1;
        string PreYear = PreviousYear.ToString();
        string NexYear = NextYear.ToString();
        string CurYear = CurrentYear.ToString();
        string FinYear = null;
        string fys = DateTime.Today.Year.ToString().Substring(2);
        string py = Convert.ToString(Convert.ToInt32(fys) + 1);

        if (DateTime.Today.Month > 3)
            FinYear = CurYear + "-" + py;
        else
            FinYear = PreYear + "-" + DateTime.Today.Year.ToString().Substring(2);
        return FinYear.Trim();
    }

    public static Object Caller(object InputData, string APIName)
    {
        string result = string.Empty;
        try
        {
            string apiUrl = Convert.ToString(ConfigurationManager.AppSettings["SAPURL"]) + "/" + APIName;

            Inputdata ID = new Inputdata();

            ID.Data = Convert.ToString(InputData);
            ID.API = apiUrl;

            string inputJson = JsonConvert.SerializeObject(ID);  //InputData;

            string Url = ConfigurationManager.AppSettings["HZLURL"];

            HttpWebRequest httpWebRequest;
            httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            // httpWebRequest.Headers.Add("Authorization: Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(UserName + ":" + password)));

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(inputJson);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            result = string.Empty;
        }
        return result;
    }

    //private string SyncDataTOSAP()
    //{
    //    var getResult = "";
    //    try
    //    {
    //        //Get Token And Cookies
    //        var getTokenCookie = GetSAPTokenCookies();
    //        var splitTokenCookie = getTokenCookie.Split('~');
    //        var x_csrf_token = splitTokenCookie[0];
    //        var cookie = splitTokenCookie[1];
    //        //End

    //        var getURL = "http://10.250.2.5:50000//RESTAdapter/podata/";// _genericRespository.getSAPEndpoint("CREATEVENDOR");
    //        // Call Post Data
    //        var request = (HttpWebRequest)WebRequest.Create(getURL);
    //            //  var request = (HttpWebRequest)WebRequest.Create("https://hbt-hana01.highbartech.com/vpsapapis/poconf?sap-client=130");

    //            var obj = new
    //            {
    //                PO_NO = "8510001291",// Convert.ToString(input.PONo),
    //                AUTH_KEY = "MME5MI9OCDN5VERHD1C0MZYZRVH4" //ConfigurationManager.AppSettings["SAPAuthKey"]
    //            };

    //            var json = JsonConvert.SerializeObject(obj);

    //        //string parm = "{\n\"po_no\":\"11000118\",\n\"item_no\":\"10\"}";
    //        string DATA = json.ToString();

    //        request.Method = "POST";
    //        request.ContentType = "application/json";
    //        request.Credentials = new NetworkCredential("hopsdm", "Blue@1993");
    //        request.UseDefaultCredentials = true;
    //        request.PreAuthenticate = true;
    //        // request.Credentials = CredentialCache.DefaultCredentials;
    //        request.ContentLength = DATA.Length;
    //        request.Headers.Add("x-csrf-token", x_csrf_token);
    //        request.Headers.Add("Cookie", cookie);
    //        using (Stream webStream = request.GetRequestStream())
    //        using (StreamWriter requestWriter = new StreamWriter(webStream, Encoding.ASCII))
    //        {
    //            requestWriter.Write(DATA);
    //        }
    //        WebResponse webResponse = request.GetResponse();
    //        using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
    //        using (StreamReader responseReader = new StreamReader(webStream))
    //        {
    //            string response = responseReader.ReadToEnd();

    //            JObject joResponse = JObject.Parse(response);
    //            string jsonresponse = joResponse.ToString();

    //            dynamic respn = JsonConvert.DeserializeObject(jsonresponse);

    //            //var model = JsonConvert.DeserializeObject<VMGetResponseVendorSynkSAP>(jsonresponse);
    //            //if (model.RETURN_CODE == "200")
    //            //{
    //            //    getResult = model.RETURN_VALUE;
    //            //}
    //            //else
    //            //{
    //            //    getResult = "Error";
    //            //}
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        getResult = "Error";
    //    }
    //    return getResult;
    //}

    //private string GetSAPTokenCookies()
    //{

    //    var token = "";
    //    try
    //    {
    //        var getLoginURL = _genericRespository.getSAPEndpoint("LOGIN");
    //        HttpMessageHandler handler = new HttpClientHandler()
    //        {
    //        };
    //        var httpClient = new HttpClient(handler)
    //        {
    //            BaseAddress = new Uri(getLoginURL),
    //            Timeout = new TimeSpan(0, 2, 0)
    //        };
    //        var getSAPUserId = ConfigurationManager.AppSettings["SAPUserName"].ToString();
    //        var getSAPPassword = ConfigurationManager.AppSettings["SAPPassword"].ToString();
    //        var getSAPAuthorization = ConfigurationManager.AppSettings["SAPAuthorizationKey"].ToString();
    //        getSAPAuthorization = "Basic " + getSAPAuthorization;
    //        httpClient.DefaultRequestHeaders.Add("ContentType", "application/json;");
    //        httpClient.DefaultRequestHeaders.Add("x-csrf-token", "Fetch");

    //        httpClient.DefaultRequestHeaders.Add("Authorization", getSAPAuthorization);
    //        httpClient.DefaultRequestHeaders.Add("username", getSAPUserId);
    //        httpClient.DefaultRequestHeaders.Add("password", getSAPPassword);

    //        HttpResponseMessage response = httpClient.GetAsync(getLoginURL).Result;
    //        string content = string.Empty;
    //        IEnumerable<string> values;
    //        values = response.Headers.GetValues("x-csrf-token");
    //        string token1 = values.First();
    //        var cookies = response.Headers.GetValues("Set-Cookie");
    //        var splitval1 = cookies.First().Split(';')[0];
    //        var splitval2 = cookies.Last().Split(';')[0];
    //        var cookiesJoin = splitval2 + "; " + splitval1;
    //        using (StreamReader stream = new StreamReader(response.Content.ReadAsStreamAsync().Result))
    //        {
    //            content = stream.ReadToEnd();
    //        }
    //        var finalString = token1 + "~" + cookiesJoin;
    //        token = finalString;
    //    }
    //    catch (Exception ex)
    //    {
    //        token = "";
    //    }
    //    return token;
    //}

    //public string getSAPEndpoint(string URLFor)
    //{

    //    string URL = "";

    //    DynamicParameters queryParameters = new DynamicParameters();
    //    queryParameters.Add("@URLFor", URLFor);

    //    var result = _db.Query("sp_GetSAPEndpoint", queryParameters, commandType: CommandType.StoredProcedure);

    //    foreach (var item in result)
    //    {
    //        URL = item.SAP_Endpoint;
    //    }

    //    return URL;
    //}

}
public class Inputdata
{
    public string Data { get; set; }
    public string API { get; set; }
}


public class Response
{
    public string Code { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public string DisplayMessage { get; set; }
}