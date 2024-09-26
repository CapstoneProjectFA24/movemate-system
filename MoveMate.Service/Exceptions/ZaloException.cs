namespace MoveMate.Service.Exeptions
{
    public class ZaloException : Exception
    {
        /// <summary>
        /// ExceptionCode
        /// </summary>
        public string ExceptionCode { get; set; }

        /// <summary>
        /// ExceptionMessage
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// ApiCode
        /// </summary>
        public string ApiCode { get; set; }

        /// <summary>
        /// EcoException
        /// </summary>
        /// <param name="exceptionCode"></param>
        /// <param name="exceptionMessage"></param>
        /// <param name="apiCode"></param>
        public ZaloException(string exceptionCode, string exceptionMessage, string apiCode)
            : base(exceptionMessage)
        {
            ExceptionCode = exceptionCode;
            ExceptionMessage = exceptionMessage;
            ApiCode = apiCode;
        }

        /// <summary>
        /// EcoException
        /// </summary>
        /// <param name="exceptionCode"></param>
        /// <param name="exceptionMessage"></param>
        public ZaloException(string exceptionCode, string exceptionMessage) 
            : base(exceptionMessage)
        {
            ExceptionCode = exceptionCode;
            ExceptionMessage = exceptionMessage;
            ApiCode = string.Empty;
        }
    }
}
