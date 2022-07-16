public class Banquero
{
    Dictionary<string, List<Ficha>> manos;
    public List<Ficha> fichas_fuera;
    List<Ficha> fichas_validas;
    Organizador organizador;
    Estado estado;
    Puntuador puntuador;
    public Banquero(Estado estado, Organizador organizador, List<Ficha> fichas_validas, Puntuador puntuador)
    {
        this.fichas_validas = fichas_validas;
        this.fichas_fuera = new List<Ficha>(fichas_validas);
        Util.DarAgua(fichas_fuera);
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
        Dictionary<string, int> fichas_por_mano = estado.fichas_por_mano;
        Cambiador Cambiador_de_Descarte = estado.Cambiadores_de_Repartir.Item1;
        Cambiador Cambiador_de_Robo = estado.Cambiadores_de_Repartir.Item2;
        foreach(string nombre in estado.jugadores)
            this.Refrescar(organizador[nombre], Cambiador_de_Descarte);
        foreach(string nombre in estado.jugadores)
        {
            if(Cambiador_de_Robo is Cambiador_por_Cant_de_Fichas)
                if (((Cambiador_por_Cant_de_Fichas)Cambiador_de_Robo).Cuantas_Tenia)
                {
                    this.Refrescar(organizador[nombre], new Cambiador_por_Cant_de_Fichas((Cambiador_por_Cant_de_Fichas)Cambiador_de_Robo, fichas_por_mano[nombre]));
                    continue;
                }
            this.Refrescar(organizador[nombre], Cambiador_de_Robo);
        }
    }
    public void Refrescar(Jugador jugador = null, Cambiador Cambiador = null)
    {
        if(Cambiador == null)Cambiador = this.estado.Cambiador_de_Refrescar;
        if(jugador == null)jugador = this.organizador[this.estado.Jugador_en_Turno];
        estado.Actualizar(this.DarMano(jugador, Cambiador, this.puntuador));
    }
    Intercambio DarMano(Jugador jugador, Cambiador Cambiador, Puntuador puntuador)
    { 
        List<Ficha> descartes;
        if(Cambiador is Cambiador_Por_Balance)
        {
            Cambiador_Por_Balance cambiador = (Cambiador_Por_Balance)Cambiador;
            if((cambiador.Balance < 0) && (Math.Abs(cambiador.Balance) >= this.manos[jugador.nombre].Count))
            {
                this.fichas_fuera.AddRange(this.manos[jugador.nombre]);
                this.manos[jugador.nombre] = new List<Ficha>();
                return new Intercambio(jugador.nombre, this.manos[jugador.nombre].Count, 0);
            }
            do
            {
                descartes = jugador.Descartar(cambiador, estado, this.manos[jugador.nombre]);
            }while((descartes.Count < cambiador.Descartes_Obligatorios) || (descartes.Count > cambiador.Descartes_Permitidos));
            foreach (Ficha ficha in descartes)
            {
                this.manos[jugador.nombre].Remove(ficha);
                this.fichas_fuera.Add(ficha);
            }
            List<Ficha> recogidas = cambiador.Reemplazar(fichas_fuera, descartes, puntuador, this.manos[jugador.nombre].Count);
            //Al reemplazar se borran las piezas del fichas fuera porq el Cambiador necesita borrar una ficha para saber que ya la dio
            //De todas formas puedo tratar eliminarlas de todas formas y ver si no da excepcion; para asegurar la eliminacion de las fichas aqui mismo
            this.manos[jugador.nombre].AddRange(recogidas);
            return new Intercambio(jugador.nombre, descartes.Count, recogidas.Count);
        }else
        {
            Cambiador_por_Cant_de_Fichas cambiador = (Cambiador_por_Cant_de_Fichas)Cambiador;
            if(cambiador.cant_de_fichas == 0)
            {
                int aux = this.manos[jugador.nombre].Count;
                this.fichas_fuera.AddRange(this.manos[jugador.nombre]);
                this.manos[jugador.nombre] = new List<Ficha>();
                return new Intercambio(jugador.nombre, aux, 0);
            }
            for (descartes = new List<Ficha>(); cambiador.cant_de_fichas < this.manos[jugador.nombre].Count; descartes = jugador.Descartar(cambiador, estado, this.manos[jugador.nombre]));
            foreach (Ficha ficha in descartes)
            {
                this.manos[jugador.nombre].Remove(ficha);
                this.fichas_fuera.Add(ficha);
            }
            List<Ficha> recogidas = cambiador.Reemplazar(fichas_fuera, descartes, puntuador, this.manos[jugador.nombre].Count);
            //Al reemplazar se borran las piezas del fichas fuera porq el Cambiador necesita borrar una ficha para saber que ya la dio
            //De todas formas puedo tratar eliminarlas de todas formas y ver si no da excepcion; para asegurar la eliminacion de las fichas aqui mismo
            this.manos[jugador.nombre].AddRange(recogidas);
            return new Intercambio(jugador.nombre, descartes.Count, recogidas.Count);
        }
    }
}