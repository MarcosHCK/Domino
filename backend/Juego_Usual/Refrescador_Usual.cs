public class Refrescador_Usual : IRefrescador
{
    public ICriterio_de_cambio Get_Criterios_de_Refrescar(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        return null;
    }
    public ICriterio_de_cambio Get_Criterios_de_Repartir(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        if(estado.YaSeHaJugado)return null;
        foreach (KeyValuePair<string, int> tupla in estado.fichas_por_mano)
            if(tupla.Value == 0)return new Criterio_de_cambio_Random(reglas.fichas_por_mano);
        return null;
    }
}