public class Banquero
{
    Dictionary<string, List<Ficha>> manos;
    List<Ficha> fichas_fuera;
    List<Ficha> fichas_validas;
    Organizador organizador;
    Estado estado;
    Puntuador puntuador;
    public Banquero(Estado estado, Organizador organizador, List<Ficha> fichas_validas, Puntuador puntuador)
    {
        this.fichas_validas = fichas_validas;
        this.fichas_fuera = new List<Ficha>(fichas_validas);
        this.organizador = organizador;
        this.estado = estado;
        this.puntuador = puntuador;
        this.manos = new Dictionary<string, List<Ficha>>();
        foreach (string nombre in organizador.jugadores)
            manos.Add(nombre, new List<Ficha>());
        Repartir();
    }
    public List<Ficha> GetMano(string nombre)
    {
        return new List<Ficha>(this.manos[nombre]);
    }
    public List<Ficha> Actualizar(Jugada jugada)
    {
        if(!jugada.EsPase)this.manos[jugada.autor].Remove(jugada.ficha);
        return this.GetMano(jugada.autor);
    }
    public void Repartir()
    {
        List<string> jugadores = organizador.jugadores;
        Util.DarAgua(jugadores);//La refrescadera tiene orden Random siempre, por ahora; es posible crear un criterio para esto
        this.estado.Actualizar(new Reparticion());
        foreach (string nombre in jugadores)
            Refrescar(organizador[nombre], this.estado.Cambiador_de_Repartir);
    }
    public void Refrescar(Jugador jugador = null, Cambiador Cambiador = null)
    {
        if(Cambiador == null)Cambiador = this.estado.Cambiador_de_Refrescar;
        if(jugador == null)jugador = this.organizador[this.estado.Jugador_en_Turno];
        estado.Actualizar(Negociador.DarMano(jugador, estado, manos[jugador.nombre], fichas_fuera, Cambiador, this.puntuador));
    }
}