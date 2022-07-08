public class Intercambio : Action
{
    public readonly int fichas_devueltas;
    public readonly int fichas_tomadas;
    public Intercambio(string autor, int fichas_devueltas, int fichas_tomadas) : base(autor)
    {
        this.fichas_devueltas = fichas_devueltas;
        this.fichas_tomadas = fichas_tomadas;
    }
    public override string ToString()
    {
        return "Intercambio " + this.autor + " -" + fichas_devueltas.ToString() + " +" + fichas_tomadas.ToString();
    }
}