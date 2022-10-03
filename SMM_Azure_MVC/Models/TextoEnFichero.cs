namespace SMM_Azure_MVC.smm.Models
{
    public class TextoEnFichero
    {
        public string NombreFichero { get; set; }
        public string Texto { get; set; }
        public string Sentimiento { get; set; }
        public double PuntuacionNegativa { get; set; }
        public double PuntuacionNeutral { get; set; }
        public double PuntuacionPositiva { get; set; }
    }
}
