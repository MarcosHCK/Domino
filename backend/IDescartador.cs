public interface IDescartador
{
    List<Ficha> Descartar(ICriterio_de_cambio criterio, List<Ficha> mano, int cambios_restantes, int rondas_de_cambios_restantes, Reglas_del_Juego reglas, int[] importancia_de_cada_valor);
    //Con el ultimo parametro dejamo abierta la posibilidad de descartar lo mas intelientemente que se pueda 
}
public class Descartador_Vacio : IDescartador
{
    public List<Ficha> Descartar(ICriterio_de_cambio criterio, List<Ficha> mano, int cambios_restantes, int rondas_de_cambios_restantes, Reglas_del_Juego reglas, int[] importancia_de_cada_valor)
    {
        return new List<Ficha>();
    }
}