public class Juego
{
    Organizador organizador;
    Estado estado;
    Reglas_del_Juego reglas;
    Banquero banquero;
    Portal_del_Banquero portal;
    public Juego(List<Jugador> jugadores, Reglas_del_Juego reglas, IOrdenador ordenador, params Equipo[] equipos)
    {
        this.reglas = reglas;
        this.organizador = new Organizador(jugadores, equipos.ToList(), ordenador);
        this.estado = new Estado(reglas, organizador.jugadores, organizador.equipos);
        this.banquero = new Banquero(organizador, reglas.fichas, estado.Criterio_De_Cambio_Repartir, reglas.data_tope, reglas.fichas_por_mano, reglas.cabezas_por_ficha);
        this.portal = new Portal_del_Banquero(banquero);
        foreach(Jugador jugador in jugadores)
            jugador.Iniciar(reglas, organizador.jugadores, organizador.equipos);
    }

    public IEnumerable<Jugada> Jugar
    {
        get
        {
            for (List<Ficha>? mano = null; !reglas.GameOver(this.estado, mano);)
            {
                estado.PasarTurno(reglas, mano, organizador.jugadores, organizador.equipos);
                Jugador jugador = organizador[estado.Jugador_en_Turno];
                if (estado.Criterio_De_Cambio_Repartir != null)banquero.Repartir(estado.Criterio_De_Cambio_Repartir);
                if (estado.Criterio_De_Cambio_Refrescar != null)banquero.Refrescar(jugador, estado.Criterio_De_Cambio_Refrescar);
                mano = portal[estado.Jugador_en_Turno];
                Jugada jugada = jugador.Jugar(new Estado(estado), mano);
                banquero.Actualizar(jugada);
                estado.Actualizar(new Jugada(jugada));
                mano = portal[estado.Jugador_en_Turno];
                yield return jugada;
            }
        }
    }
    public Dictionary<string, int> Puntuaciones
    {
        get
        {
            Dictionary<string, int> retorno = new Dictionary<string, int>();
            foreach (Equipo equipo in organizador.equipos)
                retorno.Add(equipo.nombre, reglas.Puntuar(equipo, portal));
            return retorno;   
        }
    }
}
