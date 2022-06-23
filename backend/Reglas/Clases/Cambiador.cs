//Esta clase es una envoltura que contiene la informacion sobre la forma en que se realizara el cambio
public class Cambiador
{
    ICriterio_de_Intercambio Criterio_de_Intercambio;
    public readonly int Descartes_Obligatorios;
    public readonly int Descartes_Permitidos;
    public readonly int Balance;
    public Cambiador(ICriterio_de_Intercambio Criterio_de_Intercambio, int Descartes_Obligatorios, int Descartes_Permitidos, int Balance)
    {
        this.Criterio_de_Intercambio = Criterio_de_Intercambio;
        this.Descartes_Obligatorios = Descartes_Obligatorios;
        this.Descartes_Permitidos = Descartes_Permitidos;
        this.Balance = Balance;
    }
    public List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, Puntuador puntuador)
    {
        return this.Criterio_de_Intercambio.Reemplazar(fichas_fuera, descartes, this.Balance + descartes.Count, puntuador);
    }
}