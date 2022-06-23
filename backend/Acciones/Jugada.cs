public class Jugada : Action
{
    public readonly Ficha ficha;
    public readonly int cabeza_usada;
    public readonly int cara_de_la_mesa;
    public readonly int fichas_restantes;
    public bool EsPase{get{return (ficha == null);}}
    public Jugada(string autor, Ficha ficha = null, int cabeza_usada = -1, int cara_de_la_mesa = -1, int fichas_restantes = -1) : base(autor)
    {
        this.ficha = ficha;
        this.fichas_restantes = fichas_restantes;
        this.cabeza_usada = cabeza_usada;
        this.cara_de_la_mesa = cara_de_la_mesa;
    }
    public override string ToString()
    {
        if(this.EsPase)return this.autor + " se pasa";
        if(this.cara_de_la_mesa == -1)return this.autor + " coloca" + this.ficha.ToString();
        return this.autor + " coloca " + this.ficha.ToString() + " por la cabeza " + this.cabeza_usada.ToString() + " por el " + this.cara_de_la_mesa.ToString() + " quedandole " + this.fichas_restantes.ToString() + " fichas.";
    }
}