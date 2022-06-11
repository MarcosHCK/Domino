public class Refrescador_Usual : IRefrescador
{
    public ICriterio_de_cambio Get_Criterios_de_Refrescar(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        return null;
    }
    public ICriterio_de_cambio Get_Criterios_de_Repartir(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        return (estado.jugadas.Count == 0)?reglas.Criterios_de_Cambio[0]:null;
    }
}