//EN cualquier situacion de juego dada, un IRefrescador se encarga de actualizar la informacion sobre si hay que Refrescar o Repartir, y en tal caso, cual criterio usar
public interface IRefrescador
{
    ICriterio_de_cambio Get_Criterios_de_Refrescar(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano);
    ICriterio_de_cambio Get_Criterios_de_Repartir(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano);
}