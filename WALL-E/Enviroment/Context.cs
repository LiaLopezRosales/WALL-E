public class Context
{
    public List<Fuction> Available_Functions{get;set;}
    public Dictionary<string,Func<double,double>> Trig_functions{get;}
    public Dictionary<string,Func<double>> Math_value{get;}
    public Dictionary<string,Func<double,double,double>> Log{get;}
    public Dictionary<string,Node> GlobalConstant{get;set;}
    public Context()
    {
        Available_Functions=new List<Fuction>();
        GlobalConstant=new Dictionary<string, Node>();
        Trig_functions = new Dictionary<string, Func<double,double>>();
        Trig_functions.Add("sin",(double argument)=>Math.Sin(argument));
        Trig_functions.Add("cos",(double argument)=>Math.Cos(argument));
        Trig_functions.Add("sqrt",(double argument)=>Math.Sqrt(argument));
        Math_value = new Dictionary<string, Func<double>>();
        Math_value.Add("PI",()=>Math.PI);
        Math_value.Add("E",()=>Math.E);
        Log = new Dictionary<string, Func<double, double, double>>();
        Log.Add("log",(double Base,double argument)=> Math.Log(argument,Base));
    }
}