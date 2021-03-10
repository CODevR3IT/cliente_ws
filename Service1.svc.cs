using System;
using System.IO;
using System.Net;



namespace cliente_ws
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            string folio = "A-COM-2021-10300";
            RecibeAvaluo.WS_Recibe_AvaluoClient cliente = new RecibeAvaluo.WS_Recibe_AvaluoClient();
            AplicarAcceso(ref cliente);
            solicitarToken();
            
            System.Threading.Thread.Sleep(5000);
            var url = $"http://ovica.linesolutions.tech/avaluosNew_backend/public/WsSolucionIdeas/tokenG?folio_Interno={folio}";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var response = request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string token = reader.ReadToEnd();

                RecibeAvaluo.DatosRegistroAvaluo instancia = new RecibeAvaluo.DatosRegistroAvaluo();
                instancia.AvaluoXML = "CADENA XML del Avaluo";
                instancia.Folio_Interno = folio;
                instancia.Folio_Usuario = "U4566";
                instancia.token = token;
                if (cliente.BandejaAvaluoXML(instancia))
                {
                    return "El avaluo fue entregado";
                }
                else
                {
                    return "El avaluo no fue entregado";
                }
            }
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        protected void AplicarAcceso(ref RecibeAvaluo.WS_Recibe_AvaluoClient servicio) {
            string usr = "Usrpruebas";
            string psw = "qa7350r3rlac6mx-45";
            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(servicio.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header;
            header = System.ServiceModel.Channels.MessageHeader.CreateHeader("usuario", "IDEAS.Avametrica",Abase64(usr));
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);
            header = System.ServiceModel.Channels.MessageHeader.CreateHeader("contrasenia", "IDEAS.Avametrica", Abase64(psw));
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);
        }

        protected string Abase64(String cadena) {
            Byte[] byt = System.Text.Encoding.UTF8.GetBytes(cadena);
            return Convert.ToBase64String(byt);
        }

        protected void solicitarToken() {
            try
            {
                RecibeAvaluo.WS_Recibe_AvaluoClient servicio = new RecibeAvaluo.WS_Recibe_AvaluoClient();
                AplicarAcceso(ref servicio);
                servicio.obtenertoken("A-COM-2021-10300");
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
