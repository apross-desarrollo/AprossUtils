using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

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


        public Gtag GetByUserId(string user_id, HttpSessionStateBase session)
        {
            var gtag = new Gtag()
            {
                Id = _trackID,
            };
            gtag.UserID = user_id;
            if (session["gtag_events"] != null)
                gtag.Eventos = (List<GtagEvento>)session["gtag_events"];

            return gtag;
        }

    }

    public class Gtag
    {
        public string Id { get; set; } //G-MDD37EPVF3
        public string UserID { get; set; }
        public List<GtagEvento> Eventos { get; set; }
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
}