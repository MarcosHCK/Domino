public static class Negociador
{
    public static Intercambio[] DarMano(Jugador jugador, List<Ficha> mano, List<Ficha> fichas_fuera, ICriterio_de_cambio criterio)
    {
        List<Intercambio> retorno = new List<Intercambio>();
        int descartes_restantes = criterio.descartes_permitidos;
        int rondas_de_cambios_restantes = criterio.rondas_de_cambios_permitidas;
        int fichas_por_mano = criterio.fichas_por_mano;
        if (fichas_por_mano == 0)fichas_por_mano = mano.Count();
        for(; rondas_de_cambios_restantes > 0; rondas_de_cambios_restantes--)
        {
            List<Ficha> descartes = jugador.Descartar(criterio, mano, descartes_restantes, rondas_de_cambios_restantes);
            if ((descartes.Count == 0) && (mano.Count == criterio.fichas_por_mano))break;
            if (descartes.Count > descartes_restantes)throw new Exception("Mas descartes de los permitidos");
            foreach (Ficha ficha in descartes)
                mano.Remove(ficha);
            List<Ficha> recogidas = criterio.Reemplazar(fichas_fuera, descartes, fichas_por_mano - mano.Count);
            mano.AddRange(recogidas);
            retorno.Add(new Intercambio(jugador.nombre, descartes.Count, recogidas.Count));
        }
        return retorno.ToArray();
    }
}
