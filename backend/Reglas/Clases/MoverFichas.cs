//EN cualquier situacion de juego dada, un IRefrescador se encarga de actualizar la informacion sobre si hay que Refrescar o Repartir, y en tal caso, cual criterio usar
public class MoverFichas
{
    DaCriterio<Cambiador> Refrescador;
    DaCriterio<Cambiador> Repartidor;
    public MoverFichas(DaCriterio<Cambiador> Refrescador, DaCriterio<Cambiador> Repartidor)
    {
        this.Refrescador = Refrescador;
        this.Repartidor = Repartidor;
    }
    public Cambiador Get_Cambiador_de_Refrescar(Estado estado, List<Ficha> mano)
    {
        return Refrescador.Criterio(estado, mano);
    }
    public Cambiador Get_Cambiador_de_Repartir(Estado estado, List<Ficha> mano)
    {
        return Repartidor.Criterio(estado, mano);
    }
}