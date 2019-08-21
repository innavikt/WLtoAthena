using System;
using System.Diagnostics;
//using System.IO;

namespace ScriptInterface
{
    public class ScriptRunner
    {
        //args separated by spaces
        public static string RunFromCmd(string rCodeFilePath, string args)
        {
            string file = rCodeFilePath;
            string result = string.Empty;

            try
            {

                var info = new ProcessStartInfo(@"C:\Anaconda3\python.exe");
                info.Arguments = rCodeFilePath + " " + args;

                info.RedirectStandardInput = false;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        result = proc.StandardOutput.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Py Script failed: " + result, ex);

            }
        }
        public static void Main()
        {
            // string args =   @"SELECT * 
            //                 FROM options_history
            //                 LEFT JOIN stock_history
            //                 ON options_history.index = stock_history.index
            //                 LIMIT 20;";
            Console.WriteLine("I start python query execution");
            string args = @"SELECT options_history.trade_time, options_history.option_type, 
                            options_history.expiration_date, options_history.strike, options_history.price,     
                            options_history.volume, stock_history.price as stock_price
                            FROM options_history 
                            LEFT JOIN stock_history
                            ON options_history.index = stock_history.index;";
            string res = ScriptRunner.RunFromCmd(@"code.py", args);
            Console.WriteLine(res);
        }
    }
}
