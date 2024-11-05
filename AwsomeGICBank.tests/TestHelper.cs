using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsomeGICBank.tests
{
    public static class TestHelper
    {

        public static string CaptureConsoleOutput(Action action)
        {
            using (var sw = new System.IO.StringWriter())
            {
                Console.SetOut(sw);
                action.Invoke();
                return sw.ToString();
            }
        }
    }
}
