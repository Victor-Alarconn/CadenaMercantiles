using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventosCadenaMercantiles.Modelos
{
    public class DocumentoAdjunto
    {
        public string Emisor { get; set; }
        public string Identificacion { get; set; }
        public string Dv { get; set; }
        public string Idnit {  get; set; }
        public string Cufe { get; set; }
        public string QRCode { get; set; }
        public string PrefijoFactura { get; set; }
        public string XmlBase64 { get; set; }
    }

}
