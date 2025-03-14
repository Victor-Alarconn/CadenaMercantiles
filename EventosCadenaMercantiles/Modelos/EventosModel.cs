using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventosCadenaMercantiles.Modelos
{
    public class EventosModel
    {
        public int IdEventos { get; set; }
        public string EvenDocum { get; set; } // numero de documento
        public string EvenReceptor { get; set; } // nombre del receptor
        public string EvenIdentif { get; set; } // identificacion del receptor nit o cedula
        public string NombreEmisor { get; set; } // nombre del emisor
        public string CorreoEmisor { get; set; } // correo del emisor
        public string EvenFecha { get; set; } // fecha del evento
        public string EvenEvento { get; set; } // tipo de evento
        public string EvenCufe { get; set; } // cufe del documento
        public string EvenCodigo { get; set; } // codigo de respuesta exitoso o error / preparado /esperado
        public string EvenResponse { get; set; } // respuesta del evento
        public string EvenQrcode { get; set; } // link del cufe ante la dian 
        public string EvenXmlb64 { get; set; } // xml en base 64
    }
}
