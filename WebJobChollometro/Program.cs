using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebJobChollometro.Data;
using WebJobChollometro.Repositories;

string connectionString = "Data Source=LOCALHOST\\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA";

var provider = new ServiceCollection()
    .AddTransient<RepositoryChollos>()
    .AddDbContext<ChollometroContext>(options => options.UseSqlServer(connectionString))
    .BuildServiceProvider();

//Recuperamos nuestro repository utilizando el provider
RepositoryChollos repo = provider.GetService<RepositoryChollos>();

Console.WriteLine("Pulse ENTER para iniciar");
Console.ReadLine();
repo.PopulateChollos();
Console.WriteLine("Proceso completado");
Console.ReadLine();