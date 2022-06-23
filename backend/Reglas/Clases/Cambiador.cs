//Esta clase es una envoltura que contiene la informacion sobre la forma en que se realizara el cambio
public abstract class Cambiador
{
    public readonly ICriterio_de_Intercambio Criterio_de_Intercambio;
    public Cambiador(ICriterio_de_Intercambio Criterio_de_Intercambio)
    {
        this.Criterio_de_Intercambio = Criterio_de_Intercambio;
    }
    public abstract List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, Puntuador puntuador, int fichas_mano);
}