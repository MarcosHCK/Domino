public class Ficha
{
    public readonly bool EsDoble;
    int[] _cabezas;
    public Ficha(params int[] _cabezas)
    {
        this._cabezas = _cabezas;
        int[] temp = this.cabezas;
        System.Array.Sort(temp);
        this.EsDoble = false;
        for(int i = 1; i < this._cabezas.Length; i++)
            if(temp[i] == temp[i - 1])
            {
                this.EsDoble = true;
                break;
            }
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
        return (this.ToString() == obj.ToString());
    }
}