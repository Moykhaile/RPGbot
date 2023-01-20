using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGbot.db
{
    public static class PlayerClasses
    {
        public static BarbaroClass Barbaro { get; }
        public static BardoClass Bardo { get; }
        public static BruxoClass Bruxo { get; }
        public static ClerigoClass Clerigo { get; }
        public static DruidaClass Druida { get; }
        public static FeiticeiroClass Feiticeiro { get; }
        public static GuerreiroClass Guerreiro { get; }
        public static LadinoClass Ladino { get; }
        public static MagoClass Mago { get; }
        public static MongeClass Monge { get; }
        public static PaladinoClass Paladino { get; }
        public static PatrulheiroClass Patrulheiro { get; }
    }

    public interface PlayerClass
    {
        string Fname { get; }
        string Mname { get; }
        int dice { get; }
    }

    public class BarbaroClass : PlayerClass
    {
        public string Mname => "Bárbaro";
        public string Fname => "Bárbara";
        public int dice => 12;
    }
    public class BardoClass : PlayerClass
    {
        public string Mname => "Bardo";
        public string Fname => "Barda";
        public int dice => 8;
    }
    public class BruxoClass : PlayerClass
    {
        public string Mname => "Bruxo";
        public string Fname => "Bruxa";
        public int dice => 8;
    }
    public class ClerigoClass : PlayerClass
    {
        public string Mname => "Clérigo";
        public string Fname => "Clériga";
        public int dice => 8;
    }
    public class DruidaClass : PlayerClass
    {
        public string Mname => "Druida";
        public string Fname => "Druidesa";
        public int dice => 8;
    }
    public class FeiticeiroClass : PlayerClass
    {
        public string Mname => "Feiticeiro";
        public string Fname => "Feiticeira";
        public int dice => 6;
    }
    public class GuerreiroClass : PlayerClass
    {
        public string Mname => "Guerreiro";
        public string Fname => "Guerreira";
        public int dice => 10;
    }
    public class LadinoClass : PlayerClass
    {
        public string Mname => "Ladino";
        public string Fname => "Ladina";
        public int dice => 8;
    }
    public class MagoClass : PlayerClass
    {
        public string Mname => "Mago";
        public string Fname => "Maga";
        public int dice => 6;
    }
    public class MongeClass : PlayerClass
    {
        public string Mname => "Monge";
        public string Fname => "Monja";
        public int dice => 8;
    }
    public class PaladinoClass : PlayerClass
    {
        public string Mname => "Paladino";
        public string Fname => "Paladina";
        public int dice => 10;
    }
    public class PatrulheiroClass : PlayerClass
    {
        public string Mname => "Patrulheiro";
        public string Fname => "Patrulheira";
        public int dice => 10;
    }
}
