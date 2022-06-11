public class Banquero
{
    Dictionary<string, List<Ficha>> manos;
    List<Ficha> fichas_fuera;
    List<Ficha> fichas_validas;
    Organizador organizador;
    Estado estado;
    public Banquero(Estado estado, Organizador organizador, List<Ficha> fichas_validas)
    {
        this.fichas_validas = fichas_validas;
        this.fichas_fuera = new List<Ficha>(fichas_validas);
        this.organizador = organizador;
        this.estado = estado;
        this.manos = new Dictionary<string, List<Ficha>>();
        foreach (string nombre in organizador.jugadores)
            manos.Add(nombre, new List<Ficha>());
        Repartir(Desde_Cero : true);
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
    public void Repartir(bool Desde_Cero = false)
    {
        List<string> jugadores = organizador.jugadores;
        Util.DarAgua(jugadores);//La refrescadera tiene orden Random siempre, por ahora; es posible crear un criterio para esto
        foreach (string nombre in jugadores)
            Refrescar(organizador[nombre], this.estado.Criterio_De_Cambio_Repartir);
    }
    public void Refrescar(Jugador jugador = null, ICriterio_de_cambio criterio = null)
    {
        if(criterio == null)criterio = this.estado.Criterio_De_Cambio_Refrescar;
        if(jugador == null)jugador = this.organizador[this.estado.Jugador_en_Turno];
        estado.Actualizar(Negociador.DarMano(jugador, manos[jugador.nombre], fichas_fuera, criterio));
    }
}