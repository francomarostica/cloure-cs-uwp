using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.properties
{
    public class Property
    {
        public int Id { get; set; }
        public DateTime? FechaAlta { get; set; }
        public bool Publicado { get; set; }
        public bool Destacado { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionInterna { get; set; }
        public int EstadoId { get; set; }
		public int OperacionId { get; set; }
		public string Operacion { get; set; }
		public double IVA { get; set; }
		public string AlquilerMoneda { get; set; }
		public double AlquilerPrecio { get; set; }
		public double AlquilerIva { get; set; }
		public double AlquilerImporte { get; set; }
		public double AlquilerComisionInicial { get; set; }
		public double AlquilerComisionMensual { get; set; }
		public string VentaMoneda { get; set; }
		public double VentaPrecio { get; set; }
		public double VentaIva { get; set; }
		public double VentaImporte { get; set; }
		public double VentaComision { get; set; }
		public double ExpensasImporte { get; set; }
		public double ExpensasPorcentaje { get; set; }
		public DateTime? ProxVto { get; set; }
		public string ProxVtoStr { get; set; }
		public string PeriodoPago { get; set; }
		public DateTime? ContratoAlta { get; set; }
		public DateTime ContratoVto { get; set; }

		public double Importe { get; set; }
		public string ImporteMoneda { get; set; }
		
		public bool Vencido { get; set; }
		public int DiasVencimiento { get; set; }

		public int TipoId { get; set; }
		public string Tipo { get; set; }
		public string Observaciones { get; set; }
		public string Imagen { get; set; }
		public string ImagenRuta { get; set; }
		public int ConsorcioId { get; set; }
		public string Consorcio { get; set; }
		public int AgenteId { get; set; }
		public string Agente { get; set; }
		public int PropietarioId { get; set; }
		public string Propietario { get; set; }
		public int LocatarioId { get; set; }
		public string Locatario { get; set; }
		public int CompradorId { get; set; }
		public string Comprador { get; set; }
		public int VendedorId { get; set; }
		public string Vendedor { get; set; }
		public int CanalId { get; set; }
		public int Antiguedad { get; set; }
		public bool AEstrenar { get; set; }
		public int ContratoMinimo { get; set; }
		public double SuperficieTotal { get; set; }
		public double SuperficieFrente { get; set; }
		public double SuperficieFondo { get; set; }
		public double SuperficieCubierta { get; set; }
		public string CuentaRentas { get; set; }
		public string CuentaMunicipalidad { get; set; }
		public string CuentaAgua { get; set; }
		public string CuentaLuz { get; set; }
		public string CuentaDFTA { get; set; }
		public string Matricula { get; set; }

		public int PH { get; set; }

		//public $Imagenes = array();
        //public $Pagos = array();
        //public $Garantes = array();

        public int PaisId { get; set; }
		public string Pais { get; set; }
		public int PaisN1Id { get; set; }
		public int PaisN2Id { get; set; }
		public int PaisN3Id { get; set; }
		public string PaisN1 { get; set; }
		public string PaisN2 { get; set; }
		public string PaisN3 { get; set; }
		public string Localidad { get; set; }
		public string Direccion { get; set; }
		public string CP { get; set; }
		public string Barrio { get; set; }
		public string Piso { get; set; }
		public string DtoOf { get; set; }

		public string Carpeta { get; set; }

		public int CarpetaSucursal { get; set; }
		public int CarpetaNumero { get; set; }
		public int CarpetaExtra { get; set; }

		public string CarpetaSucursalStr { get; set; }
		public string CarpetaNumeroStr { get; set; }
		public string CarpetaExtraStr { get; set; }

		public string NroRentas { get; set; }
		public string NroCtaMuni { get; set; }
		public string NroCtaAgua { get; set; }
		public string NroCatastro { get; set; }

    	public int TipoPublicacionId { get; set; }
    	public string TipoPublicacion { get; set; }

        public int CreadorId { get; set; }
        public string Creador { get; set; }

        public List<AvailableCommand> availableCommands = new List<AvailableCommand>();

        //Ambientes
        /*
		public $Dormitorios = 0;
		public $Baños = 0;
		public $Ambientes = 0;
		public $Cochera = 0;
		public $Quincho = 0;
		public $Piscina = 0;
		public $HabitacionServicio = 0;
		public $CocinaComedor = 0;
		public $Patio = 0;
		public $Jardin = 0;
        */
    }
}
