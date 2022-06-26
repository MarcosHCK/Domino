public class Creador_Cartas : Creador_Usual
{
    public Creador_Cartas(bool dobles, int repeticiones = 1)
    : base(true, repeticiones){}
    public virtual List<Ficha> fichas(int data_tope, int cabezas_por_ficha)
    {
        if(this._fichas.Count != 0)return new List<Ficha>(this._fichas);
        data_tope = 14;
        cabezas_por_ficha = 2;
        for(int i = 0; i < 4; i++)
            for(int j = 0; j < data_tope; j++)
                _fichas.Add(new Ficha(i, j));
        return this.fichas(data_tope, cabezas_por_ficha);
    }
}
