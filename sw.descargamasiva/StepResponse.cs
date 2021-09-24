using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.descargamasiva
{
    public class StepResponse
    {
        
        public string ResponseStatusCode { get; set; }
        public string ResponseStatusMessage { get; set; }
        public string Step { get; set; }
        public string RawRequest  { get; set; }
        public string RawResponse { get; set; }
        public string Token { get; set; }
        public string RequestId{ get; set; }
        public string Situation { get; set; }
        public string QtyCfdi { get; set; }
        public string Base64Package { get; set; }

        /// <summary>
        /// CodigoEstadoSolicitud="5002" Se agotó las solicitudes de por vida
        /// </summary>
        public string CodeStatusRequest { get; set; }

        public List<string> PackageIdentifierList { get; set; } = new List<string>();
        public Dictionary<string, string> PackageList { get; set; } = new Dictionary<string, string>();

    }
}
