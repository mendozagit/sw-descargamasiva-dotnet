using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace sw.descargamasiva
{
    internal class VerificaSolicitud : SoapRequestBase
    {
        public VerificaSolicitud(string url, string action)
            : base(url, action)
        {
        }

        #region Crea el XML para enviar

        public string Generate(X509Certificate2 certificate, string rfcSolicitante, string idSolicitud)
        {
            string canonicalTimestamp =
                "<des:VerificaSolicitudDescarga xmlns:des=\"http://DescargaMasivaTerceros.sat.gob.mx\">"
                + "<des:solicitud IdSolicitud=\"" + idSolicitud + "\" RfcSolicitante=\"" + rfcSolicitante + ">"
                + "</des:solicitud>"
                + "</des:VerificaSolicitudDescarga>";

            string digest = CreateDigest(canonicalTimestamp);

            string canonicalSignedInfo = @"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                         @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>" +
                                         @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>" +
                                         @"<Reference URI=""#_0"">" +
                                         "<Transforms>" +
                                         @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>" +
                                         "</Transforms>" +
                                         @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>" +
                                         "<DigestValue>" + digest + "</DigestValue>" +
                                         "</Reference>" +
                                         "</SignedInfo>";
            string signature = Sign(canonicalSignedInfo, certificate);
            string soap_request =
                @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">" +
                @"<s:Header/>" +
                @"<s:Body>" +
                @"<des:VerificaSolicitudDescarga>" +
                @"<des:solicitud " +
                @"IdSolicitud=""" + idSolicitud +
                @""" RfcSolicitante=""" + rfcSolicitante +
                @""">" +
                @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                @"<SignedInfo>" +
                @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
                @"<Reference URI=""#_0"">" +
                @"<Transforms>" +
                @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                @"</Transforms>" +
                @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
                @"<DigestValue>" + digest + @"</DigestValue>" +
                @"</Reference>" +
                @"</SignedInfo>" +
                @"<SignatureValue>" + signature + "</SignatureValue>" +
                @"<KeyInfo>" +
                @"<X509Data>" +
                @"<X509IssuerSerial>" +
                @"<X509IssuerName>" + certificate.Issuer +
                @"</X509IssuerName>" +
                @"<X509SerialNumber>" + certificate.SerialNumber +
                @"</X509SerialNumber>" +
                @"</X509IssuerSerial>" +
                @"<X509Certificate>" + Convert.ToBase64String(certificate.RawData) + "</X509Certificate>" +
                @"</X509Data>" +
                @"</KeyInfo>" +
                @"</Signature>" +
                @"</des:solicitud>" +
                @"</des:VerificaSolicitudDescarga>" +
                @"</s:Body>" +
                @"</s:Envelope>";
            xml = soap_request;
            return soap_request;
        }

        #endregion

        public override StepResponse GetResult(XmlDocument xmlDoc, string step)
        {

            var stepResponse = new StepResponse()
            {
                ResponseStatusCode = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes?["CodEstatus"]?.Value,
                ResponseStatusMessage = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes?["Mensaje"]?.Value,
                QtyCfdi = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes?["NumeroCFDIs"]?.Value,
                CodeStatusRequest = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes?["CodigoEstadoSolicitud"]?.Value,
                Situation = xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes?["EstadoSolicitud"]?.Value
            };


            if (stepResponse.Situation == null || !stepResponse.Situation.Equals("3")) return stepResponse;

            //Get all packages
            var packageList = xmlDoc.GetElementsByTagName("IdsPaquetes");
            for (var i = 0; i < packageList.Count; i++)
            {
                stepResponse.PackageIdentifierList.Add(packageList[i].InnerXml);
                   
            }

            return stepResponse;
        }
    }
}