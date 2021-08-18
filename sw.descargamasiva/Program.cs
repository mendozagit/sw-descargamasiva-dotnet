using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        private static byte[] pfx;
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
        static string FechaInicial = "2021-03-01";
        static string FechaFinal = "2021-04-30";

        private static string cerPath = @"C:\Users\PHILIPS-JESUSMENDOZA\Desktop\DGE131017IP1\cer.cer";
        static string keyPath = @"C:\Users\PHILIPS-JESUSMENDOZA\Desktop\DGE131017IP1\key.key";
        static string keyPass = "jjMM1994";
        private static string binPath = @"C:\Program Files\OpenSSL-Win64\bin";
        private static string tempDir = @"C:\Users\PHILIPS-JESUSMENDOZA\Desktop\DGE131017IP1\";



        static void Main(string[] args)
        {
            var pfxSerice = new PfxService(tempDir, binPath);


            var pfxFile = pfxSerice.CreatePfx(cerPath, keyPath, keyPass);





            // pfx = File.ReadAllBytes(@"C:\Users\PHILIPS-JESUSMENDOZA\Desktop\DGE131017IP1\PFX.pfx");
            pfx = pfxFile.Item2;

            //pfx = pfxFile.Item2;
            //Obtener Certificados
            X509Certificate2 certifcate = ObtenerX509Certificado(pfx);

            //Obtener Token
            string token = ObtenerToken(certifcate);
            string autorization = String.Format("WRAP access_token=\"{0}\"", HttpUtility.UrlDecode(token));
            Console.WriteLine("Token: " + token);

            //Generar Solicitud
            string idSolicitud = GenerarSolicitud(certifcate, autorization);
            Console.WriteLine("IdSolicitud: " + idSolicitud);
            //string idSolicitud = "7b12ecad-b3de-4154-9990-210db3394de1";
            //Validar Solicitud
            string idPaquete = ValidarSolicitud(certifcate, autorization, idSolicitud);
            Console.WriteLine("IdPaquete: " + idPaquete);

            if (!string.IsNullOrEmpty(idPaquete))
            {
                //Descargar Solicitud
                string descargaResponse = DescargarSolicitud(certifcate, autorization, idPaquete);

                GuardarSolicitud(idPaquete, descargaResponse);
            }
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

        private static string DescargarSolicitud(X509Certificate2 certifcate, string autorization, string idPaquete)
        {
            DescargarSolicitud descargarSolicitud = new DescargarSolicitud(urlDescargarSolicitud, urlDescargarSolicitudAction);
            string xmlDescarga = descargarSolicitud.Generate(certifcate, RfcEmisor, idPaquete);
            return descargarSolicitud.Send(autorization);
        }

        private static string ValidarSolicitud(X509Certificate2 certifcate, string autorization, string idSolicitud)
        {
            VerificaSolicitud verifica = new VerificaSolicitud(urlVerificarSolicitud, urlVerificarSolicitudAction);
            string xmlVerifica = verifica.Generate(certifcate, RfcEmisor, idSolicitud);
            return verifica.Send(autorization);
        }

        private static string GenerarSolicitud(X509Certificate2 certifcate, string autorization)
        {
            Solicitud solicitud = new Solicitud(urlSolicitud, urlSolicitudAction);
            string xmlSolicitud = solicitud.Generate(certifcate, RfcEmisor, RfcReceptor, RfcEmisor, FechaInicial, FechaFinal);
            return solicitud.Send(autorization);
        }

        private static string ObtenerToken(X509Certificate2 certifcate)
        {
            Autenticacion service = new Autenticacion(urlAutentica, urlAutenticaAction);
            string xml = service.Generate(certifcate);
            return service.Send();
        }
    }
}
