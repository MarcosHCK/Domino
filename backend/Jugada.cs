public class Jugada
{
    public string autor{get; private set;}
    Ficha _ficha;
    public int cabeza_usada{get; private set;}
    public int cara_de_la_mesa{get; private set;}
    public Jugada(string autor, Ficha _ficha = null, int cabeza_usada = -1, int cara_de_la_mesa = -1)
    {
        this.autor = autor;
        this._ficha = _ficha;
        this.cabeza_usada = cabeza_usada;
        this.cara_de_la_mesa = cara_de_la_mesa;
    }
    public Jugada(Jugada otra)
    {
        this.autor = otra.autor;
        this._ficha = (otra.EsPase)?null:otra.ficha;
        this.cabeza_usada = (otra.EsPase)?-1:otra.cabeza_usada;
        this.cara_de_la_mesa = (otra.EsPase)?-1:otra.cara_de_la_mesa;
    }
    public Ficha ficha
    {
        get
        {
            return new Ficha(this._ficha);
        }
    }
    public bool EsPase
    {
        get
        {
            return (this._ficha == null);
        }
    }
    public override string ToString()
    {
        if(this.EsPase)return this.autor + " se pasa";
        if(this.cara_de_la_mesa == -1)return this.autor + " coloca" + this._ficha.ToString();
        return this.autor + " coloca " + this.ficha.ToString() + " por la cabeza " + this.cabeza_usada.ToString() + " por el" + this.cara_de_la_mesa.ToString();
    }
}