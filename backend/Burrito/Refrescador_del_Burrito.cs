public class Refrescador_del_Burrito : IRefrescador
{
    public ICriterio_de_cambio Get_Criterios_de_Refrescar(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        if((!estado.YaSeHaJugado) || (estado.fichas_fuera == 0))return null;
        foreach(Jugada jugada in Util.PosiblesJugadas(estado, mano))
            if(reglas.EsValida(jugada, estado, mano))return null;
        return new Criterio_de_cambio_Random(mano.Count + 1);
    }
    public ICriterio_de_cambio Get_Criterios_de_Repartir(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        if(estado.YaSeHaJugado)return null;
        foreach (KeyValuePair<string, int> tupla in estado.fichas_por_mano)
            if(tupla.Value == 0)return new Criterio_de_cambio_Random(reglas.fichas_por_mano);
        return null;
    }
}