using System;


namespace HIPMS.Services
{
    public class BaseResponse
    {
        static string StatusSucess = "Success", StatusFail = "Fail";
        public static Object SuccessResponse(int Code)
        {
            ResponseModel _responseModel = new ResponseModel();
            _responseModel.Code = Code;
            _responseModel.Status = StatusSucess;

            MainResponse objMainResponce = new MainResponse();
            objMainResponce.Response = _responseModel;
            return objMainResponce;
        }

        public static Object failedResponse(int Code)
        {
            ResponseModel _responseModel = new ResponseModel();
            _responseModel.Code = Code;
            _responseModel.Status = StatusFail;

            MainResponse objMainResponce = new MainResponse();
            objMainResponce.Response = _responseModel;
            return objMainResponce;
        }

        public static Object SuccessResponse(int Code, string Message)
        {
            ResponseModel _responseModel = new ResponseModel();
            _responseModel.Code = Code;
            _responseModel.Status = StatusSucess;
            _responseModel.Message = ErrorModel.MessageReturn(Code);
            _responseModel.DisplayMessage = Message;

            MainResponse objMainResponce = new MainResponse();
            objMainResponce.Response = _responseModel;
            return objMainResponce;
        }

        public static Object failedResponse(int Code, string Message)
        {
            ResponseModel _responseModel = new ResponseModel();
            _responseModel.Code = Code;
            _responseModel.Status = StatusFail;
            _responseModel.Message = ErrorModel.MessageReturn(Code);
            _responseModel.DisplayMessage = Message;

            MainResponse objMainResponce = new MainResponse();
            objMainResponce.Response = _responseModel;
            return objMainResponce;
        }

        public static Object SuccessResponse(object OB, int Code, string Message)
        {
            ResponseModel _responseModel = new ResponseModel();
            _responseModel.Code = Code;
            _responseModel.Status = StatusSucess;
            _responseModel.Message = ErrorModel.MessageReturn(Code);
            _responseModel.DisplayMessage = Message;

            ResponseDataModel RDM = new ResponseDataModel();
            RDM.Data = OB;
            RDM.Response = _responseModel;
            return RDM;
        }

        public static Object failedResponse(object OB, int Code, string Message)
        {
            ResponseModel _responseModel = new ResponseModel();
            _responseModel.Code = Code;
            _responseModel.Status = StatusFail;
            _responseModel.Message = ErrorModel.MessageReturn(Code);
            _responseModel.DisplayMessage = Message;

            ResponseDataModel RDM = new ResponseDataModel();
            RDM.Data = OB;
            RDM.Response = _responseModel;
            return RDM;
        }
    }
    public class ResponseModel
    {
        public int Code { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public string DisplayMessage { get; set; }
    }
    public class ResponseDataModel
    {
        public object Data { get; set; }
        public ResponseModel Response { get; set; }
    }
    public class MainResponse
    {
        public ResponseModel Response { get; set; }
    }
    public class ErrorModel
    {
        public static string MessageReturn(int ErrorCode)
        {
            string Message = "";
            switch (ErrorCode)
            {
                case 100:
                    Message = "Data Not Found";
                    break;

                case 200:
                    Message = "Success";
                    break;

                case 400:
                    Message = "Bad Request";
                    break;

                case 401:
                    Message = "Unauthorized";
                    break;

                case 403:
                    Message = "Forbidden";
                    break;

                case 404:
                    Message = "Not Found";
                    break;

                case 405:
                    Message = "Method Not Allowed";
                    break;

                case 429:
                    Message = "Invalid Or Insufficient Parameters";
                    break;

                case 500:
                    Message = "Internal Server Error";
                    break;

                case 501:
                    Message = "Not Implemented";
                    break;

                default:
                    Message = "Undefined Error";
                    break;
            }
            return Message;
        }
    }

    public class HIPMSErrorConst
    {
        public const string InvalidInputParameter = "Invalid Input Parameter";

        public const string NotNull = "Input parameter cannot be null.";

        public const string InternalServer = "Internal Server Error";

        public const string BalanceQty = "Entered quantity should less then or equal to balanace quantity.";
        public const string SessionTimeOut = "Session Time Out";
    }
}
