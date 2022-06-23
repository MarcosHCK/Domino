//Esto separa del contexto la accion de pegar dos fichas, no importa si el jugador la tiene o no ni ninguna otra cosa, solo si tales fichas pueden pegarse o no
public interface IEmparejador
{
    bool EsValidoColocar(Ficha ficha, int cabeza_de_la_ficha, int cara_de_la_mesa);
}