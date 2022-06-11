public class GameOver_Usual : IGameOver
{
    public bool GameOver(Estado estado, List<Ficha> mano_del_ultimo_en_jugar)
    {
        if(estado.jugadas.Count == 0)return false;
        if(mano_del_ultimo_en_jugar.Count == 0)return true;
        bool flag = false;
        foreach(Jugada jugada in estado.jugadas.Reverse<Jugada>())
            if(!jugada.EsPase)return false;
            else if(jugada.autor == estado.Jugador_en_Turno)
            {
                if(flag)return true;
                flag = true;
            }
        return false;//aun no se ha completado un ciclo de juego
    }
}