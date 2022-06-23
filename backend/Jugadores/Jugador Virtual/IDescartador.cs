public interface IDescartador
{
    List<Ficha> Descartar(Cambiador Cambiador, Estado Estado, List<Ficha> mano, Reglas_del_Juego reglas, double[] importancia_de_cada_valor);
    //Con el ultimo parametro dejamo abierta la posibilidad de descartar lo mas inteligentemente que se pueda 
}
public class Descartador_Random : IDescartador
{
    Random Azar;
    public Descartador_Random(){this.Azar = new Random();}
    public List<Ficha> Descartar(Cambiador Cambiador, Estado Estado, List<Ficha> mano, Reglas_del_Juego reglas, double[] importancia_de_cada_valor)
    {
        if((mano == null) || (mano.Count == 0))return new List<Ficha>();
        List<Ficha> descartes = new List<Ficha>();
        if(Cambiador is Cambiador_Por_Balance)
        {
            Cambiador_Por_Balance cambiador = (Cambiador_Por_Balance)Cambiador;
            for (;(descartes.Count < cambiador.Descartes_Obligatorios)&&(mano.Count > descartes.Count); Descartar_Random());
        }else
        {
            Cambiador_por_Cant_de_Fichas cambiador = (Cambiador_por_Cant_de_Fichas)Cambiador;
            for (; cambiador.cant_de_fichas < mano.Count; Descartar_Random());
        }
        return descartes;
        void Descartar_Random()
        {
            Ficha ficha = mano[Azar.Next(mano.Count)];
            mano.Remove(ficha);
            descartes.Add(ficha);
        }
    }
}