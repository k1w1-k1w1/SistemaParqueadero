namespace SistemaParqueadero.WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            app.UseDefaultFiles();   // busca index.html automáticamente
            app.UseStaticFiles();    // habilita wwwroot

            app.Run();
        }
    }
}
