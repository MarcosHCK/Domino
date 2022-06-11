public interface ICriterio_de_cambio
{
    List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, int fichas_a_tomar);
    int cambios_permitidos{get;}
    int rondas_de_cambios_permitidas{get;}
}