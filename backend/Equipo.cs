public class Equipo
{
    public string nombre{get; private set;}
    List<string> _miembros;
    public Equipo(string nombre, params string[] _miembros)
    {
        this.nombre = nombre;
        this._miembros = _miembros.ToList();
    }
    public List<string> miembros
    {
        get
        {
            return new List<string>(_miembros);
        }
    }
    public override bool Equals(object? obj)
    {
        if((obj == null) || !(this.GetType().Equals(obj.GetType())))return false;
        Equipo otro = (Equipo)obj;
        if(otro.nombre != this.nombre)return false;
        List<string> otro_miembros = otro.miembros;
        for (int i = 0; i < this._miembros.Count; i++)
            if(this._miembros[i] != otro_miembros[i])return false;
        return true;
    }
}