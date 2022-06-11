public class Jugador_Random : Jugador_Virtual
{
    Random Azar;
    public Jugador_Random(string nombre) : base(nombre)
    {
        this.Azar = new Random();
    }
    protected override double Valorar(Jugada jugada, Estado estado, List<Ficha> mano)
    {
        return Azar.NextDouble();
    }
    protected override Jugada Apertura(List<Ficha> mano)
    {
        return new Jugada(this.nombre, mano[Azar.Next(mano.Count)]);
    }
}