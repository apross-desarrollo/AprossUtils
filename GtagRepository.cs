using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;

namespace AprossUtils.Gtag
{
    public class GtagRepository
    {
        private string _trackID;
        public string TrackID { get => _trackID; }
        public GtagRepository(NameValueCollection config)
        {
            _trackID = config["TrackID"];
        }

        public static void SetEvento(GtagEvento evento, HttpSessionStateBase session)
        {
            List<GtagEvento> eventos;
            if (session["gtag_events"] == null) eventos = new List<GtagEvento>();
            else eventos = (List<GtagEvento>)session["gtag_events"];
            eventos.Add(evento);
            session["gtag_events"] = eventos;
        }

        public static void SetException(GtagException exception, HttpSessionStateBase session)
        {
            session["gtag_exception"] = exception;
        }


        public Gtag GetByUserId(string user_id, HttpSessionStateBase session)
        {
            var gtag = new Gtag()
            {
                Id = _trackID,
            };
            gtag.UserID = user_id;
            if (session["gtag_events"] != null)
                gtag.Eventos = (List<GtagEvento>)session["gtag_events"];
            if (session["gtag_exception"] != null)
            {
                gtag.Exception = (GtagException)session["gtag_exception"];
            }
            return gtag;
            
        }

    }

    public class Gtag
    {
        public string Id { get; set; } //G-MDD37EPVF3
        public string UserID { get; set; }
        public List<GtagEvento> Eventos { get; set; } 
        public GtagException Exception { get; set; }

        public string GetExceptionString()
        {
            if (Exception!=null) return JsonConvert.SerializeObject(Exception);
            return default;
        }
    }

    public class GtagEvento
    {
        public string Id { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public string GetData()
        {
            return JsonConvert.SerializeObject(Data);
        }
    }

    public class GtagException
    {
        public string Description { get; set; }
        public bool Fatal { get; set; }
        public string UserId { get; set; }
        public string ControllerName { get; set; }
        public string Action { get; set; }
        public HandleErrorInfo Exception { get; set; }
        public string StackTrace { get; set; }
    }
}