using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AprossUtils.Diagnostic
{

    public class DiagnosticResult
    {
        public string Title;
        public bool Result;
        public string Test = "";
        public string Error = "";
    }

    public class DoDiagnostic
    {
        private Stopwatch stopwatch = new Stopwatch();

        public DiagnosticResult CheckDns(string host)
        {
            DiagnosticResult d = new DiagnosticResult()
            {
                Title = "Check DNS: " + host,
                Result = true
            };
            try
            {
                stopwatch.Start();
                IPHostEntry hostInfo = Dns.GetHostEntry(host);
                stopwatch.Stop();
                long elapsed_time = stopwatch.ElapsedMilliseconds;
                IPAddress[] address = hostInfo.AddressList;
                String[] alias = hostInfo.Aliases;
                d.Test += "Host name : " + hostInfo.HostName;
                d.Test += " Ok!(" + elapsed_time + " ms)<br />Aliases : ";
                for (int index = 0; index < alias.Length; index++)
                {
                    d.Test += alias[index] + " ";
                }

                d.Test += "<br />IP Address list :";
                for (int index = 0; index < address.Length; index++)
                {
                    d.Test += address[index] + " ";
                }
            }
            catch (SocketException e)
            {
                d.Error += ("<br />SocketException caught!!!");
                d.Error += ("<br />Source : " + e.Source);
                d.Error += ("<br />Message : " + e.Message);
                d.Result = false;
            }

            catch (Exception e)
            {
                d.Error += ("<br />Exception caught!!!");
                d.Error += ("<br />Source : " + e.Source);
                d.Error += ("<br />Message : " + e.Message);
                Exception ex = e.InnerException;
                for (int i = 0; i < 10 && ex != null; i++)
                {
                    d.Error += ("<br />Source : " + ex.Source);
                    d.Error += (" - Message : " + ex.Message);
                    ex = ex.InnerException;
                }

                d.Result = false;
            }
            finally
            {
                stopwatch.Stop();
            }

            return d;

        }

        public List<DiagnosticResult> CheckForInternetConnection()
        {
            List<DiagnosticResult> result = new List<DiagnosticResult>();
            result.Add(CheckDns("google.com"));

            DiagnosticResult d = new DiagnosticResult()
            {
                Title = "Check Internet Connection",
                Result = true
            };
            try
            {
                d.Test += ("<br />Accediendo a Google");
                stopwatch.Start();
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    d.Result = true;
                stopwatch.Stop();
                long elapsed_time = stopwatch.ElapsedMilliseconds;
                d.Test += ("<br />OK! (" + elapsed_time + " ms)");
            }
            catch (Exception e)
            {
                d.Error += ("<br />Exception caught!!!");
                d.Error += ("<br />Source : " + e.Source);
                d.Error += ("<br />Message : " + e.Message);
                Exception ex = e.InnerException;
                for (int i = 0; i < 10 && ex != null; i++)
                {
                    d.Error += ("<br />Source : " + ex.Source);
                    d.Error += (" - Message : " + ex.Message);
                    ex = ex.InnerException;
                }
                d.Result = false;
            }
            finally
            {
                stopwatch.Stop();
            }
            result.Add(d);
            return result;

        }

        public DiagnosticResult CheckTempDir()
        {
            DiagnosticResult d = new DiagnosticResult()
            {
                Title = "Check Temporary Directory",
                Result = true
            };

            string path = Path.GetTempPath();
            d.Test += "Path:" + path;

            try
            {

                using (StreamWriter sw = new StreamWriter(path + "test.txt"))
                {
                    sw.WriteLine("test");
                }
                d.Test += "<br /> Write Success";

                File.Delete(Path.Combine(path, "test.txt"));
                d.Test += "<br /> Delete Success";
            }
            catch (Exception e)
            {
                d.Error += ("<br />Exception caught!!!");
                d.Error += ("<br />Source : " + e.Source);
                d.Error += ("<br />Message : " + e.Message);
                Exception ex = e.InnerException;
                for (int i = 0; i < 10 && ex != null; i++)
                {
                    d.Error += ("<br />Source : " + ex.Source);
                    d.Error += (" - Message : " + ex.Message);
                    ex = ex.InnerException;
                }
                d.Result = false;
            }
            return d;
        }
    }

    public interface IDiagnostic
    {
        Task<List<DiagnosticResult>> Diagnostic();
    }
}