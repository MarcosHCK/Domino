/*public class Jugador_Complejo : Jugador_con_Memoria
{
    public Jugador_Complejo(string nombre, Full_Memoria memoria, IDescartador descartador = null)
    : base(nombre, memoria, descartador){}
    protected override Jugada Apertura(List<Ficha> mano)
    {

    }
}
public class Full_Memoria : Memoria_de_Pases
{
    Dictionary<int, Memoria_Data> Datas;
    public Full_Memoria()
    {
        this.Datas = new Dictionary<int, Memoria_Data>();
    }
    public override void Actualizar(Action accion)
    {
        base.Actualizar(accion);
        if(accion is Jugada)
        {
            Jugada jugada = (Jugada)accion;
            foreach(int cara in this.caras_de_la_mesa)
            {
                if(!Datas.ContainsKey(cara))Datas.Add(cara, new Memoria_Data(cara));
                Datas[cara].Actualizar(jugada);
            }
        }
    }
}
public class Jugador_Ficha
{
    public int jugadas;
    public int subidas;
    public int matadas;
}
public class Memoria_Data : Memoria
{
    readonly int data;
    public int Usadas{get; private set;}
    public Dictionary<string, Jugador_Ficha> Info_jugador;//Aqui esta toda la interaccion de un jugador respecto a una ficha
    public Memoria_Data(int data) : base()
    {
        this.data = data;
        Info_jugador = new Dictionary<string, Jugador_Ficha>();
    }
    public override void Actualizar(Action accion)
    {
        base.Actualizar(accion);
        if(accion is Jugada)
        {
            Jugada jugada = (Jugada)accion;
            if(jugada.EsPase)return;
            if(!Info_jugador.ContainsKey(jugada.autor))this.Info_jugador.Add(jugada.autor, new Jugador_Ficha());
            if(jugada.ficha.cabezas.Contains(data))Info_jugador[jugada.autor].jugadas++;
            if(jugada.cara_de_la_mesa == data)Info_jugador[jugada.autor].matadas++;
            else Info_jugador[jugada.autor].subidas++;
        }
    }
}*/