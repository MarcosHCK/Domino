public interface ICriterio_de_Intercambio
{
    List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, int fichas_a_tomar, Puntuador puntuador);
}
//bool EsValido(List<Ficha> descartes);