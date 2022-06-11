public class Banquero
{
    int data_tope;
    int cabezas_por_ficha;
    Dictionary<string, List<Ficha>> manos;
    List<Ficha> fichas_fuera;
    List<Ficha> fichas_validas;
    Organizador organizador;
    public Banquero(Organizador organizador, List<Ficha> fichas_validas, ICriterio_de_cambio criterio, int data_tope, int fichas_por_mano, int cabezas_por_ficha)
    {
        this.data_tope = data_tope;
        this.cabezas_por_ficha = cabezas_por_ficha;
        this.fichas_validas = fichas_validas;
        this.organizador = organizador;
        this.manos = new Dictionary<string, List<Ficha>>();
        foreach (string nombre in organizador.jugadores)
            manos.Add(nombre, new List<Ficha>());
        Repartir(criterio, fichas_por_mano, Desde_Cero : true);
    }
    public List<Ficha> GetMano(string nombre)
    {
        return new List<Ficha>(this.manos[nombre]);
    }
    public void Actualizar(Jugada jugada)
    {
        if(jugada.EsPase)return;
        this.manos[jugada.autor].Remove(jugada.ficha);
    }
    public void Repartir(ICriterio_de_cambio criterio, int fichas_por_mano = 0, bool Desde_Cero = false)
    {
        List<string> jugadores = new List<string>();
        foreach(var tupla in manos)
            jugadores.Add(tupla.Key);
        if(Desde_Cero)this.fichas_fuera = new List<Ficha>(fichas_validas);
        Util.DarAgua(jugadores);//La refrescadera tiene orden Random siempre, por ahora; es posible crear un criterio para esto
        foreach (string nombre in jugadores)
            Refrescar(organizador[nombre], criterio, fichas_por_mano);
    }
    public void Refrescar(Jugador jugador, ICriterio_de_cambio criterio, int fichas_por_mano = 0)
    {
        if (fichas_por_mano == 0)fichas_por_mano = manos[jugador.nombre].Count;
        Negociador.DarMano(jugador, manos[jugador.nombre], fichas_fuera, criterio, fichas_por_mano);
    }
    public int num_de_fichas_fuera
    {
        get
        {
            return fichas_fuera.Count;
        }
    }
}
