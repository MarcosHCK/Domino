public class Criterio_de_cambio_Random : ICriterio_de_cambio
{
    Random Azar;
    public int descartes_permitidos{get; private set;}
    public int rondas_de_cambios_permitidas{get; private set;}
    public int fichas_por_mano{get; private set;}
    public Criterio_de_cambio_Random(int fichas_por_mano, int descartes_permitidos = 0, int rondas_de_cambios_permitidas = 1)
    {
        this.Azar = new Random();
        this.fichas_por_mano = fichas_por_mano;
        this.descartes_permitidos = descartes_permitidos;
        this.rondas_de_cambios_permitidas = rondas_de_cambios_permitidas;
    }
    public List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, int fichas_a_tomar)
    {
        fichas_fuera.AddRange(descartes);
        List<Ficha> retorno = new List<Ficha>();
        for (int index; retorno.Count < fichas_a_tomar; fichas_fuera.RemoveAt(index))
        {
            index = Azar.Next(fichas_fuera.Count);
            retorno.Add(fichas_fuera[index]);
        }
        return retorno;
    }
}