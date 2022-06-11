public class Intercambio : Action
{
    public int fichas_devueltas{get; private set;}
    public int fichas_tomadas{get; private set;}
    public Intercambio(string autor, int fichas_devueltas, int fichas_tomadas) : base(autor)
    {
        this.fichas_devueltas = fichas_devueltas;
        this.fichas_tomadas = fichas_tomadas;
    }
    public override string ToString()
    {
        string retorno = this.autor;
        if (this.fichas_devueltas != 0)
        {
            retorno += " descarta " + fichas_devueltas.ToString() + " fichas";
            if (this.fichas_tomadas != 0)retorno += " y ";
        }
        if (this.fichas_tomadas != 0)retorno += " toma " + fichas_tomadas.ToString() + "fichas";
        return retorno;
    }
}