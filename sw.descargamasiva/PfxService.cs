using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.descargamasiva
{
    public class PfxService
    {
        private readonly string _tempDir;
        private readonly string _sslBin;
        private readonly Process process1;
        private readonly Process process2;
        private readonly Process process3;

        public PfxService(string tempDir, string sslBin)
        {
            this._tempDir = tempDir;
            this._sslBin = sslBin;

            process1 = new Process
            { EnableRaisingEvents = true, StartInfo = { WorkingDirectory = sslBin, FileName = "openssl" } };
            process2 = new Process
            { EnableRaisingEvents = true, StartInfo = { WorkingDirectory = sslBin, FileName = "openssl" } };
            process3 = new Process
            { EnableRaisingEvents = true, StartInfo = { WorkingDirectory = sslBin, FileName = "openssl" } };
        }

        public (bool, string) CreateCerPem(string cerPath, string fileName = "c.pem")
        {
            try
            {
                var fullFile = Path.Combine(_tempDir, fileName);
                //openssl x509 -inform DER -in certificado.cer -out certificado.pem
                process1.StartInfo.Arguments = $"x509 -inform DER -in {cerPath} -out {fullFile}";
                var b = process1.Start();
                process1.WaitForExit();
                //process1.Dispose();
                return (true, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (true, e.ToString());
            }
        }

        public (bool, string) CreateKeyPem(string keyPath, string keyPass, string fileName = "k.pem")
        {
            try
            {
                //openssl pkcs8 -inform DER -in llave.key -passin pass:a0123456789 -out llave.pem

                var fullFile = Path.Combine(_tempDir, fileName);
                process2.StartInfo.Arguments =
                    $"pkcs8 -inform DER -in {keyPath} -passin pass:{keyPass} -out {fullFile}";
                process2.Start();
                process2.WaitForExit();
                //  process2.Dispose();
                return (true, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (true, e.ToString());
            }
        }

        private (bool, string) CreatePfxByPemFiles(string keyPass, string cerPemPath = "c.pem",
            string keyPemPath = "k.pem", string fileName = "pfx.pfx")
        {
            try
            {
                //openssl pkcs12 -export -out fileName.pfx -inkey llave.pem -in certificado.pem -passout pass:clavedesalida

                var fullFile = Path.Combine(_tempDir, fileName);
                var cerPemFullFile = Path.Combine(_tempDir, cerPemPath);
                var keyPemFullFile = Path.Combine(_tempDir, keyPemPath);


                process3.StartInfo.Arguments =
                    $"pkcs12 -export -out {fullFile} -inkey {keyPemFullFile} -in {cerPemFullFile} -passout pass:{keyPass}";
                process3.Start();
                process3.WaitForExit();
                //process3.Dispose();
                return (true, fullFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (true, e.ToString());
            }
        }

        public (bool, byte[]) CreatePfx(string cerPath, string keyPath, string keyPass,
            string fileName = "pfxFile.pfx")
        {
            try
            {
                if (CreateCerPem(cerPath).Item1)
                {
                    if (CreateKeyPem(keyPath, keyPass).Item1)
                    {
                        var result = CreatePfxByPemFiles(keyPass);
                        if (result.Item1)
                        {
                            return (true, File.ReadAllBytes(result.Item2));
                        }
                    }
                }

                return (false, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (false, null);
            }
        }
    }
}