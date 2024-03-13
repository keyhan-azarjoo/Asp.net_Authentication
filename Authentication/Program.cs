using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDataProtection();
//// Add services to the container.

//builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//// befor hitting the Api, the request hit here
//app.Use((ctx, next) =>
//{
//    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
//    var protector = idp.CreateProtector("auth-cookie");
//    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
//    var protectedpayload = authCookie.Split("=").Last();
//    var payload = protector.Unprotect(protectedpayload);
//    var parts = payload.Split(":");
//    var key = parts[0];
//    var value = parts[1];





//    var claims = new List<Claim>();
//    claims.Add(new Claim(key,value));
//    var identity = new ClaimsIdentity(claims);
//    ctx.User = new ClaimsPrincipal(identity);




//    return next();
//});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


// HttpContext has the information related to request like header, url, responce and ...
app.MapGet("/username", (HttpContext ctx) =>
{
    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var payload = authCookie.Split("=").Last();
    var parts = payload.Split(":");
    var key = parts[0];
    var value = parts[1];
    return value;
});
// here we set a cooki to the client browser with out any protection 
app.MapGet("/login", (HttpContext ctx) =>
{
    ctx.Response.Headers["set-cookie"] = "auth=usr:Keyhan";
    return "Ok";
});


// HttpContext has the information related to request like header, url, responce and ...
app.MapGet("/usernamepotected", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");

    var authCooki = ctx.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    var protectedpayload = authCooki.Split("=").Last();
    var payload = protector.Unprotect(protectedpayload);
    var parts = payload.Split(":");
    var key = parts[0];
    var value = parts[1];
    return value;
});
// here we set a cooki to the client browser 
app.MapGet("/loginprotected", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");
    ctx.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:Keyhan")}";
    return "Ok";
});





//// HttpContext has the information related to request like header, url, responce and ...
//app.MapGet("/usernamepotected2", (HttpContext ctx) =>
//{

//    return ctx.User.FindFirst("usr").Value;
//});

//// here we set a cooki to the client browser 
//app.MapGet("/loginprotected2", (AuthService auth) =>
//{
//    auth.SignIn();
//    return "Ok";
//});




// HttpContext has the information related to request like header, url, responce and ... by cookie
app.MapGet("/usernamepotected3", (HttpContext ctx) =>
{

    return ctx.User.FindFirst("usr").Value;
});


// here we set a cooki to the client browser 
app.MapGet("/loginprotected3", async (HttpContext ctx) =>
{

    // You check username and password here then create a tooken

    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "Keyhan"));
    var identity = new ClaimsIdentity( claims, "cookie");
    var user = new ClaimsPrincipal(identity);

    await ctx.SignInAsync("cookie", user);
    return "Ok";
});




app.MapControllers();

app.Run();



//public class AuthService
//{
//    private readonly IDataProtectionProvider _idp;
//    private readonly IHttpContextAccessor _accessor;


//    public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
//    {
//        _idp = idp;
//        _accessor = accessor;
//    }

//    public void SignIn()
//    {
//        var protector = _idp.CreateProtector("auth-cookie");
//        _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:Keyhan")}";
//    }


//}