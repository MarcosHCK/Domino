public abstract class Jugador
{
    public readonly string nombre;
    protected Reglas_del_Juego reglas;
    public Jugador(string nombre)
    {
        this.nombre = nombre;
    }
    public abstract Jugada Jugar(Estado estado, List<Ficha> mano);
    public void Iniciar(Reglas_del_Juego reglas)
    {
        this.reglas = reglas;
    }
    public abstract List<Ficha> Descartar(Cambiador Cambiador, Estado Estado, List<Ficha> mano);
}