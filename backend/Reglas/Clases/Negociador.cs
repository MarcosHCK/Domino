public static class Negociador
{
    public static Intercambio DarMano(Jugador jugador, Estado Estado, List<Ficha> mano, List<Ficha> fichas_fuera, Cambiador Cambiador, Puntuador puntuador)
    {
        List<Ficha> descartes;
        if((Cambiador.Balance < 0) && (Math.Abs(Cambiador.Balance) >= mano.Count))
        {
            mano = new List<Ficha>();
            fichas_fuera.AddRange(mano);
            return new Intercambio(jugador.nombre, mano.Count, 0);
        }
        do
        {
            descartes = jugador.Descartar(Cambiador, Estado, mano);
        }while(((descartes.Count < Cambiador.Descartes_Obligatorios) && (descartes.Count < mano.Count)) || (descartes.Count > Cambiador.Descartes_Permitidos));
        foreach (Ficha ficha in descartes)
        {
            mano.Remove(ficha);
            fichas_fuera.Add(ficha);
        }
        List<Ficha> recogidas = Cambiador.Reemplazar(fichas_fuera, descartes, puntuador);
        //Al reemplazar se borran las piezas del fichas fuera porq el Cambiador necesita borrar una ficha para saber que ya la dio
        //De todas formas puedo tratar eliminarlas de todas formas y ver si no da excepcion; para asegurar la eliminacion de las fichas aqui mismo
        mano.AddRange(recogidas);
        return new Intercambio(jugador.nombre, descartes.Count, recogidas.Count);
    }
}