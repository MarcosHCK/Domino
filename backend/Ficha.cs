public class Ficha
{
    int[] _cabezas;
    public Ficha(params int[] _cabezas)
    {
        this._cabezas = _cabezas;
    }
    public Ficha(Ficha otra)
    {
        this._cabezas = otra.cabezas;
    }
    public int[] cabezas
    {
        get
        {
            return (int[])(this._cabezas.Clone());
        }
    }
    public override string ToString()
    {
        string retorno = "( ";
        foreach(int cabeza in _cabezas)
            retorno += cabeza.ToString() + ", ";
        return retorno.Substring(0, retorno.Length - 2) + " )";
    }
    public override bool Equals(object? obj)
    {
        if((obj == null) || !(this.GetType().Equals(obj.GetType())))return false;
        else
        {
            int[] otra_cabezas = ((Ficha)obj)._cabezas;
            for(int i = 0; i < this._cabezas.Count(); i++)
               if (this._cabezas[i] != otra_cabezas[i])return false;
            return true;
        }
    }
}