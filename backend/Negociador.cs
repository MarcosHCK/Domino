public static class Negociador
{
    public static void DarMano(Jugador jugador, List<Ficha> mano, List<Ficha> fichas_fuera, ICriterio_de_cambio criterio, int fichas_por_mano)
    {
        int cambios_restantes = criterio.cambios_permitidos;
        int rondas_de_cambios_restantes = criterio.rondas_de_cambios_permitidas;
        for(; (cambios_restantes > 0) && (rondas_de_cambios_restantes > 0); rondas_de_cambios_restantes--)
        {
            List<Ficha> descartes = jugador.Descartar(criterio, mano, cambios_restantes, rondas_de_cambios_restantes);
            if ((descartes.Count == 0) && (mano.Count == fichas_por_mano))return;
            if (descartes.Count > cambios_restantes)throw new Exception("Mas cambios de los permitidos");
            foreach (Ficha ficha in descartes)
                mano.Remove(ficha);
            List<Ficha> recogidas = criterio.Reemplazar(fichas_fuera, descartes, fichas_por_mano - mano.Count);
            mano.AddRange(recogidas);
        }
    }
}
