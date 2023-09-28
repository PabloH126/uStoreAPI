using uStoreAPI.ModelsAzureDB;

namespace uStoreAPI.Dtos
{
    public class filtrosGraficaDto
    {
        public bool isTienda { get; set; }
        public List<int> categorias { get; set; }
        public string periodoTiempo {  get; set; }

    }
}
