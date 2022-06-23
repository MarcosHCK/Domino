public class Cambiador_Por_Balance : Cambiador
{
    public readonly int Descartes_Obligatorios;
    public readonly int Descartes_Permitidos;
    public readonly int Balance;
    public Cambiador_Por_Balance(ICriterio_de_Intercambio Criterio_de_Intercambio, int Descartes_Obligatorios, int Descartes_Permitidos, int Balance)
    : base(Criterio_de_Intercambio)
    {
        this.Descartes_Obligatorios = Descartes_Obligatorios;
        this.Descartes_Permitidos = Descartes_Permitidos;
        this.Balance = Balance;
    }
    public override List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, Puntuador puntuador, int fichas_mano)
    {
        return this.Criterio_de_Intercambio.Reemplazar(fichas_fuera, descartes, this.Balance + descartes.Count, puntuador);
    }
}