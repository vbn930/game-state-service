using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Azure.Identity;

using GameStateService.Services;

using Telemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = new Uri("https://vbn930-rpg-kv.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(
    keyVaultUri,
    new DefaultAzureCredential());

IConfiguration configuration = builder.Configuration;

string serviceName = configuration["Logging:ServiceName"];
string serviceVersion = configuration["Logging:ServiceVersion"];

// 1. 서비스 구성 (DI 컨테이너에 서비스 등록)
builder.Services.AddMemoryCache();
builder.Services.AddOpenTelemetry().WithTracing(tcb =>
{
    tcb
    .AddSource(serviceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddAspNetCoreInstrumentation() // Automatically generate log lines for HTTP requests
    .AddJsonConsoleExporter(); // Output log lines to the console
});

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<MemoryCacheService>();
builder.Services.AddSingleton<GameFlowManager>();
builder.Services.AddSingleton<GameMoveHandler>(); // GameStateService 등록
builder.Services.AddSingleton<GameBattleHandler>(); // GameStateService 등록

builder.Services.AddControllers();  // MVC 컨트롤러 등록
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();   // Swagger 설정 (선택)

// 예: 추가적인 서비스 등록
// builder.Services.AddSingleton<IMyService, MyService>();

var app = builder.Build();

app.UseMiddleware<ManagerApiKeyMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>(); // API Key 미들웨어 사용

// 2. HTTP 요청 파이프라인 구성
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// 컨트롤러 엔드포인트 매핑
app.MapControllers();

app.Run();

