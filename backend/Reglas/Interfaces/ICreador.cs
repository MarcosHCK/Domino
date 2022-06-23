public interface ICreador
{
    List<Ficha> fichas(int data_tope, int cabezas_por_ficha);
    int cant_de_fichas(int data_tope, int cabezas_por_ficha);
    int cant_d_fichas{get;}
}