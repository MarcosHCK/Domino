public class GameOver_del_Burrito : IGameOver
{
    IGameOver GameOver_Final;//Se encarga de valorar si el juego se acabo luego de que ya no hay fichas para repartir
    public GameOver_del_Burrito(IGameOver GameOver_Final)
    {
        this.GameOver_Final = GameOver_Final;
    }
    public bool GameOver(Estado estado, List<Ficha> mano_del_ultimo_en_jugar)
    {
        if(!estado.YaSeHaJugado)return false;
        if(mano_del_ultimo_en_jugar.Count == 0)return true;
        if(estado.fichas_fuera > 0)return false;
        return this.GameOver_Final.GameOver(estado, mano_del_ultimo_en_jugar);
    }
}