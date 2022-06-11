public class GameOver_Usual : IGameOver
{
    public bool GameOver(Estado estado, List<Ficha> mano_del_ultimo_en_jugar)
    {
        if(!estado.YaSeHaJugado)return false;
        if(mano_del_ultimo_en_jugar.Count == 0)return true;
        bool flag = false;
        List<Action> acciones = estado.acciones; 
        for(int i = acciones.Count - 1; i >= 0; i--)
            if(acciones[i] is Jugada)
            {
                Jugada jugada = (Jugada)acciones[i];
                if(!jugada.EsPase)return false;
                else if(jugada.autor == estado.Jugador_en_Turno)
                {
                    if(flag)return true;
                    flag = true;
                }
            }
        return false;//aun no se ha completado un ciclo de juego
    }
}