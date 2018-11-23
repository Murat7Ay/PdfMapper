using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.Json;
using Nancy.ModelBinding;
using Nancy.Responses;

namespace NancyApi
{
    class Program
    {
        private readonly NancyHost _nancy;
        public Program()
        {
            var uri = new Uri("http://localhost:7880");
            var hostConfigs = new HostConfiguration { UrlReservations = { CreateAutomatically = true }, };
            _nancy = new NancyHost(uri, new DefaultNancyBootstrapper(), hostConfigs);
            
        }

        private void Start()
        {
            _nancy.Start();
            Console.WriteLine("Started listenig address http://localhost:7880");
            Console.ReadKey();
            _nancy.Stop();
        }

        static void Main(string[] args)
        {
            var p = new Program();
            p.Start();
        }
    }
    public class SampleModule : Nancy.NancyModule
    {
        public SampleModule()
        {
            Get["/"] = _ => "Possunt quia posse videntur!";
        }
    }
    public static class NancyExtensions
    {
        public static void EnableCors(this NancyModule module)
        {
            module.After.AddItemToEndOfPipeline(x =>
            {
                x.Response.WithHeader("Access-Control-Allow-Origin", "*");
            });
        }
    }
    public class PrintDataModule : NancyModule
    {
        public PrintDataModule()
        {
            After.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");
            });
            Post["/printdata"] = parameters =>
            {
                var printDatas = this.Bind<List<PrintData>>();
                foreach (var printData in printDatas)
                {
                    Console.WriteLine(printData.x0+"-"+printData.y0);
                }

                var response = Response.AsJson(printDatas);
                response.ContentType = "application/json";
                response.Headers.Add("Vary", "Accept");
                response.StatusCode = HttpStatusCode.OK;

                //return response
                return response;
            };
        }
    }

    public class PrintData
    {
        public string coloumnid { get; set; }
        public string fontsize { get; set; }
        public string fonttype { get; set; }
        public string isbold { get; set; }
        public string isitalic { get; set; }
        public string isvertical { get; set; }
        public string isphoto { get; set; }
        public string iscontrol { get; set; }
        public string pagenumber { get; set; }
        public string description { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public string y1 { get; set; }
        public string x1 { get; set; }
        public string y0 { get; set; }
        public string x0 { get; set; }
        public string xcenter { get; set; }
        public string ycenter { get; set; }
    }


}
