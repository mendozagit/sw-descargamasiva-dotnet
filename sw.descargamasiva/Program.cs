using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace sw.descargamasiva
{
    class Program
    {
        private static byte[] _pfx;
        static string password = "jjMM1994";

        static string urlAutentica = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/Autenticacion/Autenticacion.svc";
        static string urlAutenticaAction = "http://DescargaMasivaTerceros.gob.mx/IAutenticacion/Autentica";

        static string urlSolicitud = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/SolicitaDescargaService.svc";
        static string urlSolicitudAction = "http://DescargaMasivaTerceros.sat.gob.mx/ISolicitaDescargaService/SolicitaDescarga";

        static string urlVerificarSolicitud = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/VerificaSolicitudDescargaService.svc";
        static string urlVerificarSolicitudAction = "http://DescargaMasivaTerceros.sat.gob.mx/IVerificaSolicitudDescargaService/VerificaSolicitudDescarga";

        static string urlDescargarSolicitud = "https://cfdidescargamasiva.clouda.sat.gob.mx/DescargaMasivaService.svc";
        static string urlDescargarSolicitudAction = "http://DescargaMasivaTerceros.sat.gob.mx/IDescargaMasivaTercerosService/Descargar";

        static string RfcEmisor = "MEJJ940824C61";
        static string RfcReceptor = "";
        static string FechaInicial = "2020-01-01";
        static string FechaFinal = "2020-04-30";

        static void Main(string[] args)
        {




            // pfx = File.ReadAllBytes(@"C:\Users\PHILIPS-JESUSMENDOZA\Desktop\DGE131017IP1\PFX.pfx");
            const string b64 = "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";
            _pfx = Convert.FromBase64String(b64);


            //Obtener Certificados
            X509Certificate2 certifcate = ObtenerX509Certificado(_pfx);

            //Obtener Token
            var stepResponseToken = Step1GetToken(certifcate);
            string autorization = String.Format("WRAP access_token=\"{0}\"", HttpUtility.UrlDecode(stepResponseToken.Token));
            Console.WriteLine("Token: " + stepResponseToken.Token);

            //Generar Solicitud
            var stepResponseIdSolicitud = Step2GetRequest(certifcate, autorization);
            Console.WriteLine("IdSolicitud: " + stepResponseToken.RequestId);
            //string idSolicitud = "7b12ecad-b3de-4154-9990-210db3394de1";

            //Validar Solicitud
            //Get packageList
            var stepResponseIdPaquete = Step3VerifyRequest(certifcate, autorization, stepResponseToken.RequestId);
            Console.WriteLine("PackageIdentifierList: " + stepResponseIdPaquete.PackageIdentifierList.ToString());


            //Descargar Solicitud
            //Get packageList file inBase 64
            foreach (var packageId in stepResponseIdPaquete.PackageIdentifierList)
            {
                var stepResponseDescargaResponse = Step4GetPackages(certifcate, autorization, packageId);

                foreach (var package in stepResponseDescargaResponse.PackageList)
                {
                    File.WriteAllBytes(package.Key, Convert.FromBase64String(package.Value));
                }

            }


            //GuardarSolicitud(idPaquete, descargaResponse);

            Console.ReadLine();
        }

        private static X509Certificate2 ObtenerX509Certificado(byte[] pfx)
        {
            return new X509Certificate2(pfx, password, X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable);
        }

        private static void GuardarSolicitud(string idPaquete, string descargaResponse)
        {
            string path = "./Paquetes/";
            byte[] file = Convert.FromBase64String(descargaResponse);
            Directory.CreateDirectory(path);

            using (FileStream fs = File.Create(path + idPaquete + ".zip", file.Length))
            {
                fs.Write(file, 0, file.Length);
            }
            Console.WriteLine("FileCreated: " + path + idPaquete + ".zip");
        }

        private static StepResponse Step4GetPackages(X509Certificate2 certifcate, string autorization, string idPaquete)
        {
            DescargarSolicitud descargarSolicitud = new DescargarSolicitud(urlDescargarSolicitud, urlDescargarSolicitudAction);
            string xmlDescarga = descargarSolicitud.Generate(certifcate, RfcEmisor, idPaquete);
            return descargarSolicitud.Send(autorization);
        }

        private static StepResponse Step3VerifyRequest(X509Certificate2 certifcate, string autorization, string idSolicitud)
        {
            VerificaSolicitud verifica = new VerificaSolicitud(urlVerificarSolicitud, urlVerificarSolicitudAction);
            string xmlVerifica = verifica.Generate(certifcate, RfcEmisor, idSolicitud);
            return verifica.Send(autorization);
        }

        private static StepResponse Step2GetRequest(X509Certificate2 certifcate, string autorization)
        {
            var method = MethodBase.GetCurrentMethod();
            var methodName = method.Name;

            var requestName = $"{methodName}Request.XML";
            var responseName = $"{methodName}Response.XML";


            var service = new Solicitud(urlSolicitud, urlSolicitudAction);
            var xmlSolicitud = service.Generate(certifcate, RfcEmisor, RfcReceptor, RfcEmisor, FechaInicial, FechaFinal);

            CacheManager.RawRequest = xmlSolicitud;
            File.WriteAllText(requestName, CacheManager.RawRequest);

            return service.Send(resposeFileName: responseName, stepName: methodName);


        }

        private static StepResponse Step1GetToken(X509Certificate2 certifcate)
        {
            var method = MethodBase.GetCurrentMethod();
            var methodName = method.Name;

            var requestName = $"{methodName}Request.XML";
            var responseName = $"{methodName}Response.XML";


            var service = new Autenticacion(urlAutentica, urlAutenticaAction);
            var xml = service.Generate(certifcate);

            CacheManager.RawRequest = xml;
            File.WriteAllText(requestName, CacheManager.RawRequest);
            return service.Send(resposeFileName: responseName, stepName: methodName);
        }
    }
}
