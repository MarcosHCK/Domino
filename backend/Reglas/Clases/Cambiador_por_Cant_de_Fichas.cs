public class Cambiador_por_Cant_de_Fichas : Cambiador
{
    public readonly int cant_de_fichas;
    public bool Cuantas_Tenia
    {
        get
        {
            return (cant_de_fichas == -1);
        }
    }
    public Cambiador_por_Cant_de_Fichas(ICriterio_de_Intercambio Criterio_de_Intercambio, int cant_de_fichas)
    : base(Criterio_de_Intercambio)
    {
        this.cant_de_fichas = cant_de_fichas;
    }
    public Cambiador_por_Cant_de_Fichas(Cambiador_por_Cant_de_Fichas Otro, int cant_de_fichas)
    : base(Otro.Criterio_de_Intercambio)
    {
        this.cant_de_fichas = cant_de_fichas;
    }
    public override List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, Puntuador puntuador, int fichas_mano)
    {
        return this.Criterio_de_Intercambio.Reemplazar(fichas_fuera, descartes, cant_de_fichas - fichas_mano, puntuador);
    }
}