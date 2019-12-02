using System;
namespace MiddlewareTest.Controllers
{
    public class CustomException:Exception
    {
        public CustomException()
        {
        }

        public CustomException(string arg)
        : base(String.Format("Whatever I'm throwing : {0}", arg))
        {
        }
    }
}
