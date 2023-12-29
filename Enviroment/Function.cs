public class Fuction
{
   public string Name{get;set;}
   public Dictionary<string,object> Functions_Arguments{get;set;} 

   public Node Code{get;set;}

   public Fuction(string name,Node code,Dictionary<string,object> functions_arg)
   {
     this.Name=name;
     this.Code=code;
     this.Functions_Arguments=functions_arg;

   }
}