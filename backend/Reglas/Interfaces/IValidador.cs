public interface IValidador
{
    bool EsValida(Jugada jugada, Reglas_del_Juego reglas, Estado estado, List<Ficha> mano);
}