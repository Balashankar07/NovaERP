using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;

namespace NovaERP.Verifier;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("# Verification Evidence Log\n");
        Console.WriteLine("## 1. Build & Runtime Proof");
        
        var client = new HttpClient { BaseAddress = new Uri("http://localhost:5232") };
        
        try 
        {
            var ping = await client.GetAsync("/swagger/v1/swagger.json");
            Console.WriteLine("API Started: PASS");
            
            var swaggerContent = await ping.Content.ReadAsStringAsync();
            if(swaggerContent.Contains("/api/Reports"))
                Console.WriteLine("ReportsController in Swagger: PASS");
            else 
                Console.WriteLine("ReportsController in Swagger: FAIL");
        }
        catch(Exception)
        {
            Console.WriteLine("API Started: FAIL (Is it running?)");
            return;
        }

        Console.WriteLine("\n## 2. RBAC Verification");
        
        // Login as Super Admin
        var adminLoginRes = await client.PostAsJsonAsync("/api/Auth/login", new { Email = "admin@novaerp.com", Password = "Admin@123" });
        var adminTokenStr = await adminLoginRes.Content.ReadAsStringAsync();
        var adminToken = JsonDocument.Parse(adminTokenStr).RootElement.GetProperty("data").GetProperty("accessToken").GetString();
        
        // Login as Employee
        var empLoginRes = await client.PostAsJsonAsync("/api/Auth/login", new { Email = "employee@novaerp.com", Password = "Employee@123" });
        var empTokenStr = await empLoginRes.Content.ReadAsStringAsync();
        var empToken = JsonDocument.Parse(empTokenStr).RootElement.GetProperty("data").GetProperty("accessToken").GetString();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", empToken);
        var rbacTest = await client.GetAsync("/api/Reports/dashboard");
        Console.WriteLine($"Employee -> Dashboard -> {(int)rbacTest.StatusCode} : {(rbacTest.StatusCode == HttpStatusCode.Forbidden ? "PASS" : "FAIL")}");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var adminTest = await client.GetAsync("/api/Reports/dashboard");
        Console.WriteLine($"Super Admin -> Dashboard -> {(int)adminTest.StatusCode} : {(adminTest.StatusCode == HttpStatusCode.OK ? "PASS" : "FAIL")}");

        Console.WriteLine("\n## 3. Database vs API Verification");
        
        await using var conn = new NpgsqlConnection("Host=localhost;Port=5432;Database=NovaERPDB;Username=postgres;Password=balan123");
        await conn.OpenAsync();

        var dbProducts = Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM \"Products\"", conn).ExecuteScalarAsync());
        var dbSuppliers = Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM \"Suppliers\"", conn).ExecuteScalarAsync());
        var dbWarehouses = Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM \"Warehouses\"", conn).ExecuteScalarAsync());
        var dbInventories = Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM \"Inventories\"", conn).ExecuteScalarAsync());
        var dbSales = Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM \"SalesOrders\"", conn).ExecuteScalarAsync());
        var dbWarranties = Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM \"Warranties\"", conn).ExecuteScalarAsync());
        var dbProduction = Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM \"ProductionOrders\"", conn).ExecuteScalarAsync());

        var dashboardRes = await client.GetFromJsonAsync<JsonElement>("/api/Reports/dashboard");
        var dashboardData = dashboardRes.GetProperty("data");

        Console.WriteLine($"- Dashboard Products: DB={dbProducts} API={dashboardData.GetProperty("totalProducts").GetInt32()} -> {(dbProducts == dashboardData.GetProperty("totalProducts").GetInt32() ? "PASS" : "FAIL")}");
        Console.WriteLine($"- Dashboard Suppliers: DB={dbSuppliers} API={dashboardData.GetProperty("totalSuppliers").GetInt32()} -> {(dbSuppliers == dashboardData.GetProperty("totalSuppliers").GetInt32() ? "PASS" : "FAIL")}");
        Console.WriteLine($"- Dashboard Warehouses: DB={dbWarehouses} API={dashboardData.GetProperty("totalWarehouses").GetInt32()} -> {(dbWarehouses == dashboardData.GetProperty("totalWarehouses").GetInt32() ? "PASS" : "FAIL")}");

        var invRes = await client.GetFromJsonAsync<JsonElement>("/api/Reports/inventory");
        var invCount = invRes.GetProperty("data").GetProperty("totalCount").GetInt32();
        Console.WriteLine($"- Inventory Report: DB={dbInventories} API={invCount} -> {(dbInventories == invCount ? "PASS" : "FAIL")}");

        var prodRes = await client.GetFromJsonAsync<JsonElement>("/api/Reports/production");
        var prodCount = prodRes.GetProperty("data").GetProperty("totalCount").GetInt32();
        Console.WriteLine($"- Production Report: DB={dbProduction} API={prodCount} -> {(dbProduction == prodCount ? "PASS" : "FAIL")}");

        var salesRes = await client.GetFromJsonAsync<JsonElement>("/api/Reports/sales");
        var salesCount = salesRes.GetProperty("data").GetProperty("totalCount").GetInt32();
        Console.WriteLine($"- Sales Report: DB={dbSales} API={salesCount} -> {(dbSales == salesCount ? "PASS" : "FAIL")}");

        var warRes = await client.GetFromJsonAsync<JsonElement>("/api/Reports/warranty");
        var warCount = warRes.GetProperty("data").GetProperty("totalCount").GetInt32();
        Console.WriteLine($"- Warranty Report: DB={dbWarranties} API={warCount} -> {(dbWarranties == warCount ? "PASS" : "FAIL")}");

        var dbAudit = Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM \"AuditLogs\"", conn).ExecuteScalarAsync());
        var auditRes = await client.GetFromJsonAsync<JsonElement>("/api/Reports/audit");
        var auditCount = auditRes.GetProperty("data").GetProperty("totalCount").GetInt32();
        Console.WriteLine($"- Audit Report: DB={dbAudit} API={auditCount} -> {(dbAudit == auditCount ? "PASS" : "FAIL")}");

        Console.WriteLine("\n## 4. Regression & Performance Proof");
        var oldInvRes = await client.GetAsync("/api/Inventory");
        Console.WriteLine($"Previous API /api/Inventory -> {(int)oldInvRes.StatusCode} {(oldInvRes.IsSuccessStatusCode ? "PASS" : "FAIL")}");
        var oldWarRes = await client.GetAsync("/api/Warranties");
        Console.WriteLine($"Previous API /api/Warranties -> {(int)oldWarRes.StatusCode} {(oldWarRes.IsSuccessStatusCode ? "PASS" : "FAIL")}");
        var oldShipRes = await client.GetAsync("/api/v1/Shipments");
        Console.WriteLine($"Previous API /api/v1/Shipments -> {(int)oldShipRes.StatusCode} {(oldShipRes.IsSuccessStatusCode ? "PASS" : "FAIL")}");

        string reportRepo = await File.ReadAllTextAsync(@"E:\Nova\src\NovaERP.Infrastructure\Repositories\Reports\ReportRepository.cs");
        string reportSvc = await File.ReadAllTextAsync(@"E:\Nova\src\NovaERP.Infrastructure\Services\ReportService.cs");

        bool hasToList = reportRepo.Contains("ToList()") || reportSvc.Contains("ToList()");
        Console.WriteLine($"Performance: No ToList() found before final return? -> {(!hasToList ? "PASS" : "FAIL")}");
        bool hasIQueryable = reportRepo.Contains("IQueryable");
        Console.WriteLine($"Performance: Uses IQueryable? -> {(hasIQueryable ? "PASS" : "FAIL")}");
    }
}
