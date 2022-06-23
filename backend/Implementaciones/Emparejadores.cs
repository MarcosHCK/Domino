public class Emparejador_Permisivo : IEmparejador
{
    public bool EsValidoColocar(Ficha ficha, int cabeza_usada, int cara_de_la_mesa)
    {
        return ficha.cabezas.Contains(cabeza_usada);
    }
}
public class Emparejador_Usual : IEmparejador
{
    public bool EsValidoColocar(Ficha ficha, int cabeza_usada, int cara_de_la_mesa)
    {
        return (ficha.cabezas.Contains(cabeza_usada) && (cabeza_usada == cara_de_la_mesa));
    }
}
public class Emparejador_Pro_Dobles : IEmparejador
{
    public bool EsValidoColocar(Ficha ficha, int cabeza_usada, int cara_de_la_mesa)
    {
        return (ficha.cabezas.Contains(cabeza_usada) && (ficha.EsDoble));
    }
}
public class Emparejador_Sucesor : IEmparejador
{
    protected int data_tope;
    public Emparejador_Sucesor(int data_tope)
    {
        this.data_tope = data_tope;
    }
    public virtual bool EsValidoColocar(Ficha ficha, int cabeza_usada, int cara_de_la_mesa)
    {
        return (ficha.cabezas.Contains(cabeza_usada) && (cabeza_usada == ((cara_de_la_mesa + 1)%data_tope)));
    }
}
public class Emparejador_DobleModular : Emparejador_Sucesor
{
    public Emparejador_DobleModular(int data_tope) : base(data_tope){}
    public override bool EsValidoColocar(Ficha ficha, int cabeza_usada, int cara_de_la_mesa)
    {
        return (ficha.cabezas.Contains(cabeza_usada) && (cabeza_usada == ((cara_de_la_mesa*2)%data_tope)));
    }
}